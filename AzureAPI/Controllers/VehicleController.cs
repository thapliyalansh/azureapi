using AzureAPI.IHelper;
using AzureAPI.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;


namespace AzureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class VehicleController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;
        private readonly ILogger<VehicleController> _logger;
        private readonly IIotHubService _iothubService;
        private readonly string _iotconnectionString;
        private IEnumerable<Vehicle> _vehicle;
        public VehicleController(ICosmosDbService cosmosDbService, IIotHubService iotHubService, ILogger<VehicleController> logger, IConfiguration iConfig)
        {
            _cosmosDbService = cosmosDbService;
            _logger = logger;
            _iothubService= iotHubService;
            _iotconnectionString = iConfig.GetValue<string>("iothub:connectionstring");
        }


        [HttpGet("Get")]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetAllVehicles()
        {
            _logger.LogInformation("Inforamtion: The Get method is invoked.");
            var v = await _cosmosDbService.GetAllVehiclesAsync();
            return v.ToList();
        }


        [HttpGet("Get/{id}")]
        public async Task<ActionResult> GetVehicleById(string id)
        {
            _logger.LogInformation("Inforamtion: The GetbyId method is invoked.");
            var vehicle = await _cosmosDbService.GetVehicleAsync(id);
            if (vehicle == null)
            {
                _logger.LogError("Error: ID not found.");
                return NotFound();
            }

            return Ok(vehicle);
        }


        [HttpPost("Post")]
        public async Task<IActionResult> AddVehicle(Vehicle v)
        {
            _logger.LogInformation("Inforamtion: The Post method is invoked.");
            IEnumerable<Vehicle> existingVehicles = await _cosmosDbService.GetVehicleAsync(v.id);
            if (existingVehicles.Any())
            {
                _logger.LogError("Error: Duplicated ID.");
                return BadRequest("Duplicate ID.");
            }
            Vehicle vehicle = new Vehicle();
            vehicle.type = v.type;
            vehicle.id = v.id;
            vehicle.model = v.model;
            vehicle.region = v.region;
            vehicle.tokencreated = null;
            await _cosmosDbService.AddVehicleAsync(vehicle);

            try
            {


                _logger.LogInformation("Inforamtion: Device added to IOT HUB.");
                var iothubresult = await _iothubService.RegisterDevice(v.id, _iotconnectionString);
            }
            catch(Exception ex)
            {
                _logger.LogError($"{ex.Message}", ex);
            }

            return CreatedAtAction(
                       nameof(AddVehicle),
                       vehicle);
        }


        [HttpPut("Put")]
        public async Task<IActionResult> Put(Vehicle vehicleToUpdate)
        {
            _logger.LogInformation("Inforamtion: The put method is invoked.");
            var result = await _cosmosDbService.UpdateVehicleAsync(vehicleToUpdate);
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            _logger.LogInformation("Inforamtion: The Delete method is invoked.");
            var vehicle = await _cosmosDbService.GetVehicleAsync(id);
            Vehicle v = new Vehicle();
            foreach(var i in vehicle)
            {
                v.type = i.type;
                v.id = i.id;
                v.model = i.model;
                v.region = i.region;
            }
            if (vehicle == null)
            {
                _logger.LogError("Error: Id not found.");
                return NotFound();
            }
            await _cosmosDbService.Delete(id, v.type);
            return Ok(vehicle);
        }


        [HttpGet("GetTokenTime")]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetTokenTime()
        {
            var vehicles = await _cosmosDbService.GetLastTokenTime();
            var result = vehicles.Select(v => new Vehicle
            {
                id = v.id,
                tokencreated = v.tokencreated
            }).ToList();

            return result;
        }

    }
}
