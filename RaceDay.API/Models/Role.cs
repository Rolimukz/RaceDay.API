using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Models
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<User>? Users { get; set; }
    }
}
