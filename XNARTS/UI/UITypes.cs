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

		public class UIPosSpec
		{
			private xAABB2      mRelativeAABB;
			private Vector2     mSize;
			private ePlacement  mPlacement;
			private bool        mIsAbsolute;

			public UIPosSpec( ePlacement placement, Vector2 size )
			{
				XUtils.Assert( ((int)placement > (int)ePlacement.Invalid) && size.X > 0.0f && size.Y > 0.0f );
				Init( placement, size, xAABB2.GetOrigin(), false );
			}

			public UIPosSpec( xAABB2 relative_aabb )
			{
				XUtils.Assert( relative_aabb.IsNonDegenerate() );
				Init( ePlacement.Absolute, relative_aabb.GetSize(), relative_aabb, true );
			}

			public bool IsAbsolute()
			{
				return mIsAbsolute;
			}

			public ePlacement GetPlacement()
			{
				XUtils.Assert( !IsAbsolute() );
				return mPlacement;
			}

			public xAABB2 GetRelativeAABB()
			{
				XUtils.Assert( IsAbsolute() );
				return mRelativeAABB;
			}

			public Vector2 GetSize()
			{
				XUtils.Assert( !IsAbsolute() );
				return mSize;
			}

			private void Init( ePlacement p, Vector2 size, xAABB2 aabb, bool is_absolute )
			{
				mPlacement = p;
				mSize = size;
				mRelativeAABB = aabb;
				mIsAbsolute = is_absolute;
			}
		}

		public class UIPosition
		{
			private ePlacement	mPlacement;
			private xAABB2		mRelativeAABB;	// relative to parent's aabb min point
			private Widget      mParent;		// can be null, for screen widget

			public UIPosition( Widget parent, UIPosSpec spec )
			{
				if( spec.IsAbsolute() )
				{
					mRelativeAABB = spec.GetRelativeAABB();
					Init( parent, ePlacement.Absolute );
				}
				else
				{
					Init( parent, spec.GetPlacement() );
					Vector2 size = spec.GetSize();

					switch ( spec.GetPlacement() )
					{
						case ePlacement.Centered:		Place( size, 0.5f, 0.5f, -0.5f, -0.5f, 0.0f, 0.0f );	break;
						case ePlacement.CenteredLeft:	Place( size, 0.0f, 0.5f, 0.0f, -0.5f, 1.0f, 0.0f );		break;
						case ePlacement.CenteredRight:	Place( size, 1.0f, 0.5f, -1.0f, -0.5f, -1.0f, 0.0f );	break;
						case ePlacement.CenteredTop:	Place( size, 0.5f, 0.0f, -0.5f, 0.0f, 0.0f, 1.0f );		break;
						case ePlacement.CenteredBottom: Place( size, 0.5f, 1.0f, -0.5f, -1.0f, 0.0f, -1.0f );	break;
						case ePlacement.TopLeft:		Place( size, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f );		break;
						case ePlacement.TopRight:		Place( size, 1.0f, 0.0f, -1.0f, 0.0f, -1.0f, 1.0f );	break;
						case ePlacement.BottomLeft:		Place( size, 0.0f, 1.0f, 0.0f, -1.0f, 1.0f, -1.0f );	break;
						case ePlacement.BottomRight:	Place( size, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, -1.0f );	break;

						default:
							XUtils.Assert( false, "placement type not yet supported" );
							break;
					}
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
