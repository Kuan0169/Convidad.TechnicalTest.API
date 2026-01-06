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

        [HttpGet("children/naughty")]
        public IEnumerable<Child> GetNaughtyChildren()
        {
            var naughtyChildren = santaService.GetNaughtyChildren();
            return naughtyChildren;
        }

        [HttpGet("deliveries/failures")]
        public IEnumerable<Delivery> GetFailureDeliveries()
        {
            var failureDeliveries = santaService.GetFailureDeliveries();
            return failureDeliveries;
        }

        [HttpGet("children/{childId}/wishlist")] 
        public IEnumerable<Wish> GetWishlistByChildId(Guid childId)
        {
            var wishes = santaService.GetWishlistByChildId(childId);
            return wishes;
        }

        [HttpGet("children/{childId}/wishlist/priority")]
        public IEnumerable<Wish> GetWishlistByChildIdOrderedByPriority(Guid childId)
        {
            var wishesProirity =santaService.GetWishlistByChildIdOrderedByPriority(childId);
            return wishesProirity;
        }

        [HttpGet("reindeers")]
        public IEnumerable<Reindeer> GetAllReindeers()
        {
            var reindeers = santaService.GetAllReindeers();
            return reindeers;
        }

        [HttpGet("reindeers/{id}")]
        public Reindeer GetReindeerById(Guid id)
        {
            var reindeer = santaService.GetReindeerById(id);
            return reindeer;
        }

        [HttpPost("reindeers")]
        public ActionResult AddReindeer([FromBody] Reindeer reindeer)
        {
            santaService.AddReindeer(reindeer);
            return CreatedAtAction(nameof(GetReindeerById), new { id = reindeer.Id }, reindeer);
        }

        [HttpPost("deliveries/{deliveryId}/assign-reindeer/{reindeerId}")]
        public ActionResult AssignReindeerToDelivery(Guid deliveryId, Guid reindeerId)
        {
            santaService.AssignReindeerToDelivery(deliveryId, reindeerId);
            return NoContent();
        }
    }
}
