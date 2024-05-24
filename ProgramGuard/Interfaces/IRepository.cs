using ProgramGuard.Models;

namespace ProgramGuard.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }

    public interface IFileListRepository : IRepository<FileList>
    {
        // 可以在這裡添加 FileList 特有的方法
    }

    public interface IChangeLogRepository : IRepository<ChangeLog>
    {
        // 可以在這裡添加 ChangeLog 特有的方法
    }

    //public interface IUserRepository : IRepository<User>
    //{
    //    // 可以在這裡添加 User 特有的方法
    //}

}
