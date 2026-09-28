using TodoApi.Middleware;

namespace TodoApi.Middleware
{
	/// <summary>
	/// Endpoint filters run around Minimal API endpoint handlers and can catch exceptions thrown within that specific route handler.
	/// </summary>
	public class EndpointExceptionFilter : IEndpointFilter
	{
		public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
		{
			try
			{
				return await next(context);
			}
			catch (InvalidOperationException ex)
			{
				return Results.Problem(detail: ex.Message, statusCode: 400);
			}
		}
	}

//	Example : 
//	// Usage in Minimal API Endpoint
//  app.MapGet("/data", () => { throw new InvalidOperationException("Invalid state");})
//  .AddEndpointFilter<EndpointExceptionFilter>();
}
