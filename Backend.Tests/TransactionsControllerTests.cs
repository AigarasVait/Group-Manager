using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using back_end.Models;
using back_end.Data;

namespace ControllerTests
{
    public class TransactionsControllerTests
    {
        private AppDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase($"Db_{Guid.NewGuid()}")
                .Options;
            return new AppDbContext(options);
        }

        private IMapper CreateRealMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            config.AssertConfigurationIsValid();
            return config.CreateMapper();
        }

        [Fact]
        public async Task CreateTransaction_ValidRequest_CreatesTransactionAndDebt()
        {
            // Arrange
            var db = CreateInMemoryDb();
            var user1 = new User { Username = "payer" };
            var user2 = new User { Username = "member" };

            var group = new Group
            {
                Name = "TestGroup",
                Members = new List<User> { user1, user2 }
            };

            db.Users.AddRange(user1, user2);
            db.Groups.Add(group);
            await db.SaveChangesAsync();

            var controller = new TransactionsController(db, CreateRealMapper());

            var dto = new TransactionCreateDto
            {
                Amount = 100,
                Description = "Dinner",
                GroupId = group.Id,
                PayerId = user1.Id,
                SType = SplitType.Equal,
                SplitValues = null // not needed for Equal
            };

            // Act
            var result = await controller.CreateTransaction(dto);

            // Assert
            var created = Assert.IsType<CreatedResult>(result);
            var output = Assert.IsType<TransactionDto>(created.Value);
            Assert.Equal(100, output.Amount);
            Assert.Equal("Dinner", output.Description);

            var trackers = await db.DebtTrackers.ToListAsync();
            Assert.Single(trackers); // only user2 owes user1
            Assert.Equal(50, trackers[0].Amount);
        }

        [Fact]
        public async Task CreateTransaction_GroupNotFound_ReturnsBadRequest()
        {
            var db = CreateInMemoryDb();
            var user = new User { Username = "payer" };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var controller = new TransactionsController(db, CreateRealMapper());

            var dto = new TransactionCreateDto
            {
                Amount = 100,
                Description = "Invalid Group",
                GroupId = 999,
                PayerId = user.Id,
                SType = SplitType.Equal
            };

            var result = await controller.CreateTransaction(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Group or payer not found.", badRequest.Value);
        }

        [Fact]
        public async Task CreateTransaction_PayerNotFound_ReturnsBadRequest()
        {
            var db = CreateInMemoryDb();
            var group = new Group { Name = "Test", Members = new List<User>() };
            db.Groups.Add(group);
            await db.SaveChangesAsync();

            var controller = new TransactionsController(db, CreateRealMapper());

            var dto = new TransactionCreateDto
            {
                Amount = 50,
                Description = "Test Txn",
                GroupId = group.Id,
                PayerId = 999,
                SType = SplitType.Equal
            };

            var result = await controller.CreateTransaction(dto);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Group or payer not found.", badRequest.Value);
        }

        [Fact]
        public async Task CreateTransaction_NullRequest_ReturnsBadRequest()
        {
            var db = CreateInMemoryDb();
            var controller = new TransactionsController(db, CreateRealMapper());

            var result = await controller.CreateTransaction(null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid transaction data.", badRequest.Value);
        }
    }
}
