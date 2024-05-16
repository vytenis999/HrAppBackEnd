using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MouseTagProject.Context;
using MouseTagProject.Context.Seeders;
using MouseTagProject.Interfaces;
using MouseTagProject.Models;
using MouseTagProject.Repository;
using MouseTagProject.Services;
using System.Text;
using MouseTagProject.Configuration;
using ToDoListProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.AddControllers().AddJsonOptions(x =>
//              x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API",
        Version = "v1",
        Description = "MouseTagProject"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    Array.Empty<string>()
                }
            });
});


builder.Services.AddHostedService<MyBackgroundService>();
//builder.Services.AddHostedService<MyHostedServise>();
builder.Services.AddScoped<ICandidate, CandidateRepository>();
builder.Services.AddScoped<ICalendar, CalendarRepository>();
builder.Services.AddScoped<ITechnology, ITechnologyRepository>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<OfferService>();
builder.Services.AddScoped<StatusRepository>();
builder.Services.AddScoped<ClientRepository>();
builder.Services.AddScoped<TaxService>();
builder.Services.AddScoped<ILinkedInService, LinkedInService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<INote, NoteRepository>();
builder.Services.AddScoped<IPage, Page>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddTransient<IAzureStorage, AzureStorage>();

builder.Services.AddControllers();
builder.Services.AddCors();


var Configuration = builder.Configuration;

var emailConfig = Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

builder.Services.AddDbContext<MouseTagProjectContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;
}).AddEntityFrameworkStores<MouseTagProjectContext>().AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(2));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Configuration["JwtConfig:Issuer"],
        ValidAudience = Configuration["JwtConfig:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtConfig:Key"]))
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
    });
}

app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true)); //allow any origin

app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod()
.AllowAnyOrigin().WithExposedHeaders("content-disposition"));

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html");

#if !DEBUG
app.ApplyMigrations();
#endif

var serviceProvider = app.Services.CreateScope().ServiceProvider;
var context = serviceProvider.GetRequiredService<MouseTagProjectContext>();
var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
var userRoles = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
var config = serviceProvider.GetRequiredService<IConfiguration>();
await TechnologySeeder.Seed(context);
await StatusSeeder.Seed(context);
await IdentitySeed.Seed(userManager, userRoles, config);

var storageConnectionString = builder.Configuration.GetValue<string>("BlobConnectionString");
var storageContainerName = builder.Configuration.GetValue<string>("BlobContainerName");
BlobServiceClient container = new BlobServiceClient(storageConnectionString);
await ContainerCreator.CreateSampleContainerAsync(container, storageContainerName); 

app.Run();
