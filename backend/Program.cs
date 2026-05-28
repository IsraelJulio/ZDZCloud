using Microsoft.EntityFrameworkCore;
using backend.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://zdz-cloud.vercel.app", "https://zdz-cloud.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
