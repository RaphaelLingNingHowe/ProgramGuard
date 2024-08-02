using ProgramGuard.Dtos.FileVerification;
using ProgramGuard.Enums;
using ProgramGuard.Interfaces.Repository;
using ProgramGuard.Interfaces.Service;
using ProgramGuard.Models;

namespace ProgramGuard.Services
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();

        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker 執行時間： {time}", DateTimeOffset.Now);

            await InitializeWatchersAsync(stoppingToken);

            // Run the worker indefinitely until cancellation is requested
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task InitializeWatchersAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var fileListRepository = scope.ServiceProvider.GetRequiredService<IFileListRepository>();
            var fileList = await fileListRepository.GetAllFilesAsync();

            // 清除舊的 watchers
            foreach (var watcher in _watchers)
            {
                watcher.Dispose();
            }
            _watchers.Clear();

            foreach (var file in fileList)
            {
                string filePath = file.Path;
                string directory = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileName(filePath);

                var watcher = new FileSystemWatcher(directory)
                {
                    Filter = fileName,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.Attributes,
                    EnableRaisingEvents = true
                };

                watcher.Created += OnCreated;
                watcher.Deleted += OnDeleted;
                watcher.Changed += OnChanged;
                watcher.Renamed += OnRenamed;

                _watchers.Add(watcher);
            }
        }


        private async void OnCreated(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation($"新建檔案: {e.FullPath}");

            using var scope = _serviceScopeFactory.CreateScope();
            var pathProcessService = scope.ServiceProvider.GetRequiredService<IPathProcessService>();
            await pathProcessService.ProcessPathAsync(e.FullPath);
        }

        private async void OnDeleted(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation($"刪除檔案: {e.FullPath}");

            using var scope = _serviceScopeFactory.CreateScope();
            var fileListRepository = scope.ServiceProvider.GetRequiredService<IFileListRepository>();
            var changeLogRepository = scope.ServiceProvider.GetRequiredService<IChangeLogRepository>();
            var fileId = await fileListRepository.DeleteFileAsync(e.FullPath);
            ChangeLog changeLog = new()
            {
                FileListId = fileId,
                ChangeType = (byte?)ChangeType.Deleted,
            };
            await changeLogRepository.AddAsync(changeLog);
        }

        private async void OnChanged(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation($"檔案異動: {e.FullPath}");

            using var scope = _serviceScopeFactory.CreateScope();
            var fileListRepository = scope.ServiceProvider.GetRequiredService<IFileListRepository>();
            var changeLogRepository = scope.ServiceProvider.GetRequiredService<IChangeLogRepository>();
            var fileDetectionService = scope.ServiceProvider.GetRequiredService<IFileDetectionService>();
            var isIntact = fileDetectionService.VerifyFileIntegrity(e.FullPath);
            if (!isIntact)
            {
                var file = await fileListRepository.GetFileAsync(e.FullPath);

                if (file != null)
                {
                    FileVerificationResultDto fileVerification = fileDetectionService.GetFileVerificationResult(e.FullPath);

                    ChangeLog changeLog = new()
                    {
                        FileListId = file.Id,
                        ChangeType = (byte?)ChangeType.Changed,
                        DigitalSignature = fileVerification.DigitalSignature,
                        SHA512 = fileVerification.SHA512
                    };
                    await changeLogRepository.AddAsync(changeLog);
                }
            }
        }


        private async void OnRenamed(object sender, RenamedEventArgs e)
        {
            var detail = $"原路徑：{e.OldFullPath}，新路徑：{e.FullPath}";
            _logger.LogInformation(detail);

            using var scope = _serviceScopeFactory.CreateScope();
            var fileListRepository = scope.ServiceProvider.GetRequiredService<IFileListRepository>();
            var changeLogRepository = scope.ServiceProvider.GetRequiredService<IChangeLogRepository>();
            var fileId = await fileListRepository.RenameFileAsync(e.OldFullPath, e.FullPath);
            ChangeLog changeLog = new()
            {
                FileListId = fileId,
                ChangeType = (byte?)ChangeType.Renamed,
                ChangeDetails = detail
            };
            await changeLogRepository.AddAsync(changeLog);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var watcher in _watchers)
            {
                watcher.Dispose();
            }
            return base.StopAsync(cancellationToken);
        }
    }
}
