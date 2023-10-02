using System.Text.Json.Serialization;
using WebshopService.Models;

namespace WebShop.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        //[JsonIgnore]
        //public Customer Customer { get; set; }  
        public string AdditionalInfo { get; set; }
        public List<Product> Products { get; set; }
    }
}