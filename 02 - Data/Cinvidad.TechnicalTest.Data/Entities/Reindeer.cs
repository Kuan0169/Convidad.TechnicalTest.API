using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Convidad.TechnicalTest.Data.Entities
{
    public class Reindeer
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;

        [Required, MaxLength(20)]
        public string PlateNumber { get; set; } = default!;

        [Range(0, double.MaxValue)]
        public double Weight { get; set; }

        [Range(0, int.MaxValue)]
        public int Packets { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
