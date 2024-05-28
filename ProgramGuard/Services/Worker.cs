using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Interfaces;
using ProgramGuard.Mappers;
using ProgramGuard.Repository;

namespace ProgramGuard.Services
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ApplicationDBContext _context;
        private readonly IFileDetectionService _fileDetectionService;
        private readonly IChangeLogRepository _changeLogRepository;

        public Worker(ILogger<Worker> logger, ApplicationDBContext context, IFileDetectionService fileDetectionService, IChangeLogRepository changeLogRepository)
        {
            _logger = logger;
            _context = context;
            _fileDetectionService = fileDetectionService;
            _changeLogRepository = changeLogRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Service started......");
                // 查询数据库，获取所有文件路径
                var filePaths = await _context.FileLists.Select(f => f.FilePath).ToListAsync();

                // 遍历文件路径，检查文件完整性
                foreach (var filePath in filePaths)
                {
                    var isFileIntact = _fileDetectionService.VerifyFileIntegrity(filePath);
                    if (!isFileIntact)
                    {
                        var fileName = Path.GetFileName(filePath);
                        var savedFileList = await _context.FileLists.FirstOrDefaultAsync(f => f.FileName == fileName);
                        var currentMd5 = _fileDetectionService.CalculateMD5(filePath);
                        var currentSha512 = _fileDetectionService.CalculateSHA512(filePath);
                        var signature = _fileDetectionService.GetDigitalSignature(filePath);

                        var changelog = new ChangeLogDTO
                        {
                            FileName = fileName,
                            MD5 = currentMd5,
                            Sha512 = currentSha512,
                            DigitalSignature = signature,
                            ChangeTime = DateTime.UtcNow.ToLocalTime(),
                            ConfirmationStatus = false,
                            ConfirmedByAndTime = null,
                            FileListId = savedFileList.Id
                        };

                        var changelogModel = ChangeLogMapper.ChangeLogDtoToModel(changelog);

                        await _changeLogRepository.AddAsync(changelogModel);

                    }
                }

                // 休眠一段时间再进行下一次检查
                _logger.LogInformation("Worker running at:{time}", DateTimeOffset.Now);
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // 每隔30秒检查一次
            }
        }
    }

}
