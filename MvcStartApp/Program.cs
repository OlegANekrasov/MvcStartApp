using MvcStartApp.Middleware;
using MvcStartApp;
using Microsoft.EntityFrameworkCore;
using MvcStartApp.Models.Db;

var builder = WebApplication.CreateBuilder(args);

// �������� ������ ����������� �� ����� ������������
string connection = builder.Configuration.GetConnectionString("DefaultConnection");

// ��������� �������� ApplicationContext � �������� ������� � ����������
builder.Services.AddDbContext<BlogContext>(options => options.UseSqlServer(connection));

// Add services to the container.
builder.Services.AddControllersWithViews();

// ����������� ������� ����������� ��� �������������� � ����� ������
builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// ���������� ����������� � �������������� �� �������������� ����
app.UseMiddleware<LoggingMiddleware>();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
