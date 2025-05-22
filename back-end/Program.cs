using back_end.Data;
using back_end.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("TestDb"));

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Members.AddRange(
        new Member { FirstName = "Jake" },
        new Member { FirstName = "Bob" }
    );
    db.SaveChanges();
}

// Minimal API endpoint
app.MapGet("/members", async (AppDbContext db) =>
    await db.Members.ToListAsync());

app.Run();