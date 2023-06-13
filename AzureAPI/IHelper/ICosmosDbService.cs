using AzureAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace AzureAPI.IHelper
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();

        Task<IEnumerable<Vehicle>> GetVehicleAsync(string id);
        Task AddVehicleAsync(Vehicle vehicle);

        Task<Vehicle> UpdateVehicleAsync(Vehicle vehicleToUpdate);

        Task Delete(string id, string type);
        Task<IEnumerable<Vehicle>> GetLastTokenTime();


    }
}