﻿using ProgramGuard.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProgramGuard.Models
{
    public class ActionLog
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // This is the foreign key to AspNetUsers table

        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        [Required]
        public ACTION Action { get; set; }

        public string Comment { get; set; }

        [Required]
        public DateTime ActionTime { get; set; }
    }
}
