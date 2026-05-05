using System;
using System.Collections.Generic;
using System.Text;

namespace ScrumFlix.Models
{
    public class Timesheet
    {
        public int TimesheetId { get; set; }
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public int PayPeriodId { get; set; }
        public PayPeriod? PayPeriod { get; set; }

        public decimal TotalHours { get; set; }
        public bool Approved { get; set; }

        public int? ApprovedByUserId { get; set; }
        public User? ApprovedByUser { get; set; }
    }
}
