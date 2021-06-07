using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Native;
using UIKit;

namespace Microsoft.Maui
{
	public partial class WrapperView : UIView
	{
		CAShapeLayer? _maskLayer;
		CAShapeLayer? _shadowLayer;
		SizeF _lastMaskSize;

		public WrapperView()
		{
		}

		public WrapperView(CGRect frame)
			: base(frame)
		{
		}

		public CAShapeLayer? MaskLayer
		{
			get => _maskLayer;
			set
			{
				var layer = GetBackgroundLayer();

				if (layer != null && _maskLayer != null)
					layer.Mask = null;

				_maskLayer = value;

				if (layer != null)
					layer.Mask = value;
			}
		}

		public CAShapeLayer? ShadowLayer
		{
			get => _shadowLayer;
			set
			{
				_shadowLayer?.RemoveFromSuperLayer();
				_shadowLayer = value;

				if (_shadowLayer != null)
					Layer.InsertSublayer(_shadowLayer, 0);
			}
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			if (Subviews.Length == 0)
				return;

			var child = Subviews[0];

			child.Frame = Bounds;

			if (MaskLayer != null)
				MaskLayer.Frame = Bounds;

			if (ShadowLayer != null)
				ShadowLayer.Frame = Bounds;

			UpdateLayers();
		}

		public override CGSize SizeThatFits(CGSize size)
		{
			if (Subviews.Length == 0)
				return base.SizeThatFits(size);

			var child = Subviews[0];

			return child.SizeThatFits(size);
		}

		public override void SetNeedsLayout()
		{
			base.SetNeedsLayout();

			Superview?.SetNeedsLayout();
		}

		partial void ClipShapeChanged()
		{
			_lastMaskSize = SizeF.Zero;

			if (Frame == CGRect.Empty)
				return;
		}

		partial void ShadowChanged()
		{
			_lastMaskSize = SizeF.Zero;

			if (Frame == CGRect.Empty)
				return;
		}

		void UpdateLayers()
		{
			var mask = MaskLayer;

			if (mask == null && ClipShape == null)
				return;

			mask ??= MaskLayer = new CAShapeLayer();
			var frame = Frame;
			var bounds = new RectangleF(0, 0, (float)frame.Width, (float)frame.Height);

			if (bounds.Size == _lastMaskSize)
				return;

			_lastMaskSize = bounds.Size;
			var pathForBounds = _clipShape?.PathForBounds(bounds);
			var nativePath = pathForBounds?.AsCGPath();
			mask.Path = nativePath;

			var shadowLayer = ShadowLayer;

			if (shadowLayer == null && Shadow != null && Shadow.Value.IsEmpty)
				return;

			shadowLayer ??= ShadowLayer = new CAShapeLayer();
			shadowLayer.FillColor = new CGColor(0, 0, 0, 1);
			shadowLayer.Path = nativePath;
			
			if (Shadow!.Value.IsEmpty)
				shadowLayer.ClearShadow();
			else
				shadowLayer.SetShadow(Shadow!.Value);
		}

		CALayer? GetBackgroundLayer()
		{
			return Layer.Sublayers
				.FirstOrDefault(l => l.Name == ViewExtensions.BackgroundLayerName);
		}
	}
}