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
                var user = new User { Name = "Jonas Jonaitis", Username = "123", Password = "123" };
                if (!db.Users.Any())
                {

                    db.Users.AddRange(
                        admin,
                        user
                    );
                    db.SaveChanges();
                }

                if (!db.Groups.Any())
                {
                    db.Groups.AddRange(
                        new Group { Name = "BestGroup", Members = [admin, user] },
                        new Group { Name = "BookClub", Members = [admin, user] }
                    );

                    db.SaveChanges();
                }

                var groups = db.Groups.Include(g => g.Members).ToList();
                foreach (var group in groups)
                {
                    var t1 = new Transaction
                    {
                        Description = $"Lunch for {group.Name}",
                        Date = DateTime.Now,
                        Amount = 30,
                        SType = SplitType.Equal,
                        SplitValues = new decimal[] { 15, 15 },
                        PayerId = admin.Id,
                        Payer = admin,
                        GroupId = group.Id,
                        Group = group
                    };

                    var t2 = new Transaction
                    {
                        Description = $"Coffee for {group.Name}",
                        Date = DateTime.Now,
                        Amount = 70,
                        SType = SplitType.Equal,
                        SplitValues = new decimal[] { 5, 5 },
                        PayerId = user.Id,
                        Payer = user,
                        GroupId = group.Id,
                        Group = group
                    };

                    db.Transactions.AddRange(t1, t2);
                }
                db.SaveChanges();

            }
        }
    }
}