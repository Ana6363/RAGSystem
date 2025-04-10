using Infrastructure.Mongo;
using Infrastructure.Qdrant;
using Infrastructure.OpenAI;
using Microsoft.Extensions.Options;
using Domain.Models.Users;
using Infrastructure.Mongo.Users;
using nBanks.Application.Users;
using Domain.Models.Documents;
using Infrastructure.Mongo.Documents;
using nBanks.Application.Documents;
using Domain.Models.ChatHistories;
using Infrastructure.Mongo.ChatHistories;
using nBanks.Application.ChatHistories;


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

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<DocumentService>();

builder.Services.AddScoped<IChatHistoryRepository, ChatHistoryRepository>();
builder.Services.AddScoped<ChatHistoryService>();

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