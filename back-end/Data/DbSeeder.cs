using Microsoft.EntityFrameworkCore;
using back_end.Data;
using back_end.Models;
using Microsoft.VisualBasic;


namespace back_end.Data
{
    public static class DbSeeder
    {
        public static void Seed(WebApplication app)
        {


            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var admin = new User { Name = "Aigaras Vaitkus", Username = "admin", Password = "123" };
                if (!db.Users.Any())
                {
                    
                    db.Users.AddRange(
                        admin
                    );
                    db.SaveChanges();
                }

                if (!db.Groups.Any())
                {
                    db.Groups.AddRange(
                        new Group { Name = "BestGroup", Members = [admin] },
                        new Group { Name = "BookClub", Members = [admin] }
                    );
                    db.SaveChanges();
                }
                
            }
        }
    }
}