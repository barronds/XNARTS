﻿using System;
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

			public Panel( Widget parent, Style style, xAABB2 relative_aabb )
			{
				Init();
				InitWidget( parent, style, relative_aabb );
			}

			public Panel( Widget parent, Style style, Vector2 size, ePlacement placement )
			{
				Init();
				InitWidget( parent, style, placement, size );
			}

			public Panel()
			{
				// if using this constructor, call a flavor of InitPanel afterwards
			}

			public void InitPanel( Widget parent, Style style, xAABB2 relative_aabb )
			{
				Init();
				InitWidget( parent, style, relative_aabb );
			}

			public void InitPanel( Widget parent, Style style, Vector2 size, ePlacement placement )
			{
				Init();
				InitWidget( parent, style, placement, size );
			}

			public void AddChild( Widget child )
			{
				XUtils.Assert( child != null && mChildren.Find( Widget.CompareWidgets( child ) ) == null );
				mChildren.Add( child );
			}

			public void RemoveChild( Widget child )
			{
				XUtils.Assert( child != null && mChildren.Remove( child ) );
			}

			private void Init()
			{
				mChildren = new List<Widget>();
			}

			public override void Render( XSimpleDraw simple_draw )
			{
				base.Render( simple_draw );
				xAABB2 aabb = GetPosition().GetScreenAABB();
				XUI.Instance().Util_DrawBox( simple_draw, GetStyle(), aabb );

				for( int i = 0; i < mChildren.Count; ++i )
				{
					mChildren[ i ].Render( simple_draw );
				}
			}
		}
	}
}
