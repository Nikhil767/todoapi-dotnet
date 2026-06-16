using System.Threading.RateLimiting;
using TodoApi.Middleware;
using TodoApi.ServiceInterface;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITodoService, TodoService>();

// Add services to the container.
builder.Services.AddControllers();

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

// set request size at Kestrel level 
builder.WebHost.ConfigureKestrel(options =>
{
	options.Limits.MaxRequestBodySize = 3 * 1024 * 1024;
});
var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();

app.Run();
