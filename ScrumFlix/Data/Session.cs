using ScrumFlix.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrumFlix.Data
{
    public static class Session
    {
        public static int UserId { get; set; }
        public static string UserName { get; set; }
        public static int RoleId { get; set; }
    }
}
