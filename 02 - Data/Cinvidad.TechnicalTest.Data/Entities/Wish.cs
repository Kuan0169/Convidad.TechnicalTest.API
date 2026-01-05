using Cinvidad.TechnicalTest.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Cinvidad.TechnicalTest.Data.Entities
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

        // 1..5 typically
        [Range(1, 5)]
        public int Priority { get; set; } = 3;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
