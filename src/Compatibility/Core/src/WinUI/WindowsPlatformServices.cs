using Microsoft.UI.Dispatching;
using Windows.UI.Core;

namespace Microsoft.Maui.Controls.Compatibility.Platform.UWP
{
	internal class WindowsPlatformServices : WindowsBasePlatformServices
	{
		public WindowsPlatformServices(DispatcherQueue dispatcher) : base(dispatcher)
		{
		}
	}
}