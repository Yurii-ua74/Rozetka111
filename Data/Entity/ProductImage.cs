using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Rozetka.Data.Entity
{
    [Table("ProductImages")]
    public class ProductImage
    {
        public int Id { get; set; }
        public byte[]? ImageData { get; set; }
        public int? ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]        
        public Product? Product { get; set; } 
        //public string ImageUrl { get; set; } = default!;
    }
}
