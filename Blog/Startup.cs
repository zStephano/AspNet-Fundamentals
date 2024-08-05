using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;

namespace Blog
{
    public static class Startup
    {

        public static void LoadConfiguration(WebApplication app)
        {
            Configuration.JwtKey = app.Configuration.GetValue<string>("JwtKey");
            Configuration.ApiKeyName = app.Configuration.GetValue<string>("ApiKeyName");
            Configuration.ApiKey = app.Configuration.GetValue<string>("ApiKey");

            var smtp = new Configuration.SmtpConfiguration();
            app.Configuration.GetSection("BrevoConfiguration").Bind(smtp);
            Configuration.Smtp = smtp;
        }

        public static void ConfigureAuthentication(WebApplicationBuilder builder)
        {
            var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        public static void ConfigureMvc(WebApplicationBuilder builder)
        {
            builder.Services.AddMemoryCache(); // Enable Memory cache for requests/responses
            builder.Services.AddResponseCompression(options =>
            {
                // options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                // options.Providers.Add<CustomCompressionProvider>();
            }); //  Enable Compression for large data response
            builder.Services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });
            builder
                .Services
                .AddControllers()
                .ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; })
                .AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                });
        }

        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("SqlConnection");
            builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddTransient<TokenService>(); // Sempre cria/instancia um novo objeto/conexão
            builder.Services.AddTransient<EmailService>();
            //builder.Services.AddScoped(); // Após a primeira instância, será reutilizado a cada requisição
            //builder.Services.AddSingleton(); // Após a primeira execução, será sempre executado (ficará na memória do App)
        }
    }
}
