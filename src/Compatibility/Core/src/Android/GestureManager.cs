using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.Views;
using AndroidX.Core.View;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Controls.Compatibility.Platform.Android
{
	internal class GestureManager : Controls.Platform.GestureManager
	{
		IVisualElementRenderer _renderer;
		protected override global::Android.Views.View Control => _renderer.View;
		protected override VisualElement Element => _renderer.Element;

		public GestureManager(IVisualElementRenderer renderer)
		{
			_renderer = renderer;
		}
	}
}
