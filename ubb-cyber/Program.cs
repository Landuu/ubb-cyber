using AspNetCore.ReCaptcha;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using ubb_cyber.Database;
using ubb_cyber.Services.PrincipalProvider;
using ubb_cyber.Services.UserService;
using ubb_cyber.Services.ValidatorUserProvider;
using ubb_cyber.Validators;
using ubb_cyber.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Forbidden/";
        options.LoginPath = "/Auth/Login";
    });
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddReCaptcha(builder.Configuration.GetSection("ReCaptcha"));


builder.Services.AddScoped<IPrincipalProvider, PrincipalProvider>();
builder.Services.AddScoped<IUserService, UserService>();

// Validators
builder.Services.AddScoped<IValidatorUserProvider, ValidatorUserProvider>();
builder.Services.AddScoped<IValidator<LoginViewModel>, LoginViewModelValidator>();
builder.Services.AddScoped<IValidator<ResetPasswordViewModel>, ResetPasswordViewModelValidator>();
builder.Services.AddScoped<IValidator<ChangePasswordViewModel>, ChangePasswordViewModelValidator>();
builder.Services.AddScoped<IValidator<PanelAddUserViewModel>, PanelAddUserViewModelValidator>();
builder.Services.AddScoped<IValidator<PanelEditUserViewModel>, PanelEditUserViewModelValidator>();
builder.Services.AddScoped<IValidator<PanelPolicyViewModel>, PanelPolicyViewModelValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();

app.UseCookiePolicy(new CookiePolicyOptions()
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
