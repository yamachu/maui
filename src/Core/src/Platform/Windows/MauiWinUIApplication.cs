using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace Microsoft.Maui
{
	public class MauiWinUIApplication<TStartup> : MauiWinUIApplication
		where TStartup : IStartup, new()
	{
		protected override void OnLaunched(UI.Xaml.LaunchActivatedEventArgs args)
		{
			LaunchActivatedEventArgs = args;

			// TODO: This should not be here. CreateWindow should do it.
			MainWindow = new MauiWinUIWindow();

			var startup = new TStartup();

			var host = startup
				.CreateAppHostBuilder()
				.ConfigureServices(ConfigureNativeServices)
				.ConfigureUsing(startup)
				.Build();

			Services = host.Services;
			Application = Services.GetRequiredService<IApplication>();
			Current.Services?.InvokeLifecycleEvents<WindowsLifecycle.OnLaunching>(del => del(this, args));

			var mauiContext = new MauiContext(Services);

			var activationState = new ActivationState(mauiContext, args);
			var window = Application.CreateWindow(activationState);

			var content = window.View;

			var canvas = CreateRootContainer();

			var nativeContent = content.ToNative(mauiContext);

			canvas.Children.Add(nativeContent);

			MainWindow.Content = canvas;

			Current.Services?.InvokeLifecycleEvents<WindowsLifecycle.OnLaunched>(del => del(this, args));

			MainWindow.SizeChanged += (sender, sizeChangedArgs) =>
			{
				// TODO ezhart We need a better signalling mechanism between the RootPanel and the ContentPage for invalidation
				content.InvalidateMeasure();

				// TODO ezhart This is not ideal, but we need to force the root panel size to match the window size
				canvas.Width = MainWindow.Bounds.Width;
				canvas.Height = MainWindow.Bounds.Height;

				// TODO ezhart Once we've got navigation up and running, this will need to be updated so it 
				// affects the navigation root or the current page. 
				nativeContent.Width = canvas.ActualWidth;
				nativeContent.Height = canvas.ActualHeight;
			};

			MainWindow.Activate();
		}

		RootPanel CreateRootContainer()
		{
			// TODO WINUI should this be some other known constant or via some mechanism? Or done differently?
			return Resources.TryGetValue("MauiRootContainerStyle", out object style)
				? new RootPanel
				{
					Style = style as UI.Xaml.Style
				}
				: new RootPanel();
		}

		void ConfigureNativeServices(HostBuilderContext ctx, IServiceCollection services)
		{
		}
	}

	public abstract class MauiWinUIApplication : UI.Xaml.Application
	{
		protected MauiWinUIApplication()
		{
		}

		public static new MauiWinUIApplication Current => (MauiWinUIApplication)UI.Xaml.Application.Current;

		public UI.Xaml.LaunchActivatedEventArgs LaunchActivatedEventArgs { get; protected set; } = null!;

		public MauiWinUIWindow MainWindow { get; protected set; } = null!;

		public IServiceProvider Services { get; protected set; } = null!;

		public IApplication Application { get; protected set; } = null!;
	}
}
