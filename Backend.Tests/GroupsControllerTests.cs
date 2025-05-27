using Microsoft.EntityFrameworkCore;
using AutoMapper;
using back_end.Data;
using back_end.Models;
using Microsoft.AspNetCore.Mvc;

namespace ControllerTests
{

    public class GroupsControllerTests
    {
        private AppDbContext CreateInMemoryDb()
        {
            var opts = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase($"Db_{Guid.NewGuid()}")
                .Options;
            return new AppDbContext(opts);
        }

        private IMapper CreateRealMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            config.AssertConfigurationIsValid();
            return config.CreateMapper();
        }

        [Fact]
        public async Task GetGroups_ValidUser_ReturnsGroupsWithBalance()
        {
            var db = CreateInMemoryDb();
            var user = new User { Username = "user" };
            var group = new Group
            {
                Name = "TestGroup",
                Members = new List<User> { user },
                DebtTrackers = new List<DebtTracker>()
            };
            db.Users.Add(user);
            db.Groups.Add(group);
            await db.SaveChangesAsync();

            var controller = new GroupsController(db, CreateRealMapper());

            var result = await controller.GetGroups(user.Id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<GroupSimpleDto>>(ok.Value);
            Assert.Single(list);
            Assert.Equal("TestGroup", list[0].Name);
        }

        [Fact]
        public async Task GetGroup_InvalidGroupOrUser_ReturnsNotFound()
        {
            var db = CreateInMemoryDb();
            var controller = new GroupsController(db, CreateRealMapper());

            var result = await controller.GetGroup(1, 1);
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var message = Assert.IsType<string>(notFound.Value);
            Assert.Contains("not found", message.ToLower());
        }

        [Fact]
        public async Task CreateGroup_ValidCreator_CreatesGroup()
        {
            var db = CreateInMemoryDb();
            var user = new User { Username = "creator", Name = "Creator" };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var controller = new GroupsController(db, CreateRealMapper());

            var post = new GroupPostDto { Name = "NewGroup", CreatorId = user.Id };
            var result = await controller.CreateGroup(post);

            var created = Assert.IsType<CreatedResult>(result);
            var group = Assert.IsType<GroupDto>(created.Value);
            Assert.Equal("NewGroup", group.Name);
            Assert.Single(group.Members);
            Assert.Equal("Creator", group.Members[0].Name);
        }

        [Fact]
        public async Task CreateGroup_InvalidCreator_ReturnsBadRequest()
        {
            var db = CreateInMemoryDb();
            var controller = new GroupsController(db, CreateRealMapper());

            var post = new GroupPostDto { Name = "Invalid", CreatorId = 999 };
            var result = await controller.CreateGroup(post);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Creator user not found.", bad.Value);
        }

        [Fact]
        public async Task PatchGroup_AddMember_Works()
        {
            var db = CreateInMemoryDb();
            var group = new Group { Name = "PatchGroup", Members = [] };
            var user = new User { Username = "new" };
            db.Groups.Add(group);
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var controller = new GroupsController(db, CreateRealMapper());

            var patch = new GroupPatchDto { AddMemberUserId = user.Id };
            var result = await controller.PatchGroup(group.Id, patch);

            var ok = Assert.IsType<OkObjectResult>(result);
            var updated = Assert.IsType<Group>(ok.Value);
            Assert.Single(updated.Members);
        }

        [Fact]
        public async Task PatchGroup_RemoveMemberWithDebt_Fails()
        {
            var db = CreateInMemoryDb();
            var user = new User { Username = "debtor" };
            var group = new Group
            {
                Name = "DebtGroup",
                Members = [user],
                DebtTrackers = [new DebtTracker { FromUserId = 1, ToUserId = 2, Amount = 10 }]
            };
            db.Users.Add(user);
            db.Groups.Add(group);
            await db.SaveChangesAsync();

            var controller = new GroupsController(db, CreateRealMapper());

            var patch = new GroupPatchDto { RemoveMemberUserId = user.Id };
            var result = await controller.PatchGroup(group.Id, patch);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cannot remove user with outstanding debts.", bad.Value);
        }

        [Fact]
        public async Task PatchGroup_MarkDebtAsPaid_Works()
        {
            var db = CreateInMemoryDb();
            var group = new Group
            {
                Name = "PayGroup",
                Members = [],
                DebtTrackers = [new DebtTracker { FromUserId = 1, ToUserId = 2, Amount = 50 }]
            };
            db.Groups.Add(group);
            await db.SaveChangesAsync();

            var controller = new GroupsController(db, CreateRealMapper());

            var patch = new GroupPatchDto { PaidMemberUserId = 1, FromMemberUserId = 2 };
            var result = await controller.PatchGroup(group.Id, patch);

            var ok = Assert.IsType<OkObjectResult>(result);
            var updated = Assert.IsType<Group>(ok.Value);
            Assert.Equal(0, updated.DebtTrackers[0].Amount);
        }
    }
}