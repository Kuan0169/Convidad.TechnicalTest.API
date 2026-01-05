using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Convidad.TechnicalTest.Data.Entities
{
    public class Route
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(120)]
        public string Name { get; set; } = default!;

        [Required, MaxLength(32)]
        public string Region { get; set; } = default!;

        public int? CapacityPerNight { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    }
}
