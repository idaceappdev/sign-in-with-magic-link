using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using TMF.MagicLinks.WebApp.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2CConfiguration"));

string magic_link_policy = builder.Configuration
                           .GetSection("AzureAdB2CConfiguration")["MagicLinksPolicyId"];
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(magic_link_policy, GetOpenIdSignUpOptions(magic_link_policy));

builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();


Action<OpenIdConnectOptions> GetOpenIdSignUpOptions(string policy)
   => options =>
   {
       string clientId = builder.Configuration.GetSection("AzureAdB2CConfiguration")["ClientId"];
       string B2CDomain = builder.Configuration.GetSection("AzureAdB2CConfiguration")["Instance"];
       string Domain = builder.Configuration.GetSection("AzureAdB2CConfiguration")["Domain"];
       string MagicLinkPolicy = builder.Configuration.GetSection("AzureAdB2CConfiguration")["MagicLinksPolicyId"];

       options.MetadataAddress = $"{B2CDomain}/{Domain}/{MagicLinkPolicy}/v2.0/.well-known/openid-configuration";
       options.ClientId = clientId;
       options.ResponseType = OpenIdConnectResponseType.IdToken;
       options.SignedOutCallbackPath = "/signout/" + policy;

       if (policy == MagicLinkPolicy)
           options.CallbackPath = "/signin-oidc-link";

       options.SignedOutRedirectUri = "/";
       options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
       options.Events = new OpenIdConnectEvents
       {
           OnRedirectToIdentityProvider = context =>
           {
               if (context.Properties.Items.ContainsKey("id_token_hint"))
               {
                   context.ProtocolMessage.SetParameter("id_token_hint", context.Properties.Items["id_token_hint"]);
               }

               return Task.FromResult(0);
           }
       };
   };
