﻿namespace WebshopService.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
