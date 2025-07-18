using PRS.Application;
using PRS.Presentation.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(static options =>
{
    options.AddDefaultPolicy(static policy =>
    {
        // TODO: Make this configurable
        policy
          .WithOrigins("*")
          .AllowAnyHeader()
          .AllowAnyMethod();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(static options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddApplication()
                .AddEndpointsApiExplorer()
                .AddSwaggerGen();

var app = builder.Build();

app.UseCors();

app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
// TODO: For now let's enable the swagger for all environments
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();