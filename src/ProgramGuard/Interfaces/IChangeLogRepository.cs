﻿using ProgramGuard.Models;
namespace ProgramGuard.Interfaces
{
    public interface IChangeLogRepository
    {
        Task AddAsync(ChangeLog changeLog);
        Task<IEnumerable<ChangeLog>> GetAllAsync();
    }
}
