using Convidad.TechnicalTest.API.DTOs;
using Convidad.TechnicalTest.Data.DTOs.Requests;
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

        [HttpGet("children")]
        public ActionResult<IEnumerable<ChildDto>> GetAllChildren()
        {
            var children = santaService.GetAllChildren();
            var dtos = children.Select(c => new ChildDto(c.Id, c.Name, c.CountryCode, c.IsNice));
            return Ok(dtos);
        }

        [HttpGet("children/naughty")]
        public ActionResult<IEnumerable<ChildDto>> GetNaughtyChildren()
        {
            var children = santaService.GetNaughtyChildren();
            var dtos = children.Select(c => new ChildDto(c.Id, c.Name, c.CountryCode, c.IsNice));
            return Ok(dtos);
        }

        [HttpGet("children/{childId}/wishlist")]
        public ActionResult<IEnumerable<WishDto>> GetWishlistByChildId(Guid childId)
        {
            var wishes = santaService.GetWishlistByChildId(childId);
            var dtos = wishes.Select(w => new WishDto(w.Id, w.Category.ToString(), w.Priority));
            return Ok(dtos);
        }

        [HttpGet("children/{childId}/wishlist/priority")]
        public ActionResult<IEnumerable<WishDto>> GetWishlistByChildIdOrderedByPriority(Guid childId)
        {
            var wishes = santaService.GetWishlistByChildIdOrderedByPriority(childId);
            var dtos = wishes.Select(w => new WishDto(w.Id, w.Category.ToString(), w.Priority));
            return Ok(dtos);
        }

        [HttpGet("deliveries")]
        public ActionResult<IEnumerable<Delivery>> GetDeliveries()
        {
            var deliveries = santaService.GetDeliveries();
            return Ok(deliveries);
        }

        [HttpGet("deliveries/failures")]
        public ActionResult<IEnumerable<Delivery>> GetFailureDeliveries()
        {
            var failureDeliveries = santaService.GetFailureDeliveries();
            return Ok(failureDeliveries);
        }

        [HttpGet("reindeers")]
        public ActionResult<IEnumerable<ReindeerDto>> GetAllReindeers()
        {
            var reindeers = santaService.GetAllReindeers();
            var dtos = reindeers.Select(r => new ReindeerDto(
                r.Id, r.Name, r.PlateNumber, r.Weight, r.Packets));
            return Ok(dtos);
        }

        [HttpGet("reindeers/{id}")]
        public ActionResult<ReindeerDto> GetReindeerById(Guid id)
        {
            try
            {
                var reindeer = santaService.GetReindeerById(id);
                var dto = new ReindeerDto(
                    reindeer.Id, reindeer.Name, reindeer.PlateNumber,
                    reindeer.Weight, reindeer.Packets);
                return Ok(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Reindeer with ID {id} not found.");
            }
        }

        [HttpPost("reindeers")]
        public ActionResult<ReindeerDto> AddReindeer([FromBody] ReindeerDto reindeerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reindeer = new Reindeer
            {
                Name = reindeerDto.Name,
                PlateNumber = reindeerDto.PlateNumber,
                Weight = reindeerDto.Weight,
                Packets = reindeerDto.Packets
            };

            santaService.AddReindeer(reindeer);

            var createdDto = new ReindeerDto
            (
                reindeer.Id, reindeer.Name, reindeer.PlateNumber,
                reindeer.Weight, reindeer.Packets
            );

            return CreatedAtAction(nameof(GetReindeerById), new { id = reindeer.Id }, createdDto);
        }

        [HttpPost("deliveries/{deliveryId}/assign-reindeer")]
        public ActionResult AssignReindeerToDelivery
            (
                Guid deliveryId,
                [FromBody] AssignReindeerRequest request
            )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                santaService.AssignReindeerToDelivery(deliveryId, request.ReindeerId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
