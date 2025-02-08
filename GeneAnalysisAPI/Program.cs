using GeneAnalysisAPI.Repositories;
using GeneAnalysisAPI.Services;
using Microsoft.OpenApi.Models;
using Minio;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<MinioService>();
builder.Services.AddSingleton<ScraperService>();
builder.Services.AddSingleton<IGeneRepository, GeneRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Gene Analysis API", Version = "v1" });

});


builder.Services.AddSingleton<IMinioClient>(sp =>
    new MinioClient()
        .WithEndpoint(builder.Configuration["MinIO:Endpoint"])
        .WithCredentials(builder.Configuration["MinIO:AccessKey"], builder.Configuration["MinIO:SecretKey"])
        .Build());

var app = builder.Build();
app.MapGet("/", () => Results.Redirect("/swagger/index.html")); 


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gene Analysis API v1"));
}

var url = "https://localhost:7180/swagger/index.html";
Console.WriteLine($"Swagger running at {url}");
System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
{
    FileName = url,
    UseShellExecute = true
});
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
