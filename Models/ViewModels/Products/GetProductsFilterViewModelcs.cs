﻿using Rozetka.Data.Entity;

namespace Rozetka.Models.ViewModels.Products
{
    public class GetProductsFilterViewModelcs
    {
        public IEnumerable<Product>? Products { get; set; }
        public Category? Category { get; set; }
        public Childcategory? ChildCategory { get; set; }

        public List<string>? FilterBuyers { get; set; } = new(); //по продавцах
        public List<string>? FilterTypes { get; set; } = new(); //по типу
        public List<string>? FilterBrands { get; set; } = new(); //по бренду
        public decimal? StartPrace { get; set; }
        public decimal? EndPrace { get; set; }

        public bool? IsFilterBuyers { get; set; } = false;
        public bool? IsFilterTypes { get; set; } = false;
        public bool? IsFilterBrands { get;set; } = false;
        public bool? IsFilterPrace { get;set; } = false;
    }
}
