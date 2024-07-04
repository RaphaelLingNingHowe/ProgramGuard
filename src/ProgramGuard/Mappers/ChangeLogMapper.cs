using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Models;
namespace ProgramGuard.Mappers
{
    public static class ChangeLogMapper
    {
        public static ChangeLog ChangeLogDtoToModel(ChangeLogDTO changeLog)
        {
            return new ChangeLog
            {
                FileName = changeLog.FileName,
                MD5 = changeLog.MD5,
                SHA512 = changeLog.Sha512,
                DigitalSignature = changeLog.DigitalSignature,
                ChangeTime = changeLog.ChangeTime,
                ConfirmStatus = changeLog.ConfirmStatus,
                //ConfirmBy = changeLog.ConfirmBy,
                ConfirmTime = changeLog.ConfirmTime,
                FileListId = changeLog.FileListId
            };
        }
    }
}
