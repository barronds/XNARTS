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
		public class VerticalStack : Panel
		{
			// inheriting class should:
			// - create and assemble widgets individually outside this class
			// - assmeble self (here)
			// - place self (panel)
			// - place widgets individually outside this class using aabbs from GetRelativePlacement()
			// - TODO: unify place() if possible so that we can generically call it within here.

			xAABB2[] mRelativePlacements;

			public VerticalStack()
			{
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

				Vector2 size = CalcPlacements( widgets, style );
				base.AssemblePanel( size );
			}

			private Vector2 CalcPlacements( Widget[] widgets, Style style )
			{
				// calculate own aabb from already sized widgets
				int num = widgets.Count();
				XUtils.Assert( num > 0 );
				float vertical_sum = 0.0f;
				float horizontal_max = 0.0f;

				for ( int i = 0; i < num; ++i )
				{
					Vector2 size = widgets[ i ].GetAssembledSize();
					vertical_sum += size.Y;

					if ( size.X > horizontal_max )
					{
						horizontal_max = size.X;
					}
				}

				float total_y = (num + 1) * style.mPackingPadding + vertical_sum;
				float total_x = horizontal_max + 2 * style.mPackingPadding;
				float center_x = 0.5f * total_x;
				float y_cursor = style.mPackingPadding;

				mRelativePlacements = new xAABB2[ widgets.Count() ];

				for ( int i = 0; i < num; ++i )
				{
					// center justification, could add more options later (left, right)
					Vector2 size = widgets[ i ].GetAssembledSize();
					Vector2 top_left = new Vector2( center_x - 0.5f * size.X, y_cursor );
					xAABB2 relative = new xAABB2( top_left, top_left + size );
					mRelativePlacements[ i ] = relative;
					y_cursor += style.mPackingPadding + size.Y;
				}

				// return the size
				return new Vector2( total_x, total_y );
			}
		}
	}
}
