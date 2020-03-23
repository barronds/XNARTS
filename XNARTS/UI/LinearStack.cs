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
			Vector2 mDir;
			Vector2 mPerp;

			public LinearStack( eDirection direction )
			{
				// direction is x or y
				mDir = (direction == eDirection.Horizontal) ? Vector2.UnitX : Vector2.UnitY;
				mPerp = (direction == eDirection.Horizontal) ? Vector2.UnitY : Vector2.UnitX;
			}

			public xAABB2 GetRelativePlacement( int i )
			{
				XUtils.Assert( IsAssembled() );
				return mRelativePlacements[ i ];
			}

			public void AssembleLinearStack( Widget[] widgets, Style style )
			{
				for( int i = 0; i < widgets.Count(); ++i )
				{
					AddChild( widgets[ i ] );
				}

				mStyle = style;
				Vector2 size = CalcPackedPlacements();
				base.AssemblePanel( size );
			}

			// size of one or more children is assumed to have changed, re-pack normally and resize this widget.
			public void ReassembleLinearStack()
			{
				Vector2 size = CalcPackedPlacements();
				ReassemblePanel( size );
			}

			// dictate size of this widget, children will be equally spaced inside but edge ones
			// will be up against the edge according to the style's padding.  A single child would be centered.
			public void ReassembleLinearStack( Vector2 size )
			{
				CalcSpacedPlacements( size );
				ReassemblePanel( size );
			}

			public Vector2 GetDir()
			{
				return mDir;
			}

			public Vector2 GetPerp()
			{
				return mPerp;
			}

			private void CalcSpacedPlacements( Vector2 size )
			{
				int n = GetNumChildren();
				float total_dir = Vector2.Dot( size, mDir );
				float total_perp = Vector2.Dot( size, mPerp );
				float total_child_dir = 0.0f;

				for ( int i = 0; i < n; ++i )
				{
					total_child_dir += Vector2.Dot( GetChild( i ).GetAssembledSize(), mDir );
				}

				if ( n == 1 )
				{
					Vector2 child_size =  GetChild( 0 ).GetAssembledSize();
					float child_perp = Vector2.Dot(child_size, mPerp );

					Vector2 top_left =  0.5f * (total_dir - total_child_dir) * mDir +
										0.5f * (total_perp - child_perp) * mPerp;

					mRelativePlacements[ 0 ].Set( top_left, top_left + child_size );
					return;
				}

				int last_child = n - 1;
				Vector2 dir_space = mStyle.mPackingPadding * mDir;
				Vector2 first_child_size = GetChild( 0 ).GetAssembledSize();
				Vector2 last_child_size = GetChild( last_child ).GetAssembledSize();

				float first_child_perp_pad = 0.5f * (total_perp - Vector2.Dot( first_child_size, mPerp ));
				Vector2 first_child_top_left = dir_space + first_child_perp_pad * mPerp;

				float last_child_perp_pad = 0.5f * (total_perp - Vector2.Dot( last_child_size, mPerp ));
				Vector2 last_child_bottom_right = size - dir_space - last_child_perp_pad * mPerp;

				mRelativePlacements[ 0 ].Set( first_child_top_left, first_child_top_left + first_child_size );
				mRelativePlacements[ last_child ].Set( last_child_bottom_right - last_child_size, last_child_bottom_right );

				if( n == 2 )
				{
					return;
				}

				float remaining_dir =   total_dir -
										Vector2.Dot( first_child_size, mDir ) -
										Vector2.Dot( last_child_size, mDir ) -
										2.0f * mStyle.mPackingPadding;

				float remaining_child_dir = total_child_dir -
											Vector2.Dot( first_child_size, mDir ) -
											Vector2.Dot( last_child_size, mDir );

				float final_pad_size = (remaining_dir - remaining_child_dir) / (n - 1);
				Vector2 remaining_dir_pad = final_pad_size * mDir;
				float dir_cursor = Vector2.Dot( mRelativePlacements[ 0 ].GetMax(), mDir );

				for( int i = 1; i < last_child; ++i )
				{
					Vector2 child_size = GetChild( i ).GetAssembledSize();

					Vector2 top_left =  dir_cursor * mDir +
										remaining_dir_pad +
										0.5f * (total_perp - Vector2.Dot( child_size, mPerp )) * mPerp;

					mRelativePlacements[ i ].Set( top_left, top_left + child_size );
					dir_cursor += final_pad_size + Vector2.Dot( child_size, mDir );
				}
			}

			private Vector2 CalcPackedPlacements()
			{
				// calculate own aabb from already sized widgets
				int num = GetNumChildren();
				XUtils.Assert( num > 0 );
				float dir_sum = 0.0f;
				float perpendicular_max = 0.0f;

				for ( int i = 0; i < num; ++i )
				{
					Vector2 size = GetChild( i ).GetAssembledSize();
					dir_sum += Vector2.Dot( mDir, size );
					float perp_size = Vector2.Dot( mPerp, size );

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
					
					Vector2 top_left =	center_perp * mPerp - 
										(0.5f * Vector2.Dot( size, mPerp )) * mPerp + 
										dir_cursor * mDir;
					
					xAABB2 relative = new xAABB2( top_left, top_left + size );
					mRelativePlacements[ i ] = relative;
					dir_cursor += mStyle.mPackingPadding + Vector2.Dot( mDir, size );
				}

				// return the size
				return total_perp * mPerp + total_dir * mDir;
			}
		}
	}
}
