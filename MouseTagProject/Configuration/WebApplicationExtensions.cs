using Microsoft.EntityFrameworkCore;
using MouseTagProject.Context;

namespace MouseTagProject.Configuration;

public static class WebApplicationExtensions
{
    public static WebApplication ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MouseTagProjectContext>();
        db.Database.Migrate();
        return app;
    }
}