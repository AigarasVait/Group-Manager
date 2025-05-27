using back_end.Data;
using back_end.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ControllerTests
{

    public class UsersControllerTests
    {
        private AppDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
                .Options;
            return new AppDbContext(options);
        }

        private IMapper CreateRealMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            config.AssertConfigurationIsValid();

            return config.CreateMapper();
        }

        [Fact]
        public async Task ValidateUser_ValidCredentials_ReturnsUserDto()
        {
            var db = CreateInMemoryDb();
            var user = new User { Username = "test", Password = "pass", Name = "Test" };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var mapper = CreateRealMapper();
            var controller = new UsersController(db, mapper);

            var result = await controller.ValidateUser(new User { Username = "test", Password = "pass" });

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal("Test", dto.Name);
        }

        [Fact]
        public async Task ValidateUser_InvalidCredentials_ReturnsUnauthorized()
        {
            var db = CreateInMemoryDb();
            db.Users.Add(new User { Username = "user", Password = "right" });
            await db.SaveChangesAsync();

            var mapper = CreateRealMapper();
            var controller = new UsersController(db, mapper);

            var result = await controller.ValidateUser(new User { Username = "user", Password = "wrong" });

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid username or password.", unauthorized.Value);
        }

        [Fact]
        public async Task CreateUser_ValidNewUser_ReturnsCreated()
        {
            var db = CreateInMemoryDb();
            var mapper = CreateRealMapper();
            var controller = new UsersController(db, mapper);

            var result = await controller.CreateUser(new User
            {
                Username = "newUser",
                Password = "1234"
            });

            var created = Assert.IsType<CreatedResult>(result);
            var dto = Assert.IsType<UserDto>(created.Value);
            Assert.Equal("newUser", dto.Name);
        }

        [Fact]
        public async Task CreateUser_UsernameAlreadyTaken_ReturnsBadRequest()
        {
            var db = CreateInMemoryDb();
            db.Users.Add(new User { Username = "existing" });
            await db.SaveChangesAsync();

            var mapper = CreateRealMapper();
            var controller = new UsersController(db, mapper);

            var result = await controller.CreateUser(new User
            {
                Username = "existing",
                Password = "pass"
            });

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username is already taken.", badRequest.Value);
        }

        [Fact]
        public async Task CreateUser_EmptyName_DefaultsToUsername()
        {
            var db = CreateInMemoryDb();
            var mapper = CreateRealMapper();
            var controller = new UsersController(db, mapper);

            var result = await controller.CreateUser(new User
            {
                Username = "autoNamed",
                Password = "pass",
                Name = "" // explicitly empty
            });

            var created = Assert.IsType<CreatedResult>(result);
            var dto = Assert.IsType<UserDto>(created.Value);
            Assert.Equal("autoNamed", dto.Name);
        }
    }
}