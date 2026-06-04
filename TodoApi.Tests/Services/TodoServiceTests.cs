using Bogus;
using TodoApi.Domain;
using TodoApi.Services;

namespace TodoApi.Tests.Services
{
	public class TodoServiceTests
	{
		private readonly TodoService _service = new();

		[Fact]
		public async Task GetAll_ShouldReturnInitialSeedItems1()
		{
			// Act
			var items = await _service.GetAll();

			// Assert
			Assert.NotNull(items);
			Assert.True(items.Any());
			Assert.Equal(2, items.Count());
		}

		private static readonly Faker<TodoItem> TodoFaker = new Faker<TodoItem>()
			.CustomInstantiator(f => new TodoItem(
				Id: 0,
				Title: f.Lorem.Sentence(3),
				IsCompleted: f.Random.Bool()));

		private TodoService CreateService()
		{
			return new TodoService();
		}

		[Fact]
		public async Task GetAll_ShouldReturnInitialSeedItems()
		{
			var service = CreateService();

			var result = await service.GetAll();

			Assert.NotNull(result);
			Assert.Equal(2, result.Count());
			Assert.Contains(result, x => x.Id == 1 && x.Title == "Learn GitHub Actions");
			Assert.Contains(result, x => x.Id == 2 && x.Title == "Learn Docker");
		}

		[Fact]
		public async Task CreateTodo_ShouldAddItem_AndAssignNextId()
		{
			var service = CreateService();
			var input = TodoFaker.Generate();

			var result = await service.CreateTodo(input);
			var allItems = await service.GetAll();

			Assert.NotNull(result);
			Assert.Equal(input.Title, result.Title);
			Assert.False(result.IsCompleted);
			Assert.Equal(3, result.Id);
			Assert.Equal(3, allItems.Count());
			Assert.Contains(allItems, x => x.Id == result.Id && x.Title == result.Title);
		}

		[Fact]
		public async Task Get_ShouldReturnItem_WhenIdExists()
		{
			var service = CreateService();

			var result = await service.Get(1);

			Assert.NotNull(result);
			Assert.Equal(1, result.Id);
			Assert.Equal("Learn GitHub Actions", result.Title);
			Assert.False(result.IsCompleted);
		}

		[Fact]
		public async Task Get_ShouldThrow_WhenIdDoesNotExist()
		{
			var service = CreateService();

			var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.Get(999));

			Assert.Contains("Invalid id:999", ex.Message);
		}

		[Fact]
		public async Task Delete_ShouldRemoveItem_WhenIdExists()
		{
			var service = CreateService();

			var result = await service.Delete(1);
			var allItems = await service.GetAll();

			Assert.True(result);
			Assert.Equal(1, allItems.Count());
			Assert.DoesNotContain(allItems, x => x.Id == 1);
		}

		[Fact]
		public async Task Delete_ShouldThrow_WhenIdDoesNotExist()
		{
			var service = CreateService();

			var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.Delete(999));

			Assert.Contains("Invalid id:999", ex.Message);
		}

		[Fact]
		public async Task DeleteAll_ShouldClearAllItems()
		{
			var service = CreateService();

			var result = await service.DeleteAll();
			var allItems = await service.GetAll();

			Assert.True(result);
			Assert.Empty(allItems);
		}

		[Fact]
		public async Task UpdateTodo_ShouldUpdateExistingItem()
		{
			var service = CreateService();
			var input = new TodoItem(0, "Updated title", true);

			var result = await service.UpdateTodo(1, input);
			var updated = await service.Get(1);

			Assert.NotNull(result);
			Assert.Equal(1, result.Id);
			Assert.Equal("Updated title", result.Title);
			Assert.True(result.IsCompleted);

			Assert.Equal("Updated title", updated.Title);
			Assert.True(updated.IsCompleted);
		}

		[Fact]
		public async Task UpdateTodo_ShouldThrow_WhenIdDoesNotExist()
		{
			var service = CreateService();
			var input = TodoFaker.Generate();

			var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateTodo(999, input));

			Assert.Contains("Invalid id:999", ex.Message);
		}

		[Fact]
		public async Task CreateTodo_AfterDeleteAll_ShouldStartIdsFromOneAgain()
		{
			var service = CreateService();

			await service.DeleteAll();
			var created = await service.CreateTodo(new TodoItem(0, "First after clear", false));

			Assert.Equal(1, created.Id);
		}
	}
}
