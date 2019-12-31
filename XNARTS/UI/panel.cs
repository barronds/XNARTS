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
			private List< Widget > mChildren;

			public Panel()
			{
				mChildren = new List<Widget>();
			}

			public void AssemblePanel( Vector2 size )
			{
				// extending class needs to explicitly assemble the children, then call this
				for ( int c = 0; c < mChildren.Count; ++c )
				{
					XUtils.Assert( mChildren[ c ].IsAssembled() );
				}

				AssembleWidget( size );
			}

			public void PlacePanel( Widget parent, Style style, xAABB2 relative_aabb )
			{
				// extending class needs to explicitly place itself with this method, then place its children
				PlaceWidget( parent, style, relative_aabb );
			}

			public void PlacePanel( Widget parent, Style style, ePlacement placement )
			{
				// extending class needs to explicitly place itself with this method, then place its children
				PlaceWidget( parent, style, placement );
			}

			public void AddChild( Widget child )
			{
				XUtils.Assert( child != null && mChildren.Find( Widget.CompareWidgets( child ) ) == null );
				mChildren.Add( child );
			}

			public int GetNumChildren()
			{
				return mChildren.Count();
			}

			public Widget GetChild( int i )
			{
				return mChildren[ i ];
			}

			public void RemoveChild( Widget child )
			{
				XUtils.Assert( child != null && mChildren.Remove( child ) );
			}

			public override void Render( XSimpleDraw simple_draw )
			{
				base.Render( simple_draw );
				xAABB2 aabb = GetPosition().GetScreenAABB();
				Style s = GetStyle();
				XUI.Instance().Util_DrawBox( simple_draw, s.mBackgroundColor, s.mBorderColor, aabb );
				RenderChildren( simple_draw );
			}

			public void RenderChildren( XSimpleDraw simple_draw )
			{
				for ( int i = 0; i < mChildren.Count; ++i )
				{
					mChildren[ i ].Render( simple_draw );
				}
			}
		}
	}
}
