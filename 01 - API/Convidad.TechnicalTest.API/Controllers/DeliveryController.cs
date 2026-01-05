using Convidad.TechnicalTest.Data.Entities;
using Convidad.TechnicalTest.Services.SantaService;
using Microsoft.AspNetCore.Mvc;

namespace Convidad.TechnicalTest.API.Controllers
{
    [ApiController]
    [Route("delivery")]
    public class DeliveryController(ISantaService santaService) : ControllerBase
    {
        private readonly ISantaService santaService = santaService;

        public IEnumerable<Delivery> Get()
        {
            var deliveries = santaService.GetDeliveries();
            return deliveries;
        }

        [HttpGet("children")]
        public IEnumerable<Child> GetAllChildren()
        {
            var children = santaService.GetAllChildren();
            return children;
        }
    }
}
