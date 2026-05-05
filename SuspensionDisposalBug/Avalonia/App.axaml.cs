using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using ReactiveUI.Avalonia;
using ReactiveUI.Interfaces;
using SuspensionDisposalBug.Models;
using System;
using System.Reactive.Linq;
using System.Text.Json.Serialization.Metadata;

namespace SuspensionDisposalBug.Avalonia
{
	internal sealed partial class App : Application, IDisposable
	{
		private IDisposable? _suspensionHostDisposable, _shutdownEventSubscription;
		private AutoSuspendHelper? _suspension;

		public override void OnFrameworkInitializationCompleted()
		{
			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				#pragma warning disable IDISP003

					_suspension = new AutoSuspendHelper(desktop);

					_shutdownEventSubscription =
						Observable.FromEventPattern<ShutdownRequestedEventArgs>(
										handler => desktop.ShutdownRequested += handler,
										handler => desktop.ShutdownRequested -= handler)
								  .Subscribe(_ => Dispose());

					/* Both '_suspension' and '_shutdownEventSubscription' are disposed of during
					 * app shutdown. */

				#pragma warning restore IDISP003

				_suspension.OnFrameworkInitializationCompleted();

				ISuspensionHost<AppModel>? suspensionHost =
					RxSuspension.SuspensionHost as ISuspensionHost<AppModel>;

				if (suspensionHost is not null)
				{
					#pragma warning disable IDISP003

						/* _suspensionHostDisposable is assigned only once during app
						 * initialisation, so there is no previous value to dispose of. */

						_suspensionHostDisposable = ConfigureSuspensionHost(suspensionHost);

					#pragma warning restore IDISP003

					desktop.MainWindow = new MainWindow();
				}
			}

			base.OnFrameworkInitializationCompleted();
		}

		private static IDisposable ConfigureSuspensionHost(
			ISuspensionHost<AppModel> suspensionHost)
		{
			suspensionHost.CreateNewAppStateTyped = () =>
			{
				return new AppModel();
			};

			JsonTypeInfo<AppModel> typeInfo =
				AppModelSerialiserContext.Default.AppModel;

			return suspensionHost.SetupDefaultSuspendResume(typeInfo);
		}

		public void Dispose()
		{
			_shutdownEventSubscription?.Dispose();

			/* When the AutoSuspendHelper instance is disposed of here, a runtime exception occurs,
			 * and an 'app-state.json' file fails to generate. */

			_suspension?.Dispose();
			_suspensionHostDisposable?.Dispose();

			// "Exception thrown: 'System.ObjectDisposedException' in System.Reactive.dll"
		}
	}
}
