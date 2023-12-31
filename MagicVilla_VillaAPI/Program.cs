// 27, Custom logging
//using MagicVilla_VillaAPI.Logging;
// 26, Serilog
//using Serilog;

// 24, Logger is already registered inside the CreateBuilder.
// 24, Some setting can be seen in appsettings.json

using MagicVilla_VillaAPI;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//// 26, Log from serilog
//// 26, Create logger from serilog: minimumlevel to be logged, write to? file or console, if file what is the interval of changing log file? day, year, infinite,...?, then create the logger
//Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File("Log/villaLogs.txt", rollingInterval: RollingInterval.Infinite).CreateLogger();

//// 26, Tell the application to use the serilog logging configuration rather than built in console logging.
//// 26, Advantage of DI, change implementation without changing inside the controller.
//builder.Host.UseSerilog();

// Add services to the container.
// 30, add service
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    // 30, UseSqlServer
    // 30, GetConnectionString("name") = GetSection("connectionString")[name]
    // 30, This will pass connectionString to ApplicationDbContext. Need to pass cS to DbContext inside AppDbContext.
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();

builder.Services.AddAutoMapper(typeof(MappingConfig));

// 43, Add authentication using Bearer
var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");
builder.Services.AddAuthentication(option =>
{
    // No need?
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    // Configure JwtBearer
    .AddJwtBearer(option =>
    {
        option.RequireHttpsMetadata = false;
        option.SaveToken = true;
        // Info 
        option.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            // Secret
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            // Validate Issuer and Audience with the URLs so that only they are able to access the API 
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddControllers(option =>
{
    // 23, API has default type of Json. GET req. has default accept for Json. Set following property to turn on Acccept type check.
    // 23, IF API does not return the correct Accept type format, it will return 406NotAcceptable.
    //option.ReturnHttpNotAcceptable = true;    // 25, Disable to use text format in Swagger.
}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();    // 23, Add XML format for response.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
// 44, Adding authentication in Swagger
builder.Services.AddSwaggerGen(options => {
    // 44, AddSecurityDefinition() describes how the API is protected through the generated Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
            "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
            "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    // 44, AddSecurityRequirement() OpenApiSecurityRequirement()
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                // 44, open authorization 2
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            // 44, Pass a list of empty strings because some value are passed as some key value pair (OpenApiSecurityRequirement inherit a dictionary)
            new List<string>()
        }
    });
});

// 27, Custom Logging by DI.
// 27, Singleton - Create 1 time at the start of the application, this object will be used whenever application request the implementation of the interface 
// 27, Scoped - Create new object for every request and provide to where it is requested.
// 27, Transient - Create everytime it is accessed. (eg: 1 request, access 10 times -> 10 new objects are created)
// 27, Cannot resolve the service if it was not added.
//builder.Services.AddSingleton<ILogging, Logging>();
// 27, If the implementation of Logging is changed we just need to change the service to the correct implementation.
//builder.Services.AddSingleton<ILogging, LoggingV2>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 43, add authentication
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
