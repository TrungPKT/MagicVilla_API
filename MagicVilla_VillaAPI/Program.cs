// 24, Logger is already registered inside the CreateBuilder.
// 24, Some setting can be seen in appsettings.json
var builder = WebApplication.CreateBuilder(args);

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
