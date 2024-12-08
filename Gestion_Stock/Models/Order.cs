namespace Gestion_Stock.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int QuantiteOrder { get; set; }
        public DateTime DateCommande { get; set; } 
        public string Status { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; } 
        public int ProductId { get; set; }
        public Produit Product { get; set; } 
    }
}
