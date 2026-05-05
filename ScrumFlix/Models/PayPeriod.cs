using System;
using System.Collections.Generic;
using System.Text;

namespace ScrumFlix.Models
{
    public class PayPeriod
    {
        public int PayPeriodId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
