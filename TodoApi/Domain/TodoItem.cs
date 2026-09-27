namespace TodoApi.Domain
{

	public class TodoItem
	{
		public long Id { get; set; }
		public string Title { get; set; }
		public bool IsCompleted { get; set; }
		// Required parameterless constructor for XmlSerializer
		public TodoItem() { }

		public TodoItem(long Id, string Title, bool IsCompleted) : this()
		{
			this.Id = Id;
			this.Title = Title;
			this.IsCompleted = IsCompleted;
		}
	}
	//public record TodoItem(long Id, string Title, bool IsCompleted)
	//{
	//	// Required parameterless constructor for XmlSerializer
	//	//public TodoItem() : this(0, string.Empty, false) { }
	//}
	public record CreateTodoRequest(string Title);
	public record UpdateTodoRequest(string Title, bool IsCompleted);
}
