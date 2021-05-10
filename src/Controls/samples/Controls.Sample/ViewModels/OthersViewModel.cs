using System.Collections.Generic;
using Maui.Controls.Sample.Models;
using Maui.Controls.Sample.ViewModels.Base;
using Maui.Controls.Sample.Pages;

namespace Maui.Controls.Sample.ViewModels
{
    public class OthersViewModel : BaseGalleryViewModel
	{
#if NET6_0_OR_GREATER
		protected override IEnumerable<SectionModel> CreateItems() => new[]
		{
			new SectionModel(typeof(BlazorPage), "BlazorWebView",
				"The BlazorWebView control allow to easily embed Blazor content with native UI.")
		};
#else	
		protected override IEnumerable<SectionModel> CreateItems() => null;
#endif
	}
}