using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BlogApi.models.Repository;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Identity.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
