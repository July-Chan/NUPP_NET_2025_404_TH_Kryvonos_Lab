using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PublicTransit.Common.App.Crud;
using Xunit;

namespace PublicTransit.Common.Tests
{
    public class CrudServiceAsyncTests
    {
        private readonly string _testFilePath = "test_crud_data.json";

        private CrudServiceAsync<TestEntity> GetServiceInstance()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
            return new CrudServiceAsync<TestEntity>(_testFilePath);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddEntity()
        {
            var service = GetServiceInstance();
            var entity = new TestEntity("Test1");

            var result = await service.CreateAsync(entity);
            var allEntities = await service.ReadAllAsync();

            Assert.True(result);
            Assert.Single(allEntities);
            Assert.Equal("Test1", allEntities.First().Name);
        }

        [Fact]
        public async Task ReadAsync_ShouldReturnCorrectEntity()
        {
            var service = GetServiceInstance();
            var entity = new TestEntity("Test2");
            await service.CreateAsync(entity);

            var result = await service.ReadAsync(entity.Id);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyEntity()
        {
            var service = GetServiceInstance();
            var entity = new TestEntity("Test3");
            await service.CreateAsync(entity);

            entity.Name = "UpdatedTest3";
            var result = await service.UpdateAsync(entity);
            var updatedEntity = await service.ReadAsync(entity.Id);

            Assert.True(result);
            Assert.NotNull(updatedEntity);
            Assert.Equal("UpdatedTest3", updatedEntity.Name);
        }

        [Fact]
        public async Task RemoveAsync_ShouldDeleteEntity()
        {
            var service = GetServiceInstance();
            var entity = new TestEntity("Test4");
            await service.CreateAsync(entity);

            var result = await service.RemoveAsync(entity);
            var allEntities = await service.ReadAllAsync();
            var removedEntity = await service.ReadAsync(entity.Id);


            Assert.True(result);
            Assert.Empty(allEntities);
            Assert.Null(removedEntity);
        }

        [Fact]
        public async Task SaveAndLoadAsync_ShouldPersistAndRestoreData()
        {
            var service1 = GetServiceInstance();
            await service1.CreateAsync(new TestEntity("SaveTest1"));
            await service1.CreateAsync(new TestEntity("SaveTest2"));

            await service1.SaveAsync();
            var service2 = new CrudServiceAsync<TestEntity>(_testFilePath);
            var allEntities = await service2.ReadAllAsync();

            Assert.Equal(2, allEntities.Count());
        }
        
        [Fact]
        public async Task ReadAllAsync_WithPagination_ShouldReturnCorrectPage()
        {
            var service = GetServiceInstance();
            for (int i = 0; i < 15; i++)
            {
                await service.CreateAsync(new TestEntity($"Entity{i}"));
            }

            var page1 = await service.ReadAllAsync(1, 10);
            var page2 = await service.ReadAllAsync(2, 10);

            Assert.Equal(10, page1.Count());
            Assert.Equal(5, page2.Count());
        }
    }
}
