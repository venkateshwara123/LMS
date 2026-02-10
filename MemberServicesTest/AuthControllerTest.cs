using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using MemberServices.Data;
using MemberServices.Models;
using MemberServices.Interfaces;

namespace MemberServicesTest
{
    public class AuthControllerTest
    {
        private static AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task Register_AddsUserAndReturnsOk()
        {
            // Arrange
            await using var context = CreateInMemoryContext();
            var jwtMock = new Mock<IJwtService>();
            var controller = new AuthController(context, jwtMock.Object);

            var user = new User
            {
                Username = "testuser",
                PasswordHash = "hash",
                Role = "Member"
            };

            // Act
            var result = await controller.Register(user);

            // Assert - response
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User Successfully Registered", okResult.Value);

            // Assert - persisted
            var persisted = await context.User.FirstOrDefaultAsync(u => u.Username == "testuser");
            Assert.NotNull(persisted);
            Assert.Equal("hash", persisted.PasswordHash);
            Assert.Equal("Member", persisted.Role);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            await using var context = CreateInMemoryContext();
            var seededUser = new User
            {
                Username = "loginuser",
                PasswordHash = "pw",
                Role = "Member"
            };
            context.User.Add(seededUser);
            await context.SaveChangesAsync();

            var jwtMock = new Mock<IJwtService>();
            jwtMock.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("token123");

            var controller = new AuthController(context, jwtMock.Object);

            var request = new User
            {
                Username = "loginuser",
                PasswordHash = "pw"
            };

            // Act
            var result = await controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            // extract anonymous object's token property via reflection
            var tokenProp = okResult.Value.GetType().GetProperty("token", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            Assert.NotNull(tokenProp);
            var tokenValue = tokenProp.GetValue(okResult.Value) as string;
            Assert.Equal("token123", tokenValue);

            // ensure GenerateToken was called with the persisted user
            jwtMock.Verify(j => j.GenerateToken(It.Is<User>(u => u.Username == "loginuser")), Times.Once);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            await using var context = CreateInMemoryContext();
            var jwtMock = new Mock<IJwtService>();
            var controller = new AuthController(context, jwtMock.Object);

            var request = new User
            {
                Username = "nouser",
                PasswordHash = "nopw"
            };

            // Act
            var result = await controller.Login(request);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);

            // ensure GenerateToken was never called
            jwtMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
        }
    }
}