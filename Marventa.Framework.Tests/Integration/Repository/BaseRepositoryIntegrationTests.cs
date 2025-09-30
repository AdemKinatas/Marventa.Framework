using Marventa.Framework.Core.Entities;
using Marventa.Framework.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Marventa.Framework.Tests.Integration.Repository;

public class BaseRepositoryIntegrationTests : IDisposable
{
    private class TestEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    private class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        public DbSet<TestEntity> TestEntities { get; set; }
    }

    private readonly TestDbContext _context;
    private readonly BaseRepository<TestEntity> _repository;

    public BaseRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _repository = new BaseRepository<TestEntity>(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntityToDatabase()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test", Value = 42 };

        // Act
        await _repository.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(entity.Id);
        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        await _repository.AddAsync(new TestEntity { Name = "Test1", Value = 1 });
        await _repository.AddAsync(new TestEntity { Name = "Test2", Value = 2 });
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, results.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntity()
    {
        // Arrange
        var entity = new TestEntity { Name = "Original", Value = 1 };
        await _repository.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        entity.Name = "Updated";
        entity.Value = 99;
        await _repository.UpdateAsync(entity);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(entity.Id);
        Assert.Equal("Updated", result.Name);
        Assert.Equal(99, result.Value);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteEntity()
    {
        // Arrange
        var entity = new TestEntity { Name = "ToDelete", Value = 1 };
        await _repository.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(entity);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(entity.Id);
        Assert.Null(result); // Soft deleted entities should not be returned
    }

    [Fact]
    public async Task Query_ShouldReturnFilteredResults()
    {
        // Arrange
        await _repository.AddAsync(new TestEntity { Name = "Test1", Value = 10 });
        await _repository.AddAsync(new TestEntity { Name = "Test2", Value = 20 });
        await _repository.AddAsync(new TestEntity { Name = "Test3", Value = 30 });
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.Query()
            .Where(e => e.Value > 15)
            .ToListAsync();

        // Assert
        Assert.Equal(2, results.Count);
        Assert.All(results, e => Assert.True(e.Value > 15));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}