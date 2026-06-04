using TodoApi.Domain;
using TodoApi.ServiceInterface;

namespace TodoApi.Services
{
	public class TodoService : ITodoService
	{
		// In-memory storage for learning
		private List<TodoItem> todos =
[
	new TodoItem(1, "Learn GitHub Actions", false),
	new TodoItem(2, "Learn Docker", false)
];
		public TodoService(){}

		public async Task<TodoItem> CreateTodo(TodoItem todoItem)
		{
			var newId = todos.Count == 0 ? 1 : todos.Max(t => t.Id) + 1;
			var todo = new TodoItem(newId, todoItem.Title, false);
			todos.Add(todo);
			return await Task.FromResult(todo);
		}

		public async Task<bool> Delete(long id)
		{
			var isDeleted = false;
			var itemToDelete = todos.FirstOrDefault(x => x.Id == id);
			if (itemToDelete is null)
				throw new ArgumentException($"Invalid id:{id}");			 
				isDeleted = todos.Remove(itemToDelete);			
			return await Task.FromResult(isDeleted);
		}

		public async Task<bool> DeleteAll()
		{
			todos.Clear();
			return await Task.FromResult(true);
		}

		public async Task<TodoItem> Get(long id)
		{
			var existing = todos.FirstOrDefault(t => t.Id == id);
			if (existing is null)
				throw new ArgumentException($"Invalid id:{id}");
			return await Task.FromResult(existing);
		}

		public async Task<IEnumerable<TodoItem>> GetAll()
		{
			return await Task.FromResult(todos);
		}

		public async Task<TodoItem> UpdateTodo(long id, TodoItem todoItem)
		{
			var existing = todos.FirstOrDefault(t => t.Id == id);
			if (existing is null)
				throw new ArgumentException($"Invalid id:{id}");
			var updated = existing with
			{
				Title = todoItem.Title,
				IsCompleted = todoItem.IsCompleted
			};
			todos.Remove(existing);
			todos.Add(updated);
			return await Task.FromResult(updated);
		}
	}
}
