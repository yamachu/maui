#nullable enable

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Handlers;

namespace Microsoft.Maui
{
	static class ViewHandlerExtensions
	{
		public static IServiceProvider GetServiceProvider(this IViewHandler handler)
		{
			var context = handler.MauiContext ??
				throw new InvalidOperationException($"Unable to find the context. The {nameof(ViewHandler.MauiContext)} property should have been set by the host.");

			var services = context?.Services ??
				throw new InvalidOperationException($"Unable to find the service provider. The {nameof(ViewHandler.MauiContext)} property should have been set by the host.");

			return services;
		}

		public static T? GetService<T>(this IViewHandler handler, Type type)
		{
			var services = handler.GetServiceProvider();

			var service = services.GetService(type);

			return (T?)service;
		}

		public static T? GetService<T>(this IViewHandler handler)
		{
			var services = handler.GetServiceProvider();

			var service = services.GetService<T>();

			return service;
		}

		public static T GetRequiredService<T>(this IViewHandler handler, Type type)
			where T : notnull
		{
			var services = handler.GetServiceProvider();

			var service = services.GetRequiredService(type);

			return (T)service;
		}

		public static T GetRequiredService<T>(this IViewHandler handler)
			where T : notnull
		{
			var services = handler.GetServiceProvider();

			var service = services.GetRequiredService<T>();

			return service;
		}
	}
}