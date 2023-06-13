using Microsoft.AspNetCore.Mvc;
using AzureAPI.Models;

namespace AzureAPI.IHelper
{
    public interface IIotHubService
    {
        Task<int> GetDeviceNumber(string _connectionString);
        Task<string> RegisterDevice(string deviceID, string _connectionstring);
        Task<List<string>> GetConnectionSates(string _connectionString);

    }
}
