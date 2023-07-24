using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using ASPNetVueTemplate.Models;
using ASPNetVueTemplate.Helpers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

//Add your database context with the connection string name in appsettings.json
builder.Services.AddDbContext<ExampleAppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ExampleAppDb")));

//Add an email service to the application
builder.Services.AddScoped((serviceProvider) => {
	var config = serviceProvider.GetRequiredService<IConfiguration>();

	var cli = new SmtpClient()
	{
		Host = config.GetValue<string>("Email:Smtp:Host"),
		Port = config.GetValue<int>("Email:Smtp:Port"),
		EnableSsl = config.GetValue<bool>("Email:Smtp:EnableSsl")
	};

	if (!string.IsNullOrEmpty(config.GetValue<string>("Email:Smtp:Username")))
	{
		cli.Credentials = new NetworkCredential(
			config.GetValue<string>("Email:Smtp:Username"),
			config.GetValue<string>("Email:Smtp:Password")
		);
	}

	return new MailSender
	{
		SmtpClient = cli,
		FromEmail = config.GetValue<string>("Email:Smtp:Sender")
	};
});

builder.Services.AddHttpContextAccessor();

//Map the AppSettings section of appsettings.json to the AppSettings model class
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);
var appSettings = appSettingsSection.Get<AppSettings>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseHsts();
	app.UseHttpsRedirection();
	app.UseForwardedHeaders(new ForwardedHeadersOptions
	{
		ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
	});
}
else
{
	app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
}

app.UseExceptionHandler(a => a.Run(async context =>
{
	var feature = context.Features.Get<IExceptionHandlerPathFeature>();
	var exception = feature.Error;

	var errorList = new Dictionary<string, List<string>>
				{
					{ "exception", new List<string> { exception.Message } }
				};

	var result = JsonConvert.SerializeObject(new
	{
		errors = errorList,
		stackTrace = ErrorUtility.GetExceptionMessages(exception)
	});
	context.Response.ContentType = "application/json";
	await context.Response.WriteAsync(result);
}));

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ExampleAppDbContext>();
db.Database.Migrate();

app.Run();
