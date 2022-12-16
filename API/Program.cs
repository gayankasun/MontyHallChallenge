
var builder = WebApplication.CreateBuilder(args);

var environmentName = "Development";
string configFileName = string.Format("appsettings.{0}.json", environmentName);


IConfigurationRoot configurationRoot = new ConfigurationBuilder()
    .AddJsonFile(configFileName)
    .Build();


// Add services to the container.
builder.Services.Configure<IConfigurationRoot>(configurationRoot);
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

app.UseCors(configurePolicy =>
{
    configurePolicy.AllowAnyOrigin();
    configurePolicy.AllowAnyMethod();
    configurePolicy.AllowAnyHeader();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
