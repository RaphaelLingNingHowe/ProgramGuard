using ProgramGuard.Dtos.LogQuery;
using ProgramGuard.Interfaces;
using ProgramGuard.Models;

namespace ProgramGuard.Repository
{
    public class QueryConditionHandler : IQueryConditionHandler
    {
        public IQueryable<ChangeLog> HandleQueryConditions(IQueryable<ChangeLog> query, SearchDto queryDto)
        {
            if (!string.IsNullOrWhiteSpace(queryDto.FileName))
            {
                query = query.Where(cl => cl.FileName.Contains(queryDto.FileName));
            }

            if (queryDto.StartTime != null)
            {
                query = query.Where(cl => cl.ChangeTime >= queryDto.StartTime);
            }

            if (queryDto.EndTime != null)
            {
                query = query.Where(cl => cl.ChangeTime <= queryDto.EndTime);
            }

            return query;
        }
    }

}
