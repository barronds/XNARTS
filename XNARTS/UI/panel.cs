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
			private List< Widget > mChildren;

			public Panel()
			{
				mChildren = new List<Widget>();
			}

			public void Assemble()
			{
				// extending class needs to explicitly assemble the children, then call this
				for ( int c = 0; c < mChildren.Count; ++c )
				{
					XUtils.Assert( mChildren[ c ].IsAssembled() );
				}

				AssembleWidget();
			}

			public void Place( Widget parent, Style style, xAABB2 relative_aabb, eInitialState state )
			{
				// extending class needs to explicitly place itself with this method, then place its children
				PlaceWidget( parent, style, relative_aabb, state );
			}

			public void Place( Widget parent, Style style, Vector2 size, ePlacement placement, eInitialState state )
			{
				// extending class needs to explicitly place itself with this method, then place its children
				PlaceWidget( parent, style, placement, size, state );
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

			public override void FacilitateInteractability( bool interactable )
			{
				// TODO: haven't thought about this yet
				base.FacilitateInteractability( interactable );
			}

			public override void SetState( eInputChange i, eFocusChange f, eVisibilityChange v )
			{
				base.SetState( i, f, v );

				for( int c = 0; c < mChildren.Count; ++c )
				{
					mChildren[ c ].SetState( i, f, v );
				}
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
