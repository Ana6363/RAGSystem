using Infrastructure.Mongo;
using Infrastructure.Qdrant;
using Infrastructure.OpenAI;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoSettings"));

builder.Services.AddSingleton<MongoDbContext>();


builder.Services.Configure<QdrantSettings>(
    builder.Configuration.GetSection("QdrantSettings"));

builder.Services.AddSingleton<QdrantClientProvider>();


builder.Services.Configure<OpenAISettings>(
    builder.Configuration.GetSection("OpenAIsettings"));

builder.Services.AddSingleton<OpenAIService>();

builder.Services.AddHttpClient(); 


// builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();