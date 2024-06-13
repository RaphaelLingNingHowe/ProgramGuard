using ProgramGuard.Dtos.LogQuery;
using ProgramGuard.Models;
namespace ProgramGuard.Interfaces
{
    public interface IQueryConditionHandler
    {
        IQueryable<ChangeLog> HandleQueryConditions(IQueryable<ChangeLog> query, SearchDto queryDto);
    }
}
