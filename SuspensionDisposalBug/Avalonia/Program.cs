using Avalonia;
using ReactiveUI;
using ReactiveUI.Avalonia;
using ReactiveUI.Interfaces;
using SuspensionDisposalBug.Avalonia.Services;
using SuspensionDisposalBug.Models;
using System;
using System.IO;

namespace SuspensionDisposalBug.Avalonia
{
	internal sealed class Program
	{
		/* Initialization code. Don't use any Avalonia, third-party APIs or any
		 * SynchronizationContext-reliant code before AppMain is called: things aren't initialized
		 * yet and stuff might break. */
		[STAThread]
		public static void Main(string[] args) => BuildAvaloniaApp()
			.StartWithClassicDesktopLifetime(args);

		// Avalonia configuration, don't remove; also used by visual designer.
		public static AppBuilder BuildAvaloniaApp()
			=> AppBuilder.Configure<App>()
				.UsePlatformDetect()
				.WithInterFont()
				.LogToTrace()
				.UseReactiveUI(rxAppBuilder =>
				{
					rxAppBuilder
						.WithAvalonia()
						.WithRegistration(locator =>
						{
							string widgetConfigurationFilepath =
								Path.Combine(AppContext.BaseDirectory, "app-state.json");

							locator.Register<ISuspensionDriver>(() =>
								new SuspensionService(widgetConfigurationFilepath));

							locator.Register<AppModel>(() =>
							{
								ISuspensionHost<AppModel> suspensionHost =
									RxSuspension.SuspensionHost as ISuspensionHost<AppModel>
									?? throw new InvalidOperationException();

								return suspensionHost.GetAppState();
							});

						})
						.WithSuspensionHost<AppModel>();
				});
	}
}
