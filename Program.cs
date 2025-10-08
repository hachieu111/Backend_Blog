using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BlogApp.Repository;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Identity.Client;
using BlogApp.Entity;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Add Authorize button
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Your API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Nhập 'Bearer' + [khoảng trắng] + token của bạn.\n\nVí dụ: Bearer abc123xyz"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

// Đọc connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Thêm DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity Setup 
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => //Identity with User and Role
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
.AddEntityFrameworkStores<AppDbContext>() // Save user/role to DB through this DbContext (AppDbContext)
.AddDefaultTokenProviders(); // sercurity token


// Cấu hình JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // way to authentication a user -> use jwt bearer authentication
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // way to respone to user when not authentication -> 401, 403,...
})
.AddJwtBearer(options => //details
{
    options.TokenValidationParameters = new TokenValidationParameters // a validate token
    {
        ValidateIssuer = true, // who...
        ValidateAudience = true, // who use???
        ValidateLifetime = true, // expire
        ValidateIssuerSigningKey = true, // check issuer sign -> is right? or fake
        ValidIssuer = builder.Configuration["Jwt:Issuer"], // validIssuer = validIssuer from appsetting.json
        ValidAudience = builder.Configuration["Jwt:Audience"], // same like above
        IssuerSigningKey = new SymmetricSecurityKey( // secret key
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication(); 
app.UseAuthorization(); 
app.MapControllers();

// Seed Roles (Admin, User)
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role)) // if role not exist? 
            await roleManager.CreateAsync(new IdentityRole(role)); //create new role 
    }
}

app.Run();
//test
