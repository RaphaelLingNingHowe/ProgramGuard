namespace ProgramGuard.Interfaces
{
    public interface IFileDetectionService
    {
        string CalculateMD5(string filePath);
        string CalculateSHA512(string filePath);
        List<string> GetDigitalSignature(string filePath);
        bool VerifyFileIntegrity(string filePath);
    }
}
