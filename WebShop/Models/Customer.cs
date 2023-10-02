using WebShop.Models;

namespace WebshopService.Models
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public string Firstname { get; set; }
        public DateTime Birthdate { get; set; }
        public CustomerAddress Address { get; set; }
        public string JobTitle { get; set; }
        public CustomerCategory Category { get; set; }
        public string EmailAddress { get; set; }
        public ICollection<Order> Orders { get; set; }
    }

    public enum CustomerCategory
    {
        NEW_CUSTOMER,
        EXISTING_CUSTOMER,
        INACTIVE_CUSTOMER
    }
}
