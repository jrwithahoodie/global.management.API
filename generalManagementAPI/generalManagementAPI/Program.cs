using BusinessLogic.ActivityType;
using BusinessLogic.User;
using BusinessLogic.UserActivity;

var builder = WebApplication.CreateBuilder(args);

#region Dependencies

builder.Services.AddScoped<IUserBll, UserBll>();
builder.Services.AddScoped<IActivityTypeBll, ActivityTypeBll>();
builder.Services.AddScoped<IUserActivityBll, UserActivityBll>();

#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(builder =>
    builder.AllowAnyOrigin()
    .WithMethods("GET", "PUT", "POST", "DELETE", "OPTIONS")
    .WithHeaders("Content-Type", "Authorization", "Content-Length", "X-Requested-With", "Origin")
    .WithExposedHeaders("Location"));

app.MapControllers();

app.Run();