using JWTLearningProject.CORE.Configuration;
using JWTLearningProject.CORE.Models;
using JWTLearningProject.CORE.Repositories;
using JWTLearningProject.CORE.Services;
using JWTLearningProject.CORE.UnitOfWork;
using JWTLearningProject.DATA;
using JWTLearningProject.DATA.Repositories;
using JWTLearningProject.SERVICE.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SharedLibrary.Services;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.Configure<CustomTokenOption>(builder.Configuration.GetSection("TokenOption"));


builder.Services.Configure<List<Client>>(builder.Configuration.GetSection("Clients"));

#region DIRegister
builder.Services.AddScoped<IAuthenticationService,AuthenticationService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<ITokenService,TokenService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));

builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("SqLiteConnection"),sqlOptions =>
    {
        sqlOptions.MigrationsAssembly("JWTLearningProject.DATA");
    });
});
builder.Services.AddIdentity<UserApp,IdentityRole>(opt =>
{
    opt.User.RequireUniqueEmail = true;
    opt.Password.RequireNonAlphanumeric = false;

}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,options =>
            {
                var tokenOptions = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOption>();
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience[0],
                    IssuerSigningKey = SignInService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),



                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,

                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
