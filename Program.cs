using System.Text;
using krist_server.Interfaces;
using krist_server.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Supabase;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
});
builder.Services.AddControllers();

// builder.Services.AddControllers().AddNewtonsoftJson(options =>
// {
//     // Preserve property casing (PascalCase) from C# class
//     options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();

//     // Ignore null values in the JSON response
//     options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
// });

string url = builder.Configuration["Supabase:Url"] ?? throw new ArgumentNullException("Supabase:Url", "Supabase URL is missing from configuration.");
string key = builder.Configuration["Supabase:ApiKey"] ?? throw new ArgumentNullException("Supabase:ApiKey", "Supabase ApiKey is missing from configuration.");

var supabaseClient = new Client(url, key);
supabaseClient.InitializeAsync().Wait();

builder.Services.AddSingleton(supabaseClient);
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

var bytes = Encoding.UTF8.GetBytes(builder.Configuration["Authentication:JwtSecret"] ?? throw new ArgumentNullException("Authentication:JwtSecret", "Authentication JwtSecret is missing from configuration."));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(bytes),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Authentication:ValidAudience"],
            ValidIssuer = builder.Configuration["Authentication:ValidIssuer"],
            ValidateLifetime = true,
        };

        // options.Events = new JwtBearerEvents
        // {
        //     OnAuthenticationFailed = context =>
        //     {
        //         Console.WriteLine("Authentication failed: " + context.Exception.Message);
        //         return Task.CompletedTask;
        //     },
        //     OnTokenValidated = context =>
        //     {
        //         Console.WriteLine("Token validated.");
        //         return Task.CompletedTask;
        //     },
        //     OnMessageReceived = context =>
        //     {
        //         Console.WriteLine("Message received.");
        //         return Task.CompletedTask;
        //     }
        // };

    });

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddControllers();
// builder.Services.AddDbContext<ProductContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
