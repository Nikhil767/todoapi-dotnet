using System.Net;
using System.Text.Json;
using TodoApi.ServiceInterface;

namespace TodoApi.Middleware
{

	/// <summary>
	/// 2) Convention-based (Classic) Middleware
	/// Injecting Singleton : Pass via Constructor.
	/// Injecting Scoped / Transient Services: Pass directly as parameters to InvokeAsync
	/// </summary>
	public class GlobalExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<GlobalExceptionMiddleware> _logger;
		private readonly ISingletonService _singletonService; // Injected ONCE at startup
		private readonly IServiceScopeFactory _scopeFactory; // Singleton-safe!

		/// <summary>
		/// Injecting Singleton : Pass via Constructor.
		/// </summary>
		/// <param name="next"></param>
		/// <param name="logger"></param>
		public GlobalExceptionMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory, ILogger<GlobalExceptionMiddleware> logger
		, ISingletonService singletonService)
		{
			_next = next;
			_logger = logger;
			_singletonService = singletonService;
			_scopeFactory = scopeFactory;
		}

		/// <summary>
		/// Injecting Scoped / Transient Services: Pass directly as parameters to InvokeAsync
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public async Task InvokeAsync(HttpContext context
		, IScopedService scopedService, ITransientService transientService
		)
		{
			try
			{

				// Explicitly resolve scoped service from the request scope
				var scopedService1 = context.RequestServices.GetRequiredService<IScopedService>();
				var transientService1 = context.RequestServices.GetRequiredService<ITransientService>();

				// Manually open a dedicated scope
				using (var scope = _scopeFactory.CreateScope())
				{
					var scopedService2 = scope.ServiceProvider.GetRequiredService<IScopedService>();
					var transientService2 = scope.ServiceProvider.GetRequiredService<ITransientService>();
				}

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