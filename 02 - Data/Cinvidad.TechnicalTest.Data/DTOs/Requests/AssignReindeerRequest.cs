using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Convidad.TechnicalTest.Data.DTOs.Requests
{
    public record AssignReindeerRequest
    {
        [Required]
        public Guid ReindeerId { get; init; }
    }
}
