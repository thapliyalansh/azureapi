using AzureAPI.IHelper;
using AzureAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Azure.Devices.Shared;
using System.Threading.Tasks;

namespace AzureAPI.Helper
{
    public class IotHubService : IIotHubService
    {
        static RegistryManager registryManager;

        public async Task<string> RegisterDevice(string deviceID, string _connectionString)
        {
            registryManager = RegistryManager.CreateFromConnectionString(_connectionString);
            await AddDeviceAsync(deviceID);
            return "Device registered successfully";
        }

        public static async Task AddDeviceAsync(string deviceId)
        {
            Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                Console.WriteLine("Device: {0} alread exist", deviceId);
                device = await registryManager.GetDeviceAsync(deviceId);
            }

            if (device.Authentication.SymmetricKey != null)
            {
                //Console.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);
            }
            else
            {
                // Console.WriteLine("SymmetricKey is null or invalid.");
            }
        }

        public async Task<int> GetDeviceNumber(string _connectionString)
        {
            registryManager = RegistryManager.CreateFromConnectionString(_connectionString);
            IEnumerable<Device> device = await registryManager.GetDevicesAsync(100);

            return device.Count();
        }

        public async Task<List<string>> GetConnectionSates(string _connectionString)
        {
            RegistryManager registryManager = RegistryManager.CreateFromConnectionString(_connectionString);
            var devices = await registryManager.GetDevicesAsync(int.MaxValue);
            List<string> connectionStates = new List<string>();
           
            foreach (var device in devices)
            {
                Twin twin = await registryManager.GetTwinAsync(device.Id);
                connectionStates.Add(twin.ConnectionState.ToString());
            }

            return connectionStates;
        }
    }
}