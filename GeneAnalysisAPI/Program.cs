using GeneAnalysisAPI.Repositories;
using GeneAnalysisAPI.Services;
using Microsoft.OpenApi.Models;
using Minio;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Gene Analysis API", Version = "v1" });

    // ✅ Manually define file upload in Swagger
    options.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorUI",
        policy => policy
            .AllowAnyOrigin()  // ✅ Allows Blazor WebAssembly to access API
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Register dependencies
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<MinioService>();
builder.Services.AddSingleton<IGeneRepository, GeneRepository>();
builder.Services.AddSingleton<ScraperService>();

// Register MinIO Client
builder.Services.AddSingleton<IMinioClient>(sp =>
    new MinioClient()
        .WithEndpoint(builder.Configuration["MinIO:Endpoint"])
        .WithCredentials(builder.Configuration["MinIO:AccessKey"], builder.Configuration["MinIO:SecretKey"])
        .Build());

var app = builder.Build();
// ✅ Apply CORS policy
app.UseCors("AllowBlazorUI");
// Redirect root URL (/) to Swagger UI
app.MapGet("/", () => Results.Redirect("/swagger/index.html"));

// Configure Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gene Analysis API v1"));
}

// Middleware
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
