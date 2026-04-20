namespace ScrumFlix.Models
{
    public class ConcessionSaleItem
    {
        public int ConcessionSaleItemId { get; set; }

        public int ConcessionSaleId { get; set; }
        public ConcessionSale? ConcessionSale { get; set; }

        public int ConcessionItemId { get; set; }
        public ConcessionItem? ConcessionItem { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal LineTotal { get; set; }
    }
}