using ProgramGuard.Dtos.LogQuery;
using ProgramGuard.Models;
namespace ProgramGuard.Mappers
{
    public static class ActionLogMapper
    {
        public static ActionLog ActionLogDtoToModel(ActionLogDto actionLog)
        {
            return new ActionLog
            {
                UserName = actionLog.UserName,
                Action = actionLog.Action,
                ActionTime = actionLog.ActionTime
            };
        }
    }
}
