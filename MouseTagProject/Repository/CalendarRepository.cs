using MouseTagProject.Context;
using MouseTagProject.DTOs;
using MouseTagProject.Interfaces;
using MouseTagProject.Models;


namespace MouseTagProject.Repository
{
    public class CalendarRepository : ICalendar
    {
        private readonly MouseTagProjectContext _context;

        public CalendarRepository(MouseTagProjectContext context)
        {
            _context = context;
        }

        public List<CalendarDto> GetCandidates(string date)
        {
            date = date.Replace('_', '/');

            DateTime test = new DateTime();
            test = Convert.ToDateTime(date).AddMonths(-1);

            DateTime test2 = Convert.ToDateTime(date).AddMonths(2);

            Console.WriteLine(test);
            Console.WriteLine(test2);

            var candidates = _context.Candidates
                .Where(c => c.WillBeContacted > (test) && c.WillBeContacted < (test2))
                .ToList()
                .Select(
                candidate => new CalendarDto
                {
                    Name = candidate.Name,
                    Surname = candidate.Surname,
                    WillBeContacted = candidate.WillBeContacted?.ToString("yyyy-MM-dd")
                }).ToList();
            Console.WriteLine(candidates);
            return candidates;
        }
    }
}
