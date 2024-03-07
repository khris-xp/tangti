using tangti.Configs;
using tangti.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.Configure<TangtiDatabaseSetting>(
    builder.Configuration.GetSection("TangtiDatabase"));

builder.Services.Configure<MailSettings>(
    builder.Configuration.GetSection("SmtpSettings"));


builder.Services.AddSingleton<BlogService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<EnrollService>();
builder.Services.AddSingleton<EventService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<CategoryService>();
builder.Services.AddSingleton<ReportService>();
builder.Services.AddTransient<EmailService>();

builder.Services.AddControllersWithViews();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAny",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("K6g(u)%mb8Dp*HkPSTFG@UIf^yBWqvxdt5YMnANcLjs9w#2!h3+QRVzCe$XrE47a")),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowAny");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "register",
    pattern: "Register/{action=Index}/{id?}",
    defaults: new { controller = "Register" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
