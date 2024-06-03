using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProgramGuard.Data;
using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Interfaces;
using ProgramGuard.Mappers;
using ProgramGuard.Repository;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

                    // 查询数据库，获取所有文件路径
                    var filePaths = await dbContext.FileLists.Select(f => f.FilePath).ToListAsync();

                    // 遍历文件路径，检查文件完整性
                    foreach (var filePath in filePaths)
                    {
                        var fileDetectionService = scope.ServiceProvider.GetRequiredService<IFileDetectionService>();
                        var changeLogRepository = scope.ServiceProvider.GetRequiredService<IChangeLogRepository>();

                        var isFileIntact = fileDetectionService.VerifyFileIntegrity(filePath);
                        if (!isFileIntact)
                        {
                            var fileName = Path.GetFileName(filePath);
                            var savedFileList = await dbContext.FileLists.FirstOrDefaultAsync(f => f.FileName == fileName);
                            var currentMd5 = fileDetectionService.CalculateMD5(filePath);
                            var currentSha512 = fileDetectionService.CalculateSHA512(filePath);
                            var signature = fileDetectionService.GetDigitalSignature(filePath);

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

                            await changeLogRepository.AddAsync(changelogModel);
                        }
                    }

                    // 休眠一段时间再进行下一次检查
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // 每隔30秒检查一次
            }
        }
    }
}
