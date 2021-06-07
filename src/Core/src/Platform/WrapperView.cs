using Microsoft.Maui.Graphics;

namespace Microsoft.Maui
{
	public partial class WrapperView
	{
		IShape? _clipShape;
		Shadow? _shadow;

		public IShape? ClipShape
		{
			get => _clipShape;
			set
			{
				if (_clipShape == value)
					return;

				_clipShape = value;
				ClipShapeChanged();
			}
		}

		public Shadow? Shadow
		{
			get => _shadow;
			set
			{
				_shadow = value;
				ShadowChanged();
			}
		}

		partial void ClipShapeChanged();
		partial void ShadowChanged();
	}
}