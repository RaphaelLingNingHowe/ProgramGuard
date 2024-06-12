using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Dtos.User;
using ProgramGuard.Models;

namespace ProgramGuard.Dtos.Account
{
    public class FreezeAccountDto
    {
        public string UserId { get; set; }
    }
}
