using Convidad.TechnicalTest.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Convidad.TechnicalTest.Data.Entities
{
    public class Wish
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ChildId { get; set; }

        [ForeignKey(nameof(ChildId))]
        public Child Child { get; set; } = default!;

        public WishCategory Category { get; set; } = WishCategory.Other;

        [Range(1, 5)]
        public int Priority { get; set; } = 3;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
