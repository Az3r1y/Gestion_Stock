namespace Gestion_Stock.Models
{
    public class Produit
    {
        public int Id { get; set; }
        public int QuantiteProduct { get; set; }
        public decimal Prix { get; set; }
        public string Nom { get; set; }
        public string Emplacement { get; set; }
        public int CategorieId { get; set; }
        public Categorie Categorie { get; set; } 
    }
}
