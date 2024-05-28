using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Models;
using System.ComponentModel.DataAnnotations.Schema;

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
                ConfirmationStatus = changeLog.ConfirmationStatus,
                ConfirmedByAndTime = changeLog.ConfirmedByAndTime,
                FileListId = changeLog.FileListId
            };
        }
    }
}
