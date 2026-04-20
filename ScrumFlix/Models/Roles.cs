using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ScrumFlix.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        [MaxLength(30)]
        public string RoleName { get; set; }
    }
}
