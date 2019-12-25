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
		public class VerticalStack : Panel
		{
			public VerticalStack()
			{
				// call InitVerticalStack afterwards
			}

			public void InitVerticalStack(	Widget parent, Widget[] widgets, Style style, ePlacement placement, 
											eInitialState state )
			{
				Vector2 size = Init_PlaceWidgets( parent, widgets, style );
				Place( parent, style, size, placement, state );
				AddAndParentChildren( widgets );
			}

			public void InitVerticalStack( Widget parent, Widget[] widgets, Style style, Vector2 pos, eInitialState state )
			{
				Vector2 size = Init_PlaceWidgets( parent, widgets, style );
				Place( parent, style, new xAABB2( pos, pos + size ), state );
				AddAndParentChildren( widgets );
			}

			private Vector2 Init_PlaceWidgets( Widget parent, Widget[] widgets, Style style )
			{
				// calculate own aabb from already sized widgets
				int num = widgets.Count();
				XUtils.Assert( num > 0 );
				float vertical_sum = 0.0f;
				float horizontal_max = 0.0f;

				for ( int i = 0; i < num; ++i )
				{
					// XUtils.Assert( widgets[ i ].IsInitialized() );
					Vector2 size = widgets[ i ].GetPosition().GetRelatveAABB().GetSize();
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

				for ( int i = 0; i < num; ++i )
				{
					// center justification, could add more options later (left, right)
					Vector2 size = widgets[ i ].GetPosition().GetRelatveAABB().GetSize();
					Vector2 top_left = new Vector2( center_x - 0.5f * size.X, y_cursor );
					xAABB2 relative = new xAABB2( top_left, top_left + size );
					widgets[ i ].GetPosition().SetRelativeAABB( relative );
					y_cursor += style.mPackingPadding + size.Y;
				}

				// return the size of this widget
				return new Vector2( total_x, total_y );
			}

			private void AddAndParentChildren( Widget[] widgets )
			{
				for( int i = 0; i < widgets.Count(); ++i )
				{
					AddChild( widgets[ i ] );
					widgets[ i ].Reparent( this, widgets[ i ].GetPosition().GetRelatveAABB().GetMin() );
				}
			}
		}
	}
}
