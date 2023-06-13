using AzureAPI.IHelper;
using AzureAPI.Models;
using Microsoft.Azure.Cosmos;


namespace AzureAPI.Helper
{
    public class CosmosDbService : ICosmosDbService
    {
        private Container _container;
        public CosmosDbService(CosmosClient cosmosClient, string databasename, string containername)
        {
            _container = cosmosClient.GetContainer(databasename, containername);

        }


        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
        {
            var query = _container.GetItemQueryIterator<Vehicle>(new QueryDefinition("Select * from c"));
            var results = new List<Vehicle>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }



        public async Task<IEnumerable<Vehicle>> GetVehicleAsync(string id)
        {
            var query = _container.GetItemQueryIterator<Vehicle>(new QueryDefinition("Select * from c  where c.id=" + "\"" + id + "\""));
            var result = new List<Vehicle>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                result.AddRange(response.ToList());
            }
            return result;
        }




        public async Task AddVehicleAsync(Vehicle vehicle)
        {
            await _container.CreateItemAsync(vehicle, new PartitionKey(vehicle.type));
        }

        public async Task<Vehicle> UpdateVehicleAsync(Vehicle vehicleToUpdate)
        {
            var item = await _container.UpsertItemAsync(vehicleToUpdate, new PartitionKey(vehicleToUpdate.type));
            return item;
        }

        public async Task Delete(string id, string type)
        {
            await _container.DeleteItemAsync<Vehicle>(id, new PartitionKey(type));
        }

        public async Task<IEnumerable<Vehicle>> GetLastTokenTime()
        {
            var query = _container.GetItemQueryIterator<Vehicle>(new QueryDefinition("Select c.id, c.tokencreated from c where c.tokencreated != null "));
            var result = new List<Vehicle>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                result.AddRange(response.Select(item => new Vehicle
                {
                    id = item.id,
                    tokencreated = item.tokencreated
                }));
                return result;
            }
            return result;
        }
    }
}

