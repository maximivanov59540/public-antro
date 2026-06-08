using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Antro.Application;
using Antro.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IAntroClock, SystemAntroClock>();
builder.Services.AddScoped<IStageShowcaseBuilder>(_ => new StageShowcaseBuilder());
builder.Services.AddScoped<IDashboardBuilder>(_ => new DashboardBuilder());
builder.Services.AddScoped<IBenefitDetailBuilder>(_ => new BenefitDetailBuilder());
builder.Services.AddScoped<IQuestionnaireFlow>(_ => new QuestionnaireFlow());
builder.Services.AddScoped<InMemoryUserProfileState>();

await builder.Build().RunAsync();
