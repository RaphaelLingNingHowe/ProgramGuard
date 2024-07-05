using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Interfaces;
using ProgramGuard.Mappers;
namespace ProgramGuard.Services
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                    _logger.LogInformation("Service started...");
                    var filePaths = await dbContext.FileLists.Select(f => f.FilePath).ToListAsync();
                    foreach (var filePath in filePaths)
                    {
                        if (!File.Exists(filePath))
                        {
                            _logger.LogWarning($"File {filePath} does not exist.");
                            continue;
                        }
                        var fileDetectionService = scope.ServiceProvider.GetRequiredService<IFileDetectionService>();
                        var changeLogRepository = scope.ServiceProvider.GetRequiredService<IChangeLogRepository>();
                        var isFileIntact = fileDetectionService.VerifyFileIntegrity(filePath);
                        if (!isFileIntact)
                        {
                            var fileName = Path.GetFileName(filePath);
                            var savedFileList = await dbContext.FileLists.OrderByDescending(f => f.FileName == fileName).FirstOrDefaultAsync();
                            var currentMd5 = fileDetectionService.CalculateMD5(filePath);
                            var currentSha512 = fileDetectionService.CalculateSHA512(filePath);
                            var signature = fileDetectionService.HasValidDigitalSignature(filePath);
                            var changelog = new ChangeLogDTO
                            {
                                FileName = fileName,
                                MD5 = currentMd5,
                                Sha512 = currentSha512,
                                DigitalSignature = signature,
                                ChangeTime = DateTime.UtcNow.ToLocalTime(),
                                ConfirmStatus = false,
                                ConfirmBy = null,
                                ConfirmTime = null,
                                FileListId = savedFileList.Id
                            };
                            var changelogModel = ChangeLogMapper.ChangeLogDtoToModel(changelog);
                            await changeLogRepository.AddAsync(changelogModel);
                        }
                    }
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
