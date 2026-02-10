using FluentAssertions;
using MemberServices.Models;
using MemberServices.Repository;
using MemberServicesTest;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class UserRepositoryTests
{
        
    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenNotExists()
    {
        using var context = DbContextHelper.CreateContext(nameof(GetByEmailAsync_ShouldReturnNull_WhenNotExists));
        var repository = new UserRepository(context);

        var user = await repository.GetByEmailAsync("missing@mail.com");

        user.Should().BeNull();
    }
    
    [Fact]
    public async Task UserExistsAsync_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Arrange
        using var context = DbContextHelper.CreateContext(nameof(UserExistsAsync_ShouldReturnFalse_WhenUserDoesNotExist));
        var repository = new UserRepository(context);

        // Act
        var exists = await repository.UserExistsAsync("missing@mail.com");

        // Assert
        exists.Should().BeFalse();
    }
}
