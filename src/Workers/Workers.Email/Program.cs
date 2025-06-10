using BuildingBlocks.Messaging.MassTransit;
using System.Reflection;
using Workers.Email.Configurations;
using Workers.Email.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddMessageBroker(builder.Configuration, Assembly.GetExecutingAssembly(), useCompetingConsumers: true);

builder.Services.AddSingleton<IEmailTemplateRenderer, RazorEmailTemplateRenderer>();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

var host = builder.Build();
host.Run();
