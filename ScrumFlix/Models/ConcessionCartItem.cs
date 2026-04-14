using System;
using System.Collections.Generic;
using System.Text;

namespace ScrumFlix.Models
{
    public class ConcessionCartItem
    {
        public int ConcessionItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }
}