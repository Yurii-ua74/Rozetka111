using Microsoft.AspNetCore.Mvc.Rendering;
using Rozetka.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace Rozetka.Models.ViewModels.Products
{
    public class ProductAdvertisementVM
    {
        //public CreateProductVM Product { get; set; }

        [Required]
        public string Title { get; set; }

        public List<IFormFile> ImageData { get; set; }
       
        public string? ProductType { get; set; }
        public string? ProductColor { get; set; }


        [Required]
        public string Description { get; set; }

        [Required]
        public string Characteristic { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Brand { get; set; }
        public int BrandId { get; set; }

        public int QuantityInStock { get; set; }


        public int ProductTypeId { get; set; }
        public int ProductColorId { get; set; }
        public int CategoryId { get; set; }
        public int ChildcategoryId { get; set; }
        public int SubChildCategoryId { get; set; }


        public List<Brand>? Brands { get; set; }
        public List<ProductType>? ProductTypes { get; set; } // Список типів продукту
        public List<ProductColor>? ProductColors { get; set; } 
        public List<Category>? Categories { get; set; }
        public List<Childcategory>? Childcategories { get; set; } 
        public List<SubChildCategory>? SubChildCategories { get; set; }


    }
}
