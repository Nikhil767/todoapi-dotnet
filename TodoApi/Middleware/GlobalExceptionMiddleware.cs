using System.Net;
using System.Text.Json;

namespace TodoApi.Middleware
{
	public class GlobalExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<GlobalExceptionMiddleware> _logger;

		public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (ArgumentException ex)
			{
				_logger.LogError(ex, "User exception occurred.");
				await HandleUserExceptionAsync(context, ex);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unhandled exception occurred.");
				await HandleExceptionAsync(context, ex);
			}
		}

		private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
		{
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

			var response = new
			{
				message = "An unexpected error occurred.",
				detail = ex.Message
			};

			await context.Response.WriteAsync(JsonSerializer.Serialize(response));
		}

		private static async Task HandleUserExceptionAsync(HttpContext context, Exception ex)
		{
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
			var response = new
			{
				message = ex.Message,
				detail = ex.Message
			};
			await context.Response.WriteAsync(JsonSerializer.Serialize(response));
		}
	}
}