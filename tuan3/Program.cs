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

app.MapControllers();
var appName = builder.Configuration["AppInfo:Name"];
Console.WriteLine($"Ten app: {appName}");
app.Run(); 
