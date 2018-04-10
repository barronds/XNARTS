using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace XNARTS
{
	public struct xMapCell
	{
		public Color mColor;
	}


	public class XWorld : XSingleton< XWorld >
	{
		private bool			mWorldRendered;
		private xCoord			mMapSize;
		private xMapCell [,]	mCells;


		// private constructor as per XSingleton
		private XWorld()
		{}


		public void Init()
		{
			mWorldRendered = false;
			mMapSize = new xCoord( 16, 9 );
			mCells = new xMapCell[ mMapSize.x, mMapSize.y ];
			Random rand = new Random();

			for( int x = 0; x < mMapSize.x; ++x )
			{
				for( int y = 0; y < mMapSize.y; ++y )
				{
					mCells[ x, y ].mColor = new Color( (float)(rand.NextDouble()), (float)(rand.NextDouble()), (float)(rand.NextDouble()) );
				}
			}
		}


		public xCoord GetMapSize()
		{
			return mMapSize;
		}


		public xMapCell GetMapCell( xCoord coord )
		{
			XUtils.Assert( coord.x >= 0 && coord.y >= 0 && coord.x < mMapSize.x && coord.y < mMapSize.y );
			return mCells[ coord.x, coord.y ];
		}


		public void RenderWorldLines( GameTime game_time )
		{
			XSimpleDraw simple_draw_world = XSimpleDraw.Instance( xeSimpleDrawType.World_Transient );

			Vector3 start = new Vector3();
			Vector3 end = new Vector3();

			// each cell is 1 unit on edge in world space
			start.Y = 0;
			end.Y = mMapSize.y;

			for( int x = 0; x <= mMapSize.x; ++x )
			{
				start.X = x;
				end.X = x;

				simple_draw_world.DrawLine( start, end, Color.Yellow, Color.Black );
			}

			start.X = 0;
			end.X = mMapSize.x;

			for( int y = 0; y <= mMapSize.y; ++y )
			{
				start.Y = y;
				end.Y = y;

				simple_draw_world.DrawLine( start, end, Color.DarkGreen, Color.White );
			}
		}


		public void RenderWorld( GameTime game_time )
		{
			if( !mWorldRendered )
			{
				XSimpleDraw simple_draw_world = XSimpleDraw.Instance( xeSimpleDrawType.World_Persistent );
				System.Random rand = new Random();

				for( int x = 0; x < mMapSize.x; ++x )
				{
					for( int y = 0; y < mMapSize.y; ++y )
					{
						Vector3 low = new Vector3( x, y, 0f );
						Vector3 high = new Vector3( x + 1, y + 1, 0f );
						float r = (float)rand.NextDouble();
						float g = (float)rand.NextDouble();
						float b = (float)rand.NextDouble();
						Color color = new Color( r, g, b );

						simple_draw_world.DrawQuad( low, high, color );
					}
				}

				mWorldRendered = true;
			}
		}
	}
}
