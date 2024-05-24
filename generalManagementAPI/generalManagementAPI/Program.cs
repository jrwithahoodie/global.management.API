using System.Text;
using BusinessLogic.ActivityType;
using BusinessLogic.Role;
using BusinessLogic.User;
using BusinessLogic.UserActivity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

#region Dependencies

builder.Services.AddScoped<IUserBll, UserBll>();
builder.Services.AddScoped<IActivityTypeBll, ActivityTypeBll>();
builder.Services.AddScoped<IUserActivityBll, UserActivityBll>();
builder.Services.AddScoped<IRoleBll, RoleBll>();

#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

#region JwtConfing

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //Key used to sign and validate token
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Secret"])),

            //Validate token emissor
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtConfig:Issuer"],

            //Validate who is token made for
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtConfig:Audience"],

            //Validate the token lifetime
            ValidateLifetime = true
        };
    });

#endregion

#region Swagger config

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "General Management", Version = "v1" });

    // Configuración para la autenticación
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Bearer",
        BearerFormat = "JWT",
        Scheme = "bearer",
        Description = "Inserta tu token JWT aquí",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            securityScheme,
            Array.Empty<string>()
        },
    });

    c.OperationFilter<SwaggerAuthorizeCheckOperationFilter>();
});

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region Authorization

app.UseAuthentication();
app.UseAuthorization();

#endregion

#region Swagger

app.UseSwagger(c =>
{
    c.RouteTemplate = "/swagger/{documentName}/swagger.json";
});

#endregion

app.UseCors(builder =>
    builder.AllowAnyOrigin()
    .WithMethods("GET", "PUT", "POST", "DELETE", "OPTIONS")
    .WithHeaders("Content-Type", "Authorization", "Content-Length", "X-Requested-With", "Origin")
    .WithExposedHeaders("Location"));

app.MapControllers();

app.Run();

public class SwaggerAuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>().Any() ||
            context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

        if (hasAuthorize)
        {
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                }
            };
        }
    }
}