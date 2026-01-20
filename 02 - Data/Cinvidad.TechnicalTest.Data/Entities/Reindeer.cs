using System.ComponentModel.DataAnnotations;

namespace Convidad.TechnicalTest.Data.Entities
{
    public class Reindeer
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string PlateNumber { get; set; } = string.Empty;

        public double Weight { get; set; }

        public int Packets { get; set; }
        public virtual ICollection<RouteReindeer> AssignedRoutes { get; set; } = new List<RouteReindeer>();
    }
}
