using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace XNARTS
{
	public struct tMapCell
	{
		public Color mColor;
	}


	public class World : Singleton< World >
	{
		private tCoord			mMapSize;
		private tMapCell [,]	mCells;
		SimpleDraw				mSimpleDraw;

		// private constructor as per Singleton
		private World()
		{}


		public void Init()
		{
			mSimpleDraw = XRenderManager.Instance().mSimpleDraw_World;
			mMapSize = new tCoord( 16, 9 );
			mCells = new tMapCell[ mMapSize.x, mMapSize.y ];
			Random rand = new Random();

			for( int x = 0; x < mMapSize.x; ++x )
			{
				for( int y = 0; y < mMapSize.y; ++y )
				{
					mCells[ x, y ].mColor = new Color( (float)(rand.NextDouble()), (float)(rand.NextDouble()), (float)(rand.NextDouble()) );
				}
			}
		}


		public tCoord GetMapSize()
		{
			return mMapSize;
		}


		public tMapCell GetMapCell( tCoord coord )
		{
			Utils.Assert( coord.x >= 0 && coord.y >= 0 && coord.x < mMapSize.x && coord.y < mMapSize.y );
			return mCells[ coord.x, coord.y ];
		}


		public void RenderWorld( GameTime game_time )
		{
			Vector3 start = new Vector3();
			Vector3 end = new Vector3();

			// each cell is 1 unit on edge in world space
			start.Y = 0;
			end.Y = mMapSize.y;

			for( int x = 0; x <= mMapSize.x; ++x )
			{
				start.X = x;
				end.X = x;

				mSimpleDraw.DrawLine( start, end, Color.Yellow, Color.Black );
			}

			start.X = 0;
			end.X = mMapSize.x;

			for( int y = 0; y <= mMapSize.y; ++y )
			{
				start.Y = y;
				end.Y = y;

				mSimpleDraw.DrawLine( start, end, Color.DarkGreen, Color.White );
			}
		}
	}
}
