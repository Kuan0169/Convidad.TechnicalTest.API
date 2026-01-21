using System.ComponentModel.DataAnnotations;

namespace Convidad.TechnicalTest.Data.Entities
{
    public class Route
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Region { get; set; } = string.Empty;
        public int CapacityPerNight { get; set; } = 50;

        public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
        public virtual ICollection<RouteReindeer> AssignedReindeers { get; set; } = new List<RouteReindeer>();
    }
}