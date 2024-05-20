using Microsoft.EntityFrameworkCore;

namespace SupplierMaintenanceWeb.Models.Entities
{
    [Keyless]
    public class Location
    {
        public string Province { get; set; }

        public string String { get; set; }
    }
}
