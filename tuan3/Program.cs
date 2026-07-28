using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers(); 
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

app.UseAuthorization();

app.Use(async (context, next) =>
{
    var watch=Stopwatch.StartNew();
    Console.WriteLine("A vao");
    await next();
    watch.Stop();
    Console.WriteLine($"Request mat tong cong: {watch.ElapsedMilliseconds} ms");
    Console.WriteLine("A sau");

});


app.Use(async (context, next) =>
{
    Console.WriteLine("B vao");
    await next();
    Console.WriteLine("B sau");
});
app.MapControllers();

app.Run(); 
