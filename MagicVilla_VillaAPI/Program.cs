// 27, Custom logging
using MagicVilla_VillaAPI.Logging;
// 26, Serilog
//using Serilog;

// 24, Logger is already registered inside the CreateBuilder.
// 24, Some setting can be seen in appsettings.json

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

// 27, Custom Logging by DI.
// 27, Singleton - Create 1 time at the start of the application, this object will be used whenever application request the implementation of the interface 
// 27, Scoped - Create new object for every request and provide to where it is requested.
// 27, Transient - Create everytime it is accessed. (eg: 1 request, access 10 times -> 10 new objects are created)
// 27, Cannot resolve the service if it was not added.
//builder.Services.AddSingleton<ILogging, Logging>();
// 27, If the implementation of Logging is changed we just need to change the service to the correct implementation.
builder.Services.AddSingleton<ILogging, LoggingV2>();

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
