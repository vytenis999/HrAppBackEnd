using MouseTagProject.Context;
using MouseTagProject.Interfaces;
using MouseTagProject.Models;

namespace MouseTagProject.Repository
{
    public class ITechnologyRepository : ITechnology
    {
        private readonly MouseTagProjectContext _context;

        public ITechnologyRepository(MouseTagProjectContext context)
        {
            _context = context;
        }

        public List<Technology> GetTechnologies()
        {
            return _context.Technologies.ToList();
        }

        public Technology GetTechnology(int id)
        {
            return _context.Technologies.FirstOrDefault(t => t.Id == id);
        }

        public void AddTechnology(Technology technology)
        {
            _context.Technologies.Add(technology);
            _context.SaveChanges();
        }

        public void UpdateTechnology(int id, Technology technology)
        {
            var technologyEntity = GetTechnology(technology.Id);

            technologyEntity.TechnologyName = technology.TechnologyName;
            _context.Technologies.Update(technology);
            _context.SaveChanges();
        }

        public void RemoveTechnology(int id)
        {
            var technology = _context.Technologies.FirstOrDefault(x => x.Id == id);
            _context.Technologies.Remove(technology);
            _context.SaveChanges();
        }
    }
}
