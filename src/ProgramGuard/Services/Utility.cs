namespace ProgramGuard.Services
{
    public static class Utility
    {
        public static IQueryable<T> SkipAndTake<T>(this IQueryable<T> query, int skip, int take)
        {
            if (skip > 0)
            {
                query = query.Skip(skip);
            }

            if (take > 0)
            {
                query = query.Take(take);
            }

            return query;
        }
    }
}
