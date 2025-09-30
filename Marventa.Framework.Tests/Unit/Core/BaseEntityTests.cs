using Marventa.Framework.Core.Entities;
using Xunit;

namespace Marventa.Framework.Tests.Unit.Core;

public class BaseEntityTests
{
    private class TestEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public void BaseEntity_NewInstance_ShouldHaveId()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        Assert.NotEqual(Guid.Empty, entity.Id);
    }

    [Fact]
    public void BaseEntity_NewInstance_ShouldHaveCreatedDate()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        Assert.True(entity.CreatedDate <= DateTime.UtcNow);
        Assert.True(entity.CreatedDate >= DateTime.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public void BaseEntity_NewInstance_ShouldNotBeDeleted()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        Assert.False(entity.IsDeleted);
    }

    [Fact]
    public void BaseEntity_SetUpdatedDate_ShouldUpdateValue()
    {
        // Arrange
        var entity = new TestEntity();
        var originalUpdatedDate = entity.UpdatedDate;

        // Act
        entity.UpdatedDate = DateTime.UtcNow;

        // Assert
        Assert.True(entity.UpdatedDate > originalUpdatedDate);
    }

    [Fact]
    public void BaseEntity_TwoEntities_WithSameId_ShouldBeEqual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity { Id = id, Name = "Test1" };
        var entity2 = new TestEntity { Id = id, Name = "Test2" };

        // Act & Assert
        Assert.Equal(entity1, entity2);
    }
}