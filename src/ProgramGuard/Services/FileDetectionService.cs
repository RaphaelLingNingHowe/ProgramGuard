using ProgramGuard.Data;
using ProgramGuard.Interfaces;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
namespace ProgramGuard.Services
{
    public class FileDetectionService : IFileDetectionService
    {
        private readonly ApplicationDBContext _context;
        public FileDetectionService(ApplicationDBContext context)
        {
            _context = context;
        }
        public string CalculateMD5(string filePath)
        {
            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        byte[] hash = md5.ComputeHash(stream);
                        StringBuilder sb = new StringBuilder();
                        foreach (byte b in hash)
                        {
                            sb.Append(b.ToString("x2"));
                        }
                        return sb.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating MD5: {ex.Message}");
                return null;
            }
        }
        public string CalculateSHA512(string filePath)
        {
            try
            {
                using (var sha512 = SHA512.Create())
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        byte[] hash = sha512.ComputeHash(stream);
                        StringBuilder sb = new StringBuilder();
                        foreach (byte b in hash)
                        {
                            sb.Append(b.ToString("x2"));
                        }
                        return sb.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating SHA-512: {ex.Message}");
                return null;
            }
        }
        public List<string> GetDigitalSignature(string filePath)
        {
            List<string> signatureInfo = new List<string>(); // 創建存儲數位簽章信息的列表
            try
            {
                using (X509Certificate2 certificate = new X509Certificate2(filePath))
                {
                    // 獲取憑證的發行者和有效期
                    string subject = certificate.Subject;
                    string issuer = certificate.Issuer;
                    DateTime expirationDate = certificate.NotAfter;
                    // 添加數位簽章信息到列表中
                    signatureInfo.Add($"Subject: {subject}");
                    signatureInfo.Add($"Issuer: {issuer}");
                    signatureInfo.Add($"Expiration Date: {expirationDate}");
                }
                // 返回包含數位簽章信息的列表
                return signatureInfo;
            }
            catch (CryptographicException)
            {
                // 憑證沒有數位簽章的處理
                return null;
            }
            catch (FileNotFoundException ex)
            {
                // 檔案不存在的處理
                throw new Exception($"Error processing certificate: File not found - {ex.Message}");
            }
            catch (Exception ex)
            {
                // 其他異常的處理
                throw new Exception($"Error processing certificate: {ex.Message}");
            }
        }
        public bool VerifyFileIntegrity(string filePath)
        {
            var changeLog = _context.ChangeLogs
                .Where(c => c.FileList.FilePath == filePath)
                .OrderByDescending(c => c.ChangeTime)
                .FirstOrDefault();
            if (changeLog == null)
            {
                // 文件不存在或無法驗證
                return false;
            }
            var oldMD5 = changeLog.MD5;
            var oldSha512 = changeLog.SHA512;
            // 計算新的 MD5 和 SHA512 值
            var newMD5 = CalculateMD5(filePath);
            var newSha512 = CalculateSHA512(filePath);
            // 比較 MD5 和 SHA512 值
            return oldMD5 == newMD5 && oldSha512 == newSha512;
        }
    }
}
