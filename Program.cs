using BankApi;
using BankApi.Implementation;
using BankApi.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddSingleton<IMongoDatabase>(
    sp =>
    {
        var settings =
            sp.GetRequiredService<
                IOptions<MongoSettings>>();

        var client =
            new MongoClient(
                settings.Value.ConnectionString);

        return client.GetDatabase(
            settings.Value.DatabaseName);
    });
builder.Services.AddScoped<
    ICustomerRepository,
    CustomerRepository>();

builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoSettings"));

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(
        "localhost:6379"));

builder.Services.AddScoped<ICustomerRepository,CustomerRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ICustomerCacheService,CustomerCacheService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();