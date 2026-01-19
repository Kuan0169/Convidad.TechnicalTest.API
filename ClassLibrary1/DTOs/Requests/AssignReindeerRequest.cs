using System.ComponentModel.DataAnnotations;

namespace Convidad.TechnicalTest.Data.DTOs.Requests
{
    public record AssignReindeerRequest
    {
        [Required]
        public Guid ReindeerId { get; init; }
    }
}
