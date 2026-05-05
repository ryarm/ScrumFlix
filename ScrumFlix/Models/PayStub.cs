using System;
using System.Collections.Generic;
using System.Text;

namespace ScrumFlix.Models
{
    public class PayStub
    {
        public int PayStubId { get; set; }

        public int PayrollId { get; set; }
        public Payroll? Payroll { get; set; }

        public DateTime IssueDate { get; set; }
    }
}
