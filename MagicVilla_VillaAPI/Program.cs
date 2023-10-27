// 24, Logger is already registered inside the CreateBuilder.
// 24, Some setting can be seen in appsettings.json
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//// 26, Log from serilog
//// 26, Create logger from serilog: minimumlevel to be logged, write to? file or console, if file what is the interval of changing log file? day, year, infinite,...?, then create the logger
//Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File("Log/villaLogs.txt", rollingInterval: RollingInterval.Infinite).CreateLogger();

//// 26, Tell the application to use the serilog logging configuration rather than built in console logging.
//// 26, Advantage of DI, change implementation without changing inside the controller.
//builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers(option =>
{
    // 23, API has default type of Json. GET req. has default accept for Json. Set following property to turn on Acccept type check.
    // 23, IF API does not return the correct Accept type format, it will return 406NotAcceptable.
    //option.ReturnHttpNotAcceptable = true;    // 25, Disable to use text format in Swagger.
}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();    // 23, Add XML format for response.
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

app.UseAuthorization();

app.MapControllers();

app.Run();
