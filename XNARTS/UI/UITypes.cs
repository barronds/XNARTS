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
			private xAABB2		mAABB;	// relative to parent's aabb min point
			private Widget      mParent;

			// constructor for absolute position relative to widget.  use screen widget for screen space position.
			public Position( Widget parent, xAABB2 aabb )
			{
				mAABB = aabb;
				mPlacement = ePlacement.Absolute;
				mParent = parent;
			}

			// constructor for placement relative to a widget.  use screen widget for screen placement.
			public Position( Widget parent, ePlacement placement, Vector2 size )
			{
				XUtils.Assert( placement != ePlacement.Absolute, "wrong constructor for absolute" );
				mPlacement = placement;
				mParent = parent;

				switch ( placement )
				{
					case ePlacement.Centered:
						PlaceCentered( size );
						break;

					default:
						XUtils.Assert( false, "placement type not yet supported" );
						break;
				}
			}

			public xAABB2 GetAABB()
			{
				return mAABB;
			}

			public void Translate( Vector2 v )
			{
				mAABB.Translate( v );
			}

			private void PlaceCentered( Vector2 size )
			{
				xAABB2 parent_aabb = mParent.GetPosition().GetAABB();
				Vector2 parent_center = parent_aabb.GetCenter();
				Vector2 half_size = 0.5f * size;
				mAABB = new xAABB2( parent_center - half_size, parent_center + half_size );
			}
		}
	}
}
