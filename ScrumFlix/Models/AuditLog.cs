using System;
using System.Collections.Generic;
using System.Text;

namespace ScrumFlix.Models
{
    public class AuditLog
    {
        public int AuditLogId { get; set; }
        public int UserId { get; set; }
        public string ActionType { get; set; } = "";
        public string TableName { get; set; } = "";
        public int? ObjectId { get; set; }
        public DateTime ActionTime { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? Description { get; set; }

        public User? User { get; set; }
    }
}
