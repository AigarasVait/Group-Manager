using back_end.Models;
using back_end.Data;
using back_end.Helper;
using Microsoft.EntityFrameworkCore;

namespace Backend.Tests;

public class TransactionSplitterTests

{
    private static Group CreateGroupWithMembers(params User[] users)
    {
        return new Group
        {
            Id = 1,
            Members = users.ToList(),
            DebtTrackers = new List<DebtTracker>()
        };
    }

    private AppDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Split_EqualSplit_AddsCorrectDebts()
    {
        // Arrange
        var payer = new User { Id = 1 };
        var u2 = new User { Id = 2 };
        var u3 = new User { Id = 3 };
        var group = CreateGroupWithMembers(payer, u2, u3);

        var db = CreateInMemoryDb();
        db.Groups.Add(group);
        db.Users.AddRange(payer, u2, u3);
        await db.SaveChangesAsync();

        var transaction = new Transaction
        {
            Amount = 90,
            SType = SplitType.Equal
        };

        // Act
        await TransactionSplitter.Split(transaction, group, payer, db);

        // Assert
        var tracker1 = group.DebtTrackers.FirstOrDefault(dt => dt.ToUserId == u2.Id);
        var tracker2 = group.DebtTrackers.FirstOrDefault(dt => dt.ToUserId == u3.Id);

        Assert.Equal(30, tracker1?.Amount);
        Assert.Equal(30, tracker2?.Amount);
    }

    [Fact]
    public async Task Split_PercentageSplit_UsesSplitValues()
    {
        var payer = new User { Id = 1 };
        var u2 = new User { Id = 2 };
        var group = CreateGroupWithMembers(payer, u2);

        var db = CreateInMemoryDb();

        var t = new Transaction
        {
            Amount = 100,
            SType = SplitType.Percentage,
            SplitValues = new decimal[] { 60, 40 }
        };

        await TransactionSplitter.Split(t, group, payer, db);

        var tracker = group.DebtTrackers.FirstOrDefault(dt => dt.ToUserId == u2.Id);
        Assert.Equal(40, tracker?.Amount);
    }

    [Fact]
    public async Task Split_DynamicSplit_UsesExactAmounts()
    {
        var payer = new User { Id = 1 };
        var u2 = new User { Id = 2 };
        var group = CreateGroupWithMembers(payer, u2);

        var db = CreateInMemoryDb();

        var t = new Transaction
        {
            Amount = 100,
            SType = SplitType.Dynamic,
            SplitValues = new decimal[] { 70, 30 }
        };

        await TransactionSplitter.Split(t, group, payer, db);

        var tracker = group.DebtTrackers.FirstOrDefault(dt => dt.ToUserId == u2.Id);
        Assert.Equal(30, tracker?.Amount);
    }

    [Fact]
    public async Task Split_InvalidSplitValues_DoesNothing()
    {
        var payer = new User { Id = 1 };
        var u2 = new User { Id = 2 };
        var group = CreateGroupWithMembers(payer, u2);

        var db = CreateInMemoryDb();

        var t = new Transaction
        {
            Amount = 100,
            SType = SplitType.Percentage,
            SplitValues = null // invalid
        };

        await TransactionSplitter.Split(t, group, payer, db);

        Assert.Empty(group.DebtTrackers);
    }

    [Fact]
    public async Task Split_SkipsPayer()
    {
        var u1 = new User { Id = 1 };
        var u2 = new User { Id = 2 };
        var u3 = new User { Id = 3 };
        var group = CreateGroupWithMembers(u1, u2, u3);

        var db = CreateInMemoryDb();

        var t = new Transaction
        {
            Amount = 90,
            SType = SplitType.Equal
        };

        await TransactionSplitter.Split(t, group, u3, db);

        // Should only update u1 and u2's debts toward u3
        Assert.Equal(2, group.DebtTrackers.Count);
        Assert.All(group.DebtTrackers, dt => Assert.Equal(u3.Id, dt.FromUserId));
    }
}

