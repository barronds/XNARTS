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
		public class LinearStack : Panel
		{
			// inheriting class should:
			// - create and assemble widgets individually outside this class
			// - assmeble self (here)
			// - place self (panel)
			// - place widgets individually outside this class using aabbs from GetRelativePlacement()
			// - TODO: unify place() if possible so that we can generically call it within here.

			xAABB2[] mRelativePlacements;
			Style mStyle;
			eDirection mDirection;

			public enum eDirection
			{
				Horizontal,
				Vertical
			}

			public LinearStack( eDirection direction )
			{
				mDirection = direction;
			}

			public xAABB2 GetRelativePlacement( int i )
			{
				XUtils.Assert( IsAssembled() );
				return mRelativePlacements[ i ];
			}

			public void AssembleVerticalStack( Widget[] widgets, Style style )
			{
				for( int i = 0; i < widgets.Count(); ++i )
				{
					AddChild( widgets[ i ] );
				}

				mStyle = style;
				Vector2 size = CalcPlacements();
				base.AssemblePanel( size );
			}

			public void ReassembleVerticalStack()
			{
				Vector2 size = CalcPlacements();
				ReassemblePanel( size );
			}

			private Vector2 CalcPlacements()
			{
				// direction is x or y
				Vector2 dir = (mDirection == eDirection.Horizontal) ? Vector2.UnitX : Vector2.UnitY;
				Vector2 perp = (mDirection == eDirection.Horizontal) ? Vector2.UnitY : Vector2.UnitX;

				// calculate own aabb from already sized widgets
				int num = GetNumChildren();
				XUtils.Assert( num > 0 );
				float dir_sum = 0.0f;
				float perpendicular_max = 0.0f;

				for ( int i = 0; i < num; ++i )
				{
					Vector2 size = GetChild( i ).GetAssembledSize();
					dir_sum += Vector2.Dot( dir, size );
					float perp_size = Vector2.Dot( perp, size );

					if ( perp_size > perpendicular_max )
					{
						perpendicular_max = perp_size;
					}
				}

				float total_dir = (num + 1) * mStyle.mPackingPadding + dir_sum;
				float total_perp = perpendicular_max + 2 * mStyle.mPackingPadding;
				float center_perp = 0.5f * total_perp;
				float dir_cursor = mStyle.mPackingPadding;

				mRelativePlacements = new xAABB2[ GetNumChildren() ];

				for ( int i = 0; i < num; ++i )
				{
					// center justification, could add more options later (left, right)
					Vector2 size = GetChild( i ).GetAssembledSize();
					
					Vector2 top_left =	center_perp * perp - 
										(0.5f * Vector2.Dot( size, perp )) * perp + 
										dir_cursor * dir;
					
					xAABB2 relative = new xAABB2( top_left, top_left + size );
					mRelativePlacements[ i ] = relative;
					dir_cursor += mStyle.mPackingPadding + Vector2.Dot( dir, size );
				}

				// return the size
				return total_perp * perp + total_dir * dir;
			}
		}
	}
}
