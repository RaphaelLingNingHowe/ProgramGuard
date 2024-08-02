using ProgramGuard.Config;
using ProgramGuard.Data;
using ProgramGuard.Dtos.FileVerification;
using ProgramGuard.Interfaces.Service;
using ProgramGuard.Models;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ProgramGuard.Services
{
    /// <summary>
    /// 負責檔案檢測和驗證的服務類別。
    /// </summary>
    public class FileDetectionService : IFileDetectionService
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<FileDetectionService> _logger;
        public FileDetectionService(ApplicationDBContext context, ILogger<FileDetectionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 獲取指定檔案的驗證結果，包括 SHA-512 哈希值和數位簽章驗證。
        /// </summary>
        /// <param name="filePath">要驗證的檔案路徑。</param>
        /// <returns>返回 <see cref="FileVerificationResultDto"/> 包含檔案的 SHA-512 哈希值和數位簽章狀態。</returns>
        public FileVerificationResultDto GetFileVerificationResult(string filePath)
        {
            var result = new FileVerificationResultDto();

            try
            {
                // 計算檔案的 SHA-512 哈希值
                using var sha512 = SHA512.Create();
                using var stream = File.OpenRead(filePath);
                byte[] hash = sha512.ComputeHash(stream);
                result.SHA512 = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
            catch (Exception ex)
            {
                // 記錄計算哈希值時發生的錯誤
                _logger.LogError(ex, "計算 SHA-512 時發生錯誤: {ErrorMessage}", ex.Message);
                result.SHA512 = string.Empty;
            }

            try
            {
                // 驗證檔案的數位簽章是否與參考證書匹配
                using var referenceCertificate = new X509Certificate2(AppSettings.CertificatePath);
                using var currentCertificate = new X509Certificate2(filePath);
                result.DigitalSignature = referenceCertificate.Thumbprint == currentCertificate.Thumbprint;
            }
            catch (CryptographicException)
            {
                // 記錄檔案沒有數位簽章的情況
                _logger.LogInformation("檔案沒有數位簽章: {FilePath}", filePath);
                result.DigitalSignature = false;
            }
            catch (FileNotFoundException ex)
            {
                // 記錄找不到檔案的錯誤
                _logger.LogError(ex, "找不到檔案: {FilePath}", filePath);
                result.DigitalSignature = false;
            }
            catch (Exception ex)
            {
                // 記錄處理證書時發生的其他錯誤
                _logger.LogError(ex, "處理證書時發生錯誤: {ErrorMessage}", ex.Message);
                result.DigitalSignature = false;
            }

            return result;
        }

        /// <summary>
        /// 驗證指定檔案的完整性，即檔案的 SHA-512 哈希值是否與異動記錄中的值匹配。
        /// </summary>
        /// <param name="filePath">要驗證的檔案路徑。</param>
        /// <returns>如果檔案的哈希值與異動記錄中的值匹配，則返回 <c>true</c>；否則返回 <c>false</c></returns>
        public bool VerifyFileIntegrity(string filePath)
        {
            // 查找最新的異動記錄條目
            if (_context.ChangeLogs
                .Where(c => c.FileList != null && c.FileList.Path == filePath)
                .OrderByDescending(c => c.Timestamp)
                .FirstOrDefault() is not ChangeLog changeLog)
            {
                return false;
            }

            // 計算檔案的 SHA-512 哈希值
            var newSha512 = GetFileVerificationResult(filePath).SHA512;
            return changeLog.SHA512 == newSha512;
        }
    }
}
