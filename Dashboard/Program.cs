using BusinessObject;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using MailKit;
using Repository.AccountRepo;
using Repository.Feedback_VocaRepo;
using Repository.General_FeedbackRepo;
using Repository.VocabularyRepo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IVocabularyRepository, VocabularyRepository>();
builder.Services.AddScoped<IFeedback_VocaRepository, Feedback_VocaRepository>();
builder.Services.AddScoped<IGeneral_FeedbackRepository, General_FeedbackRepository>();
builder.Services.AddDistributedMemoryCache();
//load thông tin c?u hình và l?u vào ??i t??ng MailSetting
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
//add dependency inject cho MailService
//builder.Services.AddTransient<IMailService, MailService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("keyfirebase.json")
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
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=Login}");

app.Run();
