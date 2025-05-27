using back_end.Models;
using back_end.Helper;

namespace Backend.Tests;

public class BalanceCalculatorTests
{
    [Fact]
    public void Personal_ThrowsArgumentNullException_IfAnyArgumentIsNull()
    {
        Group? nullGroup = null;
        int? userId = 1;
        int? otherId = 2;

        Assert.Throws<ArgumentNullException>(() => BalanceCalculator.Personal(nullGroup, userId, otherId));
        Assert.Throws<ArgumentNullException>(() => BalanceCalculator.Personal(new Group(), null, otherId));
        Assert.Throws<ArgumentNullException>(() => BalanceCalculator.Personal(new Group(), userId, null));
    }

    [Fact]
    public void Personal_ReturnsZero_IfNoDebtTrackers()
    {
        var group = new Group
        {
            DebtTrackers = new List<DebtTracker>()
        };

        var result = BalanceCalculator.Personal(group, 1, 2);

        Assert.Equal(0m, result);
    }

    [Fact]
    public void Personal_ReturnsCorrectNetAmount_WhenDebtTrackersExist()
    {
        var group = new Group
        {
            DebtTrackers = new List<DebtTracker>
            {
                new DebtTracker { FromUserId = 1, ToUserId = 2, Amount = 50m },   // user 1 is owed 50 by 2
                new DebtTracker { FromUserId = 2, ToUserId = 1, Amount = 20m },   // user 1 owes 20 to 2
                new DebtTracker { FromUserId = 1, ToUserId = 3, Amount = 100m }   // irrelevant to user 2
            }
        };

        var result = BalanceCalculator.Personal(group, 1, 2);

        // Net = 50 - 20 = 30
        Assert.Equal(30m, result);
    }

    [Fact]
    public void Personal_ReturnsNegativeNetAmount_WhenUserOwesMore()
    {
        var group = new Group
        {
            DebtTrackers = new List<DebtTracker>
            {
                new DebtTracker { FromUserId = 1, ToUserId = 2, Amount = 10m },   // user 1 is owed 10 by 2
                new DebtTracker { FromUserId = 2, ToUserId = 1, Amount = 40m }    // user 1 owes 40 to 2
            }
        };

        var result = BalanceCalculator.Personal(group, 1, 2);

        // Net = 10 - 40 = -30
        Assert.Equal(-30m, result);
    }

     [Fact]
    public void Total_ReturnsZero_IfGroupHasNoMembers()
    {
        var group = new Group
        {
            Members = new List<User>()
        };

        var result = BalanceCalculator.Total(group, 1);

        Assert.Equal(0m, result);
    }

    [Fact]
    public void Total_ReturnsZero_IfUserIsOnlyMember()
    {
        var group = new Group
        {
            Members = new List<User> { new User { Id = 1 } }
        };

        var result = BalanceCalculator.Total(group, 1);

        Assert.Equal(0m, result);
    }

    [Fact]
    public void Total_CallsPersonalAndSumsResults()
    {
        var group = new Group
        {
            Members = new List<User>
            {
                new User { Id = 1 },
                new User { Id = 2 },
                new User { Id = 3 }
            },
            DebtTrackers = new List<DebtTracker>
            {
                new DebtTracker { FromUserId = 1, ToUserId = 2, Amount = 50m },
                new DebtTracker { FromUserId = 2, ToUserId = 1, Amount = 20m },
                new DebtTracker { FromUserId = 3, ToUserId = 1, Amount = 10m },
                new DebtTracker { FromUserId = 1, ToUserId = 3, Amount = 15m }
            }
        };

        // Personal(1, 2) = 50 - 20 = 30
        // Personal(1, 3) = 15 - 10 = 5
        // Total = 30 + 5 = 35

        var result = BalanceCalculator.Total(group, 1);

        Assert.Equal(35m, result);
    }
}