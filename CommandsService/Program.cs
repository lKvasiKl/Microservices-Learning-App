using CommandsService.AsyncDataServices;
using CommandsService.Data;
using CommandsService.EventProcessing;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt => 
    opt.UseInMemoryDatabase("InMem"));

builder.Services.AddScoped<ICommandRepository, CommandRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();

builder.Services.AddHostedService<MessageBusSubscriber>();

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
