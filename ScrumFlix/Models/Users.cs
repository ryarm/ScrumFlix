using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ScrumFlix.Models
{
    public class User
    {
        public int UserId { get; set; }
        public int EmployeeId { get; set; }
        [MaxLength(100)]
        public string UserName { get; set; }
        [MaxLength(20)]
        public string UserPassword { get; set; }
        public int RoleId { get; set; }
        public Employee? Employee { get; set; }
    }
}
