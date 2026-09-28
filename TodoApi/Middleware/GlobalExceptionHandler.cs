using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TodoApi.Middleware
{
	/// <summary>
	/// Global Exception Hanlde Middleware
	/// </summary>
	public class GlobalExceptionHandler : IExceptionHandler
	{
		private readonly ILogger<GlobalExceptionHandler> _logger;

		public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
		{
			_logger = logger;
		}

		public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
		{
			_logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

			var problemDetails = new ProblemDetails
			{
				Status = StatusCodes.Status500InternalServerError,
				Title = "Server Error",
				Detail = "An unexpected error occurred."
			};

			httpContext.Response.StatusCode = problemDetails.Status.Value;
			await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

			// Return true to signal that this exception has been handled completely
			return true;
		}
	}
}
