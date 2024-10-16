using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Rozetka.Data.Entity
{
    public class Brand
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = default!;
        public byte[]? ImageData { get; set; }
        //public string ImageUrl { get; set; } = default!;        
        public ICollection<Product>? Products { get; set; }
    }
}
