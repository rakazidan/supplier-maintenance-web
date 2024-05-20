using System.ComponentModel.DataAnnotations;

namespace SupplierMaintenanceWeb.Models.Entities
{
    public class Supplier
    {
        [Key]
        public string SupplierCode { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string PIC { get; set; }
    }
}
