using Microsoft.EntityFrameworkCore;
using PolarizedJam.Database;
using PolarizedJam.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DatabaseContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DB"));
});
builder.Services.AddCors(opts =>
    opts.AddDefaultPolicy(builder =>
        builder
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(s => true)
            .Build()
    )
);

builder.Services.AddSignalR()
    .AddJsonProtocol();

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        scope.ServiceProvider.GetRequiredService<DatabaseContext>().Database.Migrate();
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<JamHub>(JamHub.HubUrl);
});

app.Run();
