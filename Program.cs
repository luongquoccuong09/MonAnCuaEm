using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MonAnCuaEm;
using MonAnCuaEm.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// UI + lưu trữ cục bộ
builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

// Lưu trữ dùng chung trên Supabase (mirror xuống máy để vẫn xem offline)
builder.Services.AddScoped<IAppStore>(sp =>
{
    var http = new HttpClient { BaseAddress = new Uri(SupabaseConfig.Url) };
    http.DefaultRequestHeaders.TryAddWithoutValidation("apikey", SupabaseConfig.Key);
    http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {SupabaseConfig.Key}");
    return new SupabaseStore(http, sp.GetRequiredService<ILocalStorageService>());
});

// Nghiệp vụ
builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<MealPlanService>();
builder.Services.AddScoped<ExpenseService>();
builder.Services.AddScoped<StatisticsService>();
builder.Services.AddScoped<ImageService>();

await builder.Build().RunAsync();
