using System.ComponentModel.DataAnnotations;

namespace TEST_DATA_SETUP.Models
{
    public class Vendor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
    }
}
