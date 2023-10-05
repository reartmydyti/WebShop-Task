namespace WebshopService.Models
{
    public class CustomerAddress
    {
        public Guid CustomerAddressId { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }

        public override string ToString()
        {
            return $"{Street}, {City}, {State}, {Country}, {ZipCode}";
        }
    }

}
