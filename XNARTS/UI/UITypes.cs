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
		public class _Position
		{
			private bool	mCentered;
			private Vector2 mPosition;

			// one constructor for centered and one for absolute position.  could add one for relative.
			public _Position()
			{
				mCentered = true;
				mPosition = Vector2.Zero;
			}
			public _Position( Vector2 pos )
			{
				mCentered = false;
				mPosition = pos;
			}

			public bool IsCentered()
			{
				return mCentered;
			}
			public Vector2 GetPosition()
			{
				return mPosition;
			}
		}


		public enum ePlacement
		{
			Invalid = -1,

			Absolute, // Vector2, a position relative to parent widget
			Centered,
			TopLeft,
			TopRight,
			BottomLeft,
			BottomRight,
			CenteredBottom,

			Num
		}

		public class Position
		{
			private ePlacement	mPlacement;
			private xAABB2		mRelativeAABB;	// relative to parent's aabb min point
			private Widget      mParent;		// can be null, for screen widget

			// constructor for absolute position relative to widget.  use screen widget for screen space position.
			public Position( Widget parent, xAABB2 relative_aabb )
			{
				mRelativeAABB = relative_aabb;
				Init( parent, ePlacement.Absolute );
			}

			// constructor for placement relative to a widget.  use screen widget for screen placement.
			public Position( Widget parent, ePlacement placement, Vector2 size )
			{
				XUtils.Assert( placement != ePlacement.Absolute, "wrong constructor for absolute" );
				Init( parent, placement );

				switch ( placement )
				{
					case ePlacement.Centered:
						PlaceCentered( size );
						break;

					case ePlacement.CenteredBottom:
						PlaceCenteredBottom( size );
						break;

					default:
						XUtils.Assert( false, "placement type not yet supported" );
						break;
				}
			}

			public xAABB2 GetRelatveAABB()
			{
				return mRelativeAABB;
			}

			public xAABB2 GetScreenAABB()
			{
				if( mParent != null )
				{
					xAABB2 parent_screen_aabb = mParent.GetPosition().GetScreenAABB();
					xAABB2 aabb = mRelativeAABB; 
					aabb.Translate( parent_screen_aabb.GetMin() );
					return aabb;
				}
				else
				{
					return mRelativeAABB;
				}
			}

			public void Translate( Vector2 v )
			{
				mRelativeAABB.Translate( v );
			}

			public void ValidateParent( Widget parent )
			{
				if( mParent == null )
				{
					XUtils.Assert( parent == null );
				}
				else
				{
					XUtils.Assert( Widget.CompareWidgets( parent )( this.mParent ) );
				}
			}

			private void Init( Widget parent, ePlacement placement )
			{
				mParent = parent;
				mPlacement = placement;
			}

			private Style GetStyle()
			{
				return (mParent != null) ? mParent.GetStyle() : XUI.Instance().GetStyle( eStyle.Screen );
			}

			private void PlaceCentered( Vector2 size )
			{
				Vector2 parent_aabb_size = mParent.GetPosition().GetRelatveAABB().GetSize();
				Vector2 parent_center = 0.5f * parent_aabb_size;
				Vector2 half_size = 0.5f * size;
				mRelativeAABB = new xAABB2( parent_center - half_size, parent_center + half_size );
			}

			private void PlaceCenteredBottom( Vector2 size )
			{
				Style style = GetStyle();
				Vector2 parent_aabb_size = mParent.GetPosition().GetRelatveAABB().GetSize();
				Vector2 parent_center_bottom = new Vector2( parent_aabb_size.X * 0.5f, parent_aabb_size.Y );
				Vector2 vertical_size = new Vector2( 0, size.Y );
				Vector2 horizontal_size = new Vector2( size.X, 0 );

				Vector2 top_left =  parent_center_bottom -
									vertical_size -
									style.mPlacementPadding * Vector2.UnitY -
									0.5f * horizontal_size;

				mRelativeAABB = new xAABB2( top_left, top_left + size );		
			}
		}
	}
}
