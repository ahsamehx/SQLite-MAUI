﻿using Microsoft.Extensions.Logging;
using MuaiSQL.Data;
using MuaiSQL.ViewModels;

namespace MuaiSQL
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
            builder.Services.AddSingleton<DatabaseContext>();
            builder.Services.AddSingleton<ProductsViewModel>();
            builder.Services.AddSingleton<MainPage>();
            return builder.Build();
        }
    }
}
