using Newtonsoft.Json;

namespace Gestion_Stock.Models
{
    public class Categorie
    {
        public int Id { get; set; }
        public string Nom { get; set; }

        [JsonIgnore] // Empêche la sérialisation de la propriété Produits
        public ICollection<Produit> Produits { get; set; }
    }
}
