using Cinvidad.TechnicalTest.Data.Entities;
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
            var deliveries = santaDb.Deliveries.ToList();

            return deliveries;
        }

        public IEnumerable<Wish> GetWishes()
        {
            var wishes = new List<Wish>();
            var result = santaDb.Wishes.ToList();

            return wishes;
        }
    }
}
