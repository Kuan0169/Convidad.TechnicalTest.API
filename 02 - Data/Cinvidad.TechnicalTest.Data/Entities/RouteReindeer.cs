namespace Convidad.TechnicalTest.Data.Entities
{
    public class RouteReindeer
    {
        public Guid RouteId { get; set; }
        public Guid ReindeerId { get; set; }

        public int MaxDeliveries { get; set; } = 10;
        public int CurrentDeliveries { get; set; } = 0;

        public virtual Route Route { get; set; } = null!;
        public virtual Reindeer Reindeer { get; set; } = null!;
    }
}
