using ProgramGuard.Enums;

namespace ProgramGuard.Dtos.ActionLog
{
    public class LogActionDto
    {
        public ACTION Action { get; set; }
        public string? Comment { get; set; }
    }
}
