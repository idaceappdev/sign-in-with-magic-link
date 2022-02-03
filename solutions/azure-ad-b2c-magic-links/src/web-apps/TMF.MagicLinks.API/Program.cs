using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using SendGrid.Extensions.DependencyInjection;
using TMF.MagicLinks.API.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2CConfiguration"));

builder.Services.AddSendGrid((sp, options) =>
{
    options.ApiKey = builder.Configuration
                            .GetSection("SendGridConfiguration")["ApiKey"];
});

builder.Services.AddScoped<IdTokenHintBuilder>();
builder.Services.AddScoped<MailDeliveryService>();
builder.Services.AddScoped<TokenSigningCertificateManager>();
builder.Services.AddScoped<UserMagicLinkInvitationHandler>();

builder.Services.AddControllers();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
