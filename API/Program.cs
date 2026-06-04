using System.Reflection;
using Microsoft.OpenApi;
using service_matrix.CommandHandlers;
using service_matrix.Helpers;
using service_matrix.Middleware;
using service_matrix.QueryHandlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddControllers();

// Register core services
builder.Services.AddScoped<IFileHelper, FileHelper>();

// Register command handlers
builder.Services.AddScoped<WordSearchCommandHandler>();
builder.Services.AddScoped<UpdateWordsCommandHandler>();
builder.Services.AddScoped<MergeWordsCommandHandler>();

// Register query handlers
builder.Services.AddScoped<GetWordsQueryHandler>();
builder.Services.AddScoped<LookupWordQueryHandler>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Service Matrix API", Version = "v1" });
      // Include XML comments for Swagger
     var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
     var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
     c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

// Enable CORS
app.UseCors("AllowAllOrigins");

  
app.UseAuthorization();

app.MapControllers();

app.Run();