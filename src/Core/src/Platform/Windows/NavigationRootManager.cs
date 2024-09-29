﻿using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Microsoft.Maui.Platform
{
	public partial class NavigationRootManager
	{
		readonly WeakReference<Window> _platformWindow;
		WindowRootView _rootView;
		bool _disconnected = true;
		internal event EventHandler? OnApplyTemplateFinished;

		public NavigationRootManager(Window platformWindow)
		{
			_platformWindow = new(platformWindow);
			_rootView = new WindowRootView();
			_rootView.BackRequested += OnBackRequested;
			_rootView.OnApplyTemplateFinished += WindowRootViewOnApplyTemplateFinished;

			var titleBar = platformWindow.GetAppWindow()?.TitleBar;
			if (titleBar is not null)
			{
				SetTitleBarVisibility(platformWindow, titleBar.ExtendsContentIntoTitleBar);
			}
		}

		internal void SetTitleBarVisibility(Window platformWindow, bool isVisible)
		{
			// https://learn.microsoft.com/en-us/windows/apps/design/basics/titlebar-design
			// Standard title bar height is 32px
			// This should always get set by the code after but
			// we are setting it just in case
			var appbarHeight = isVisible ? 32 : 0;
			if (isVisible && UI.Windowing.AppWindowTitleBar.IsCustomizationSupported())
			{
				var titleBar = platformWindow.GetAppWindow()?.TitleBar;
				if (titleBar is not null)
				{
					var density = platformWindow.GetDisplayDensity();
					appbarHeight = (int)(titleBar.Height / density);
				}
			}

			_rootView.UpdateAppTitleBar(
					appbarHeight,
					UI.Windowing.AppWindowTitleBar.IsCustomizationSupported() &&
					isVisible
				);
		}

		void WindowRootViewOnWindowTitleBarContentSizeChanged(object? sender, EventArgs e)
		{
			if (_disconnected)
				return;

			_platformWindow.GetTargetOrDefault()?
				.GetWindow()?
				.Handler?
				.UpdateValue(nameof(IWindow.TitleBarDragRectangles));
		}

		void WindowRootViewOnApplyTemplateFinished(object? sender, System.EventArgs e) =>
			OnApplyTemplateFinished?.Invoke(this, EventArgs.Empty);

		void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
		{
			_platformWindow.GetTargetOrDefault()?
				.GetWindow()?
				.BackButtonClicked();
		}

		internal FrameworkElement? AppTitleBar => _rootView.AppTitleBar;
		internal MauiToolbar? Toolbar => _rootView.Toolbar;
		public FrameworkElement RootView => _rootView;

		public virtual void Connect(UIElement platformView)
		{
			if (_rootView.Content != null)
			{
				// Clear out the toolbar that was set from the previous content
				SetToolbar(null);

				// We need to make sure to clear out the root view content 
				// before creating the new view.
				// Otherwise the new view might try to act on the old content.
				// It might have code in the handler that retrieves this class.
				_rootView.Content = null;
			}

			_rootView.Content = platformView is NavigationView ? platformView : new RootNavigationView()
			{
				Content = platformView
			};

			if (_disconnected && _platformWindow.TryGetTarget(out var platformWindow))
			{
				platformWindow.Activated += OnWindowActivated;
				_disconnected = false;
			}

			_rootView.OnWindowTitleBarContentSizeChanged += WindowRootViewOnWindowTitleBarContentSizeChanged;
		}

		public virtual void Disconnect()
		{
			_rootView.OnWindowTitleBarContentSizeChanged -= WindowRootViewOnWindowTitleBarContentSizeChanged;
			if (_platformWindow.TryGetTarget(out var platformWindow))
			{
				platformWindow.Activated -= OnWindowActivated;
			}
			SetToolbar(null);

			if (_rootView.Content is RootNavigationView navView)
				navView.Content = null;

			_rootView.Content = null;
			_disconnected = true;
		}

		internal void SetMenuBar(MenuBar? menuBar)
		{
			_rootView.MenuBar = menuBar;
		}

		internal void SetToolbar(FrameworkElement? toolBar)
		{
			_rootView.Toolbar = toolBar as MauiToolbar;
		}

		internal string? WindowTitle
		{
			get => WindowRootView.WindowTitle;
			set => WindowRootView.WindowTitle = value;
		}

		internal void SetTitle(string? title) =>
			WindowRootView.WindowTitle = title;

		void OnWindowActivated(object sender, WindowActivatedEventArgs e)
		{
			SolidColorBrush defaultForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
			SolidColorBrush inactiveForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorDisabledBrush"];

			if (e.WindowActivationState == WindowActivationState.Deactivated)
			{
				WindowRootView.WindowTitleForeground = inactiveForegroundBrush;
			}
			else
			{
				WindowRootView.WindowTitleForeground = defaultForegroundBrush;
			}
		}
	}
}