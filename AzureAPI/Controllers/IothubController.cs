using AzureAPI.Helper;
using AzureAPI.IHelper;
using AzureAPI.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AzureAPI.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class IothubController : Controller
    {
        private readonly IIotHubService _iothubService;
        private readonly string _iotconnectionString;
        public IothubController(IIotHubService iotHubService,  IConfiguration iConfig)
        {
          
            _iothubService = iotHubService;
            _iotconnectionString = iConfig.GetValue<string>("iothub:connectionstring");
        }

        [HttpGet("GetNumber")]
        public async Task<int> GetDevices()
        {
            int d =await _iothubService.GetDeviceNumber(_iotconnectionString);
            return d;
        }

        [HttpGet("GetConnectionStates")]

        public async Task<int[]> GetConnectionStates()
        {
            int connected = 0;
            int disconnected = 0;
            List<string> states = await _iothubService.GetConnectionSates(_iotconnectionString);

            foreach (string s in states)
            {
                if (s == "Connected") { connected++; }
                else { disconnected++; }
            }
            int[] States = { connected, disconnected };
            return States;
        }

    }
}
