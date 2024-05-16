using MouseTagProject.DTOs;

namespace MouseTagProject.Interfaces
{
    public interface ICalendar
    {
        public List<CalendarDto> GetCandidates(string date);
    }
}
