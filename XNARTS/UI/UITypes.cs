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

			Absolute, // Vector2
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
			private xAABB2		mAABB;
			private Widget      mParent;

			// constructor for absolute position relative to widget.  use screen widget for screen space position.
			public Position( Widget parent, xAABB2 aabb )
			{
				Init( parent, ePlacement.Absolute, aabb );
			}

			// constructor for placement relative to a widget.  use screen widget for screen placement.
			public Position( Widget parent, ePlacement placement )
			{
				// should maybe be calculating aabb here or force constructing code to pass it in
				XUtils.Assert( placement != ePlacement.Absolute, "wrong constructor for absolute" );
				Init( parent, placement, xAABB2.GetOrigin() );
			}

			public xAABB2 GetAABB()
			{
				return mAABB;
			}

			// maybe add methods for updating parent, or updating motion, updating aabb

			private void Init( Widget parent, ePlacement placement, xAABB2 aabb )
			{
				mAABB = aabb;
				mPlacement = placement;
				mParent = parent;
			}
		}
	}
}
