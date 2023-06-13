using System.ComponentModel.DataAnnotations;

namespace AzureAPI.Models
{
    public class Vehicle
    {
        
        public string? type { get; set; }
        [Key]
        public string? id { get; set; }
        
        public string? model { get; set; }
        public string? region{ get; set; }

        public DateTime? tokencreated { get; set; }

    }
}
