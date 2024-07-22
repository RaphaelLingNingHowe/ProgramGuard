using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Interfaces;
using ProgramGuard.Mappers;
using ProgramGuard.Models;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                var fileDetectionService = scope.ServiceProvider.GetRequiredService<IFileDetectionService>();
                var changeLogRepository = scope.ServiceProvider.GetRequiredService<IChangeLogRepository>();

                try
                {
                    var fileLists = await dbContext.FileLists.ToListAsync();

                    foreach (var fileList in fileLists)
                    {
                        await ProcessFile(fileList, fileDetectionService, changeLogRepository, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing files");
                }
            }

            var delay = _configuration.GetValue<int>("WorkerDelaySeconds", 30);
            await Task.Delay(TimeSpan.FromSeconds(delay), stoppingToken);
        }
    }

    private async Task ProcessFile(FileList fileList, IFileDetectionService fileDetectionService, IChangeLogRepository changeLogRepository, CancellationToken stoppingToken)
    {
        if (!File.Exists(fileList.FilePath))
        {
            _logger.LogWarning($"File {fileList.FilePath} does not exist.");
            return;
        }

        try
        {
            var isFileIntact = fileDetectionService.VerifyFileIntegrity(fileList.FilePath);
            if (!isFileIntact)
            {
                var currentMd5 = fileDetectionService.CalculateMD5(fileList.FilePath);
                var currentSha512 = fileDetectionService.CalculateSHA512(fileList.FilePath);
                var signature = fileDetectionService.HasValidDigitalSignature(fileList.FilePath);

                var changelog = new ChangeLogDTO
                {
                    FileName = fileList.FileName,
                    MD5 = currentMd5,
                    Sha512 = currentSha512,
                    DigitalSignature = signature,
                    ChangeTime = DateTime.UtcNow.ToLocalTime(),
                    ConfirmStatus = false,
                    ConfirmBy = null,
                    ConfirmTime = null,
                    FileListId = fileList.Id
                };

                var changelogModel = ChangeLogMapper.ChangeLogDtoToModel(changelog);
                await changeLogRepository.AddAsync(changelogModel);

                _logger.LogInformation($"Change detected for file: {fileList.FileName}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing file: {fileList.FilePath}");
        }
    }
}