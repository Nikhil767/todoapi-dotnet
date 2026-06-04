using Microsoft.AspNetCore.Mvc;
using TodoApi.Domain;
using TodoApi.ServiceInterface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TodoController : ControllerBase
	{
		private readonly ITodoService _todoService = null;

		public TodoController(ITodoService todoService)
		{
			_todoService = todoService;
		}

		// GET: api/<TodoController>
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			return Ok(await _todoService.GetAll());
		}

		// GET api/<TodoController>/5
		[HttpGet("{id}")]
		public async Task<ActionResult<TodoItem>> Get(long id)
		{
			var item = await _todoService.Get(id);
			return item is null ? BadRequest($"Invalid Id : {id}") : Ok(item);
		}

		// POST api/<TodoController>
		[HttpPost]
		public async Task<ActionResult<TodoItem>> Post([FromBody] TodoItem todoItem)
		{
			return Ok(await _todoService.CreateTodo(todoItem));
		}

		// PUT api/<TodoController>/5
		[HttpPut("{id}")]
		public async Task<ActionResult<TodoItem>> Put(long id, [FromBody] TodoItem todoItem)
		{
			var item = await _todoService.UpdateTodo(id, todoItem);
			return item is null ? BadRequest($"Invalid Id : {id}") : Ok(item);
		}

		// DELETE api/<TodoController>/5
		[HttpDelete("{id}")]
		public async Task<ActionResult<bool>> Delete(long id)
		{
			return Ok(await _todoService.Delete(id));
		}

		// DELETE api/<TodoController>/all
		[HttpDelete("all")]
		public async Task<ActionResult<bool>> DeleteAll()
		{
			return Ok(await _todoService.DeleteAll());
		}
	}
}
