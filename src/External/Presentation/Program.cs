using Infrastructure.Persistence;
using Microsoft.OpenApi;
using Presentation.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value!.Errors
                        .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? "The input was not valid."
                            : error.ErrorMessage)
                        .ToArray());

            var response = new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                ErrorCode = ErrorCodes.ValidationFailed,
                Message = "The request contains validation errors.",
                Details = "One or more request fields failed validation.",
                TraceId = context.HttpContext.TraceIdentifier,
                Path = context.HttpContext.Request.Path,
                Errors = errors
            };

            return new BadRequestObjectResult(response);
        };
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Add CORS (example configuration)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});


builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<JwtHelper>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var response = new ApiErrorResponse
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    ErrorCode = ErrorCodes.Unauthorized,
                    Message = "Authentication is required to access this resource.",
                    Details = context.ErrorDescription,
                    TraceId = context.HttpContext.TraceIdentifier,
                    Path = context.HttpContext.Request.Path
                };

                return context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                var response = new ApiErrorResponse
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    ErrorCode = ErrorCodes.Forbidden,
                    Message = "You do not have permission to access this resource.",
                    TraceId = context.HttpContext.TraceIdentifier,
                    Path = context.HttpContext.Request.Path
                };

                return context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<AuditInterceptor>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"  // Optional, but good for clarity
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty; // Serve Swagger UI at the root (optional)
});

app.UseGlobalExceptionHandling();
app.UseStaticFiles();
app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});
app.UseStatusCodePages(async statusCodeContext =>
{
    var httpContext = statusCodeContext.HttpContext;

    if (httpContext.Response.HasStarted || httpContext.Response.ContentLength.HasValue)
    {
        return;
    }

    var statusCode = httpContext.Response.StatusCode;
    var response = new ApiErrorResponse
    {
        StatusCode = statusCode,
        ErrorCode = statusCode switch
        {
            StatusCodes.Status401Unauthorized => ErrorCodes.Unauthorized,
            StatusCodes.Status403Forbidden => ErrorCodes.Forbidden,
            StatusCodes.Status404NotFound => ErrorCodes.NotFound,
            _ => ErrorCodes.BadRequest
        },
        Message = statusCode switch
        {
            StatusCodes.Status401Unauthorized => "Authentication is required to access this resource.",
            StatusCodes.Status403Forbidden => "You do not have permission to access this resource.",
            StatusCodes.Status404NotFound => "The requested endpoint was not found.",
            _ => "The request could not be processed."
        },
        TraceId = httpContext.TraceIdentifier,
        Path = httpContext.Request.Path
    };

    httpContext.Response.ContentType = "application/json";
    await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
});

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");

    try
    {
        var placementService = scope.ServiceProvider
            .GetRequiredService<IBannerPlacementService>();

        await placementService.SyncPlacementsWithEnumAsync();
    }
    catch (Exception exception)
    {
        logger.LogError(exception,
            "Banner placement synchronization failed during startup. The application will continue running and the operation can be retried later.");
    }
}


//app.UseHttpsRedirection();

//app.UseAuthorization();

app.UseCors("AllowAll"); // Apply CORS policy
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

//app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
