using System.ComponentModel.DataAnnotations;

namespace Convidad.TechnicalTest.Data.Entities
{
    public class Child
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required, MaxLength(120)]
        public string Name { get; set; } = default!;
        [Required, MaxLength(2)]
        public string CountryCode { get; set; } = default!;
        public bool IsNice { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public virtual ICollection<Wish> Wishes { get; set; } = new List<Wish>();
        public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
    }
}
