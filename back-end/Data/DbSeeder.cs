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
                var admin = new User { Name = "admin", Username = "admin", Password = "123" };
                var user = new User { Name = "JonasJ", Username = "JonasJ", Password = "123" };
                var user2 = new User { Name = "NewFriend", Username = "NewFriend", Password = "123" };
                if (!db.Users.Any())
                {

                    db.Users.AddRange(
                        admin,
                        user,
                        user2
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
                    

                    var t2 = new Transaction
                    {
                        Description = $"Coffee for {group.Name}",
                        Date = DateTime.Now,
                        Amount = 100,
                        SType = SplitType.Equal,
                        SplitValues = [50, 50],
                        PayerId = user.Id,
                        Payer = user,
                        GroupId = group.Id,
                        Group = group
                    };

                    

                    var dt1 = new DebtTracker
                    {
                        FromUserId = user.Id,
                        ToUserId = admin.Id,
                        FromUser = user,
                        ToUser = admin,
                        Amount = 50,
                        GroupId = group.Id,
                        Group = group
                    };


                    db.Transactions.AddRange(t2);
                    db.DebtTrackers.AddRange(dt1);
                }
                db.SaveChanges();

            }
        }
    }
}