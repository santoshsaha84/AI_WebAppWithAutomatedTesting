using System.Collections.Generic;

namespace TEST_DATA_SETUP.Models
{
    public partial class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public int DisplayOrder { get; set; }

        public virtual ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
}
