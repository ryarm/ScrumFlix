using System;
using System.Collections.Generic;
using System.Text;

namespace ScrumFlix.Models
{
    public class Payroll
    {
        public int PayrollId { get; set; }

        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public int PayPeriodId { get; set; }
        public PayPeriod? PayPeriod { get; set; }

        public decimal GrossPay { get; set; }
    }
}
