
using DemoSaga.ProductService.Database;
using DemoSaga.ProductService.Kafka;
using DemoSaga.ProductService.Services;
using DemoSaga.Shared;
using Microsoft.EntityFrameworkCore;

namespace DemoSaga.ProductApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddScoped<IProductService, DemoSaga.ProductService.Services.ProductService>();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<ProductDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            .ConfigureWarnings(warnings =>
          warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        });
        builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
        builder.Services.AddHostedService<ProductConsumer>();
        builder.Services.AddHostedService<ProductOutboxPulisher<ProductDbContext>>();
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
    }
}
