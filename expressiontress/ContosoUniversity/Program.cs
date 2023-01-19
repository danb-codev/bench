using ContosoUniversity.Data;
using ContosoUniversity.Infrastructure;
using ContosoUniversity.Infrastructure.Tags;
using HtmlTags;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

RegisterServices(builder);

var app = builder.Build();

ConfigureApplication(app);

app.Run();

static void RegisterServices(WebApplicationBuilder builder)
{
    var services = builder.Services;

    services.AddMiniProfiler().AddEntityFramework();

    services.AddDbContext<SchoolContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    services.AddAutoMapper(typeof(Program));
    services.AddMediatR(typeof(Program));
    services.AddHtmlTags(new TagConventions());
    services.AddRazorPages(opt =>
    {
        opt.Conventions.ConfigureFilter(new DbContextTransactionPageFilter());
        opt.Conventions.ConfigureFilter(new ValidatorPageFilter());
    });

    services.AddMvc(opt => opt.ModelBinderProviders.Insert(0, new EntityModelBinderProvider()));
}

static void ConfigureApplication(WebApplication app)
{
    app.UseMiniProfiler();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseAuthentication();

    app.MapRazorPages();
}

//// Add services to the container.
//builder.Services.AddRazorPages();
//
//var app = builder.Build();
//
//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}
//
//app.UseHttpsRedirection();
//app.UseStaticFiles();
//
//app.UseRouting();
//
//app.UseAuthorization();
//
//app.MapRazorPages();
//
//app.Run();
