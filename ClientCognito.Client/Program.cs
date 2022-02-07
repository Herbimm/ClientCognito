using Amazon;
using Amazon.CognitoIdentity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options => {
        options.ClientId = "75uvvl7sd7uoo9s0jco9mskcqi";
        options.ClientSecret = "1jsn6jrg6vebom2161f2eu5v6ad7uqfukj5l7lru2c49esus0233";
        options.Authority = "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_2lpuOG5PD";
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.ResponseType = "code";
        options.Scope.Add("http://leave.firstclient.com/leaves.cancel");
        //options.Scope.Add("http://leave.firstclient.com/leaves.apply");
        options.SaveTokens = true;

        options.TokenValidationParameters = new TokenValidationParameters()
        {
            NameClaimType = "cognito:user"
        };
        options.Events = new OpenIdConnectEvents()
        {
            OnRedirectToIdentityProviderForSignOut = context =>
            {
                var logoutUri = $"https://firstclient-identity.auth.us-east-1.amazoncognito.com/logout?client_id=75uvvl7sd7uoo9s0jco9mskcqi";
                logoutUri += $"&logout_uri={context.Request.Scheme}://{context.Request.Host}";


                context.Response.Redirect(logoutUri);
                context.HandleResponse();
                return Task.CompletedTask;
            }   
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
