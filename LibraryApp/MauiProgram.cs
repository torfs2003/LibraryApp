using Microsoft.Extensions.Logging;

namespace LibraryApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();

#endif

            builder.Services.AddSingleton<BookService>();
            builder.Services.AddSingleton<InventoryService>();
            builder.Services.AddSingleton<UserService>();

            builder.Services.AddSingleton<BaseViewModel>();
            builder.Services.AddSingleton<BookViewModel>();
            builder.Services.AddSingleton<InventoryViewModel>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<AdminViewModel>();

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<InventoryPage>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<AdminPage>();


            return builder.Build();
        }
    }
}