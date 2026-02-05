using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BookServices.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BookDbContext>(options =>
options.UseInMemoryDatabase(builder.Configuration.GetConnectionString("BookDB")));
// Add services to the container.
builder.Services.AddAuthentication("Bearer")
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey123"))
    };
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
