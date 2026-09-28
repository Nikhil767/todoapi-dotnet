using Microsoft.AspNetCore.Diagnostics;
using System.Threading.RateLimiting;
using TodoApi.Middleware;
using TodoApi.ServiceInterface;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISingletonService, SingletonService>();
builder.Services.AddScoped<IScopedService, ScopedService>();
builder.Services.AddTransient<ITransientService, TransientService>();

builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<ITesting, Testing>();
//builder.Services.AddScoped<ITesting, Testing2>();
builder.Services.AddKeyedScoped<ITest, TestA>("testA");
builder.Services.AddKeyedScoped<ITest, TestB>("testB");
builder.Services.AddKeyedScoped<ITest, TestC>("testC");


// Add services to the container.
builder.Services.AddControllers()
.AddXmlSerializerFormatters();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// added global rate limiter
builder.Services.AddRateLimiter(options =>
{
	options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
	options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
	{
		var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
		options.OnRejected = (context, token) =>
		{
			context.HttpContext.Response.Headers["Retry-After"] = "60";
			return ValueTask.CompletedTask;
		};
		return RateLimitPartition.GetTokenBucketLimiter(
			partitionKey: ip,
			factory: _ => new TokenBucketRateLimiterOptions
			{
				TokenLimit = 20,                     // Max tokens
				TokensPerPeriod = 20,                // Refill amount
				ReplenishmentPeriod = TimeSpan.FromMinutes(1),
				QueueLimit = 0,
				AutoReplenishment = true
			});
	});
});

/// 3) Factory-Based Middleware
builder.Services.AddScoped<FactoryExceptionMiddleware>();

// set request size at Kestrel level 
builder.WebHost.ConfigureKestrel(options =>
{
	options.Limits.MaxRequestBodySize = 3 * 1024 * 1024;
});


// 2. Registration in Program.cs For (Global Exception Hanlde Middleware)
//builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
//builder.Services.AddProblemDetails(); // Integrates RFC 7807 ProblemDetails

var app = builder.Build();

// 3. Activates registered IExceptionHandler implementations For (Global Exception Hanlde Middleware)
// app.UseExceptionHandler();

/// Inline Exception Handling Middleware
app.UseExceptionHandler(errorApp =>
{
	errorApp.Run(async context =>
	{
		var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
		var exception = exceptionHandlerFeature?.Error;
		context.Response.ContentType = "application/json";
		context.Response.StatusCode = 500;
		var problem = new
		{
			error = "ServerError",
			message = exception?.Message ?? "An unexpected error occurred."
		};
		await context.Response.WriteAsJsonAsync(problem);
	});
});

/// 2) Convention-based (Classic) Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthorization();

/// 1) Inline / Lambda Middleware
app.Use(async (context, next) =>
{
	// Pre-processing
	await next(context);
	// Post-processing
});

/// 3) Factory-Based Middleware
app.UseMiddleware<FactoryExceptionMiddleware>();

app.MapControllers();

app.Run();
