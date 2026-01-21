using Convidad.TechnicalTest.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Convidad.TechnicalTest.Data.Entities
{
    public class Delivery
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ChildId { get; set; }

        [ForeignKey(nameof(ChildId))]
        public Child Child { get; set; } = default!;

        [Required]
        public Guid RouteId { get; set; }

        [ForeignKey(nameof(RouteId))]
        public Route Route { get; set; } = default!;

        public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;

        [Required]
        public DateTime ScheduledFor { get; set; } 

        public DateTimeOffset? DeliveredAt { get; set; }

        public FailureReason? FailureReason { get; set; }

        [MaxLength(250)]
        public string? FailureDetails { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
