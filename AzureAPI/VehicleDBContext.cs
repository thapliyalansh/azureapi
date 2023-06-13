using AzureAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureAPI
{
    public class VehicleDBContext: DbContext
    {
        public VehicleDBContext(DbContextOptions<VehicleDBContext> options) : base(options)
        { 
       
        } 
        public DbSet<Vehicle> Vehicles { get; set; }
    }
}
