using TodoApi.Domain;

namespace TodoApi.ServiceInterface
{
	public interface ITodoService
	{
		Task<IEnumerable<TodoItem>> GetAll();
		Task<TodoItem> Get(long id);
		Task<TodoItem> CreateTodo(TodoItem todoItem);
		Task<TodoItem> UpdateTodo(long id, TodoItem todoItem);
		Task<bool> Delete(long id);
		Task<bool> DeleteAll();
	}
}
