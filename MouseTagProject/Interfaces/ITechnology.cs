using MouseTagProject.Models;

namespace MouseTagProject.Interfaces
{
    public interface ITechnology
    {
        List<Technology> GetTechnologies();
        Technology GetTechnology(int id);
        void AddTechnology(Technology technolog);
        void UpdateTechnology(int id, Technology technology);
        void RemoveTechnology(int id);
    }
}
