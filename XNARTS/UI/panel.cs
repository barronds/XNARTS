using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace XNARTS
{
	public partial class XUI
	{
		public class Panel : Widget
		{
			private List< Widget >	mChildren;
			private Style           mStyle;

			public Panel( Widget parent, eStyle style, xAABB2 aabb )
			{
				Init( style );
				InitWidget( parent, aabb );
			}

			public Panel( Widget parent, eStyle style, Vector2 size, ePlacement placement )
			{
				Init( style );
				InitWidget( parent, placement, size );
			}

			private void Init( eStyle style )
			{
				mStyle = XUI.Instance().GetStyle( style );
				mChildren = new List<Widget>();
			}

			public override void Render( XSimpleDraw simple_draw )
			{
				xAABB2 aabb = GetPosition().GetScreenAABB();
				XUI.Instance().Util_DrawBox( simple_draw, mStyle, aabb );
			}
		}
	}
}
