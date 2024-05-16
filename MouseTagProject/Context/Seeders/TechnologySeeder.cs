using MouseTagProject.Models;

namespace MouseTagProject.Context.Seeders
{
    public class TechnologySeeder
    {
        public static async Task Seed(MouseTagProjectContext context)
        {
            var technologies = new List<Technology>();

            var net = new Technology { TechnologyName = ".NET" };

            var file = Path.Combine(Directory.GetCurrentDirectory(), "Technologies.txt");

            IEnumerable<string> technologiesFile = File.ReadLines(file);

            foreach (var technology in technologiesFile)
            {
                technologies.Add(new Technology { TechnologyName = technology.ToUpper() });
            }

            if (context.Technologies.Any(t => t.Id == 1)) return;

            context.Technologies.Add(net);
            await context.Technologies.AddRangeAsync(technologies);
            await context.SaveChangesAsync();
        }
    }
}
