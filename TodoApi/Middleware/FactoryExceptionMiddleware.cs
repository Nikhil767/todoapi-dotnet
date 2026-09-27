using TodoApi.ServiceInterface;

namespace TodoApi.Middleware
{
	/// <summary>
	/// 3) Factory-Based Middleware
	/// Lifetime: Registered as Scoped or Transient in IServiceCollection
	/// Injecting Services: ALL dependencies (Singleton, Scoped, or Transient) are injected cleanly via the Constructor.
	/// </summary>
	public class FactoryExceptionMiddleware : IMiddleware
	{
		private readonly ISingletonService _singleton;
		private readonly IScopedService _scoped;
		private readonly ITransientService _transient;

		/// <summary>
		/// Injecting Services: ALL dependencies (Singleton, Scoped, or Transient) are injected cleanly via the Constructor.
		/// </summary>
		/// <param name="logger"></param>
		public FactoryExceptionMiddleware(ISingletonService singleton, IScopedService scoped, ITransientService transient
		)
		{
			_singleton = singleton;
			_scoped = scoped;
			_transient = transient;
		}

		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			// Pre-processing
			await next(context);
			// Post-processing
		}
	}
}
