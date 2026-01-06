using Convidad.TechnicalTest.Data.Entities;
using Convidad.TechnicalTest.Data.Enums;
using Convidad.TechnicalTest.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Convidad.TechnicalTest.Services.SantaService
{
    public interface ISantaService
    {
        public IEnumerable<Child> GetAllChildren();
        public IEnumerable<Delivery> GetDeliveries();
        public IEnumerable<Child> GetNaughtyChildren();
        public IEnumerable<Delivery> GetFailureDeliveries();
        public IEnumerable<Wish> GetWishes();
        public IEnumerable<Wish> GetWishlistByChildId(Guid childId);
        public IEnumerable<Wish> GetWishlistByChildIdOrderedByPriority(Guid childId);
        public IEnumerable<Reindeer> GetAllReindeers();
        public Reindeer GetReindeerById(Guid id);
        public void AddReindeer(Reindeer reindeer);
        public void AssignReindeerToDelivery(Guid deliveryId, Guid reindeerId);
    }

    public class SantaService(SantaDbContext santaDb) : ISantaService
    {
        private readonly SantaDbContext santaDb = santaDb;
        
        public IEnumerable<Child> GetAllChildren()
        {
           var children = santaDb.Children.ToList(); 

            return children;
        }

        public IEnumerable<Delivery> GetDeliveries()
        {
           var deliveries = santaDb.Deliveries.Include(c => c.Child).Include(c => c.Route).ToList(); 

            return deliveries;
        }

        public IEnumerable<Child> GetNaughtyChildren()
        {
            var children = santaDb.Children.ToList();
            var naughtyChildren = children.Where(c => !c.IsNice);

            return naughtyChildren;
        }

        public IEnumerable<Delivery> GetFailureDeliveries()
        {
            var deliveries = santaDb.Deliveries.ToList().Where(d => d.Status == DeliveryStatus.Failed);

            return deliveries;
        }

        public IEnumerable<Wish> GetWishes()
        {
            var wishes = new List<Wish>();
            var result = santaDb.Wishes.ToList();

            return result;
        }

        public IEnumerable<Wish> GetWishlistByChildId(Guid childId)
        {
            return santaDb.Wishes
                .Where(w => w.ChildId == childId)
                .ToList();
        }

        public IEnumerable<Wish> GetWishlistByChildIdOrderedByPriority(Guid childId)
        {
            return santaDb.Wishes
                .Where(w => w.ChildId == childId)
                .OrderByDescending(w => w.Priority)
                .ToList();
        }

        public IEnumerable<Reindeer> GetAllReindeers()
        {
            return santaDb.Reindeers.ToList();
        }

        public Reindeer GetReindeerById(Guid id)
        {
            return santaDb.Reindeers.Find(id)
                   ?? throw new KeyNotFoundException($"Reindeer with ID {id} not found.");
        }

        public void AddReindeer(Reindeer reindeer)
        {
            santaDb.Reindeers.Add(reindeer);
            santaDb.SaveChanges();
        }

        public void AssignReindeerToDelivery(Guid deliveryId, Guid reindeerId)
        {
            var delivery = santaDb.Deliveries.Find(deliveryId);
            if (delivery == null)
                throw new KeyNotFoundException($"Delivery with ID {deliveryId} not found.");

            var reindeer = santaDb.Reindeers.Find(reindeerId);
            if (reindeer == null)
                throw new KeyNotFoundException($"Reindeer with ID {reindeerId} not found.");

            delivery.ReindeerId = reindeerId;
            santaDb.SaveChanges();
        }
    }
}
