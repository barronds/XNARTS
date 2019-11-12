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
			Absolute = -2, // Vector2, a position relative to parent widget
			Invalid = -1,

			Centered,
			CenteredLeft,
			CenteredRight,
			TopLeft,
			TopRight,
			BottomLeft,
			BottomRight,
			CenteredBottom,
			CenteredTop,

			Num
		}

		public class UIPosition
		{
			private ePlacement	mPlacement;
			private xAABB2		mRelativeAABB;	// relative to parent's aabb min point
			private Widget      mParent;		// can be null, for screen widget

			// constructor for absolute position relative to widget.  use screen widget for screen space position.
			public UIPosition( Widget parent, xAABB2 relative_aabb )
			{
				mRelativeAABB = relative_aabb;
				Init( parent, ePlacement.Absolute );
				ValidateAABB();
			}

			// constructor for placement relative to a widget.  use screen widget for screen placement.
			public UIPosition( Widget parent, ePlacement placement, Vector2 size )
			{
				XUtils.Assert( placement != ePlacement.Absolute, "wrong constructor for absolute" );
				Init( parent, placement );

				switch ( placement )
				{
					case ePlacement.Centered:			Place( size, 0.5f, 0.5f, -0.5f, -0.5f, 0.0f, 0.0f );	break;
					case ePlacement.CenteredLeft:		Place( size, 0.0f, 0.5f, 0.0f, -0.5f, 1.0f, 0.0f );		break;
					case ePlacement.CenteredRight:		Place( size, 1.0f, 0.5f, -1.0f, -0.5f, -1.0f, 0.0f );	break;
					case ePlacement.CenteredTop:		Place( size, 0.5f, 0.0f, -0.5f, 0.0f, 0.0f, 1.0f );		break;
					case ePlacement.CenteredBottom:		Place( size, 0.5f, 1.0f, -0.5f, -1.0f, 0.0f, -1.0f );	break;
					case ePlacement.TopLeft:			Place( size, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f );		break;
					case ePlacement.TopRight:			Place( size, 1.0f, 0.0f, -1.0f, 0.0f, -1.0f, 1.0f );	break;
					case ePlacement.BottomLeft:			Place( size, 0.0f, 1.0f, 0.0f, -1.0f, 1.0f, -1.0f );	break;
					case ePlacement.BottomRight:		Place( size, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, -1.0f );	break;

					default:
						XUtils.Assert( false, "placement type not yet supported" );
						break;
				}

				ValidateAABB();
			}

			public xAABB2 GetRelatveAABB()
			{
				return mRelativeAABB;
			}

			public void SetRelativeAABB( xAABB2 aabb )
			{
				mRelativeAABB = aabb;
				ValidateAABB();
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
				ValidateAABB();
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

			public ePlacement GetPlacement()
			{
				return mPlacement;
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

			private void Place(	Vector2 size,
								float parent_start_norm_x, 
								float parent_start_norm_y,
								float shape_correct_norm_x,
								float shape_correct_norm_y,
								float padding_norm_x,
								float padding_norm_y )
			{
				Vector2 parent_aabb_size = mParent.GetPosition().GetRelatveAABB().GetSize();
				Vector2 parent_start_point = new Vector2(	parent_start_norm_x * parent_aabb_size.X, 
															parent_start_norm_y * parent_aabb_size.Y );
				Style s = GetStyle();
				Vector2 padding = s.mPlacementPadding * new Vector2( padding_norm_x, padding_norm_y );
				Vector2 size_correct = new Vector2( size.X * shape_correct_norm_x, size.Y * shape_correct_norm_y );
				Vector2 top_left = parent_start_point + padding + size_correct;
				Vector2 bottom_right = top_left + size;
				mRelativeAABB = new xAABB2( top_left, bottom_right );
			}

			private void ValidateAABB()
			{
				if( mParent != null )
				{
					xAABB2 parent_aabb = new xAABB2( Vector2.Zero, mParent.GetPosition().GetRelatveAABB().GetSize() );
					XUtils.Assert( parent_aabb.Contains( mRelativeAABB.GetMin() ) && parent_aabb.Contains( mRelativeAABB.GetMax() ) );
				}
			}
		}
	}
}
