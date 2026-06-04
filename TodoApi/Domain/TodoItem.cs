namespace TodoApi.Domain
{
	public record TodoItem(long Id, string Title, bool IsCompleted);
	public record CreateTodoRequest(string Title);
	public record UpdateTodoRequest(string Title, bool IsCompleted);
}
