using Microsoft.EntityFrameworkCore;
using back_end.Data;
using back_end.Models;


namespace back_end.Data
{
    public static class DbSeeder
    {
        public static void Seed(WebApplication app)
        {
            

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (!db.Groups.Any())
                {
                    db.Groups.AddRange(
                        new Group { Name = "BestGroup" },
                        new Group { Name = "BookClub" }
                    );
                    db.SaveChanges();
                }
            }
        }
    }
}