using System.Diagnostics;
using tuan3.Middlewares;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddCors(
    options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
    }
    );
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Student API",
        Version = "v1"
    });
});

var app = builder.Build(); 


if (app.Environment.IsDevelopment()) 
{
    app.UseSwagger(); 
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{

    Console.WriteLine("A vao");
    await next();   
    Console.WriteLine("A sau");

});


app.Use(async (context, next) =>
{
    Console.WriteLine("B vao");
    await next();
    Console.WriteLine("B sau");
});
app.Use(async (context, next) =>
{
    Console.WriteLine("c vao");
    await next();
    Console.WriteLine("c sau");
});
app.UseMiddleware<RequestLoggingMiddleware>();
app.MapControllers();

app.Run(); 
