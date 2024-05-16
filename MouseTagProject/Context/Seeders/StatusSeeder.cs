using MouseTagProject.Models;

namespace MouseTagProject.Context.Seeders
{
    public class StatusSeeder
    {
        public static async Task Seed(MouseTagProjectContext context)
        {
            List<Status> defaultStatuses = new List<Status>();
            defaultStatuses.Add(new Status() { Value = "Atviras pasiūlymams" });
            defaultStatuses.Add(new Status() { Value = "Įdarbintas" });
            defaultStatuses.Add(new Status() { Value = "Atmestas" });
            defaultStatuses.Add(new Status() { Value = "Kitas" });
            if (context.Statuses.Any(s => s.Id == 1)) return;
            await context.Statuses.AddRangeAsync(defaultStatuses);
            await context.SaveChangesAsync();
        }
    }
}
