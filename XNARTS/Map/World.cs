using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace XNARTS
{
	public enum xeTerrainType
	{
		Invalid = -1,
		DeepWater,
		ShallowWater,
		Sand,
		Grassland,
		Forest,
		Rock,
		Snow
	}


	public struct xMapCell
	{
		public xeTerrainType    mTerrain;
		public Color			mColor;
	}


	public class XWorld : XSingleton< XWorld >
	{
		private bool					mWorldRendered;
		private SafeGrid< xMapCell >    mMap;

		// maybe promote this to utility class
		private class SafeGrid< T > where T : struct
		{
			public xCoord mBounds;
			public T [,] mData;

			public delegate void GridFilter( SafeGrid<T> grid, int x, int y );

			public void Init( xCoord bounds, T init_value )
			{
				XUtils.Assert( bounds.x > 0 && bounds.y > 0 );
				mData = new T[ bounds.x, bounds.y ];
				mBounds = bounds;

				// would like a map function with lambda or something here
				for( int x = 0; x < bounds.x; ++x )
				{
					for( int y = 0; y < bounds.y; ++y )
					{
						mData[ x, y ] = init_value;
					}
				}
			}

			public T GetValueSafe( int x, int y )
			{
				x = (x < 0) ? 0 : (x >= mBounds.x) ? (mBounds.x - 1) : x;
				y = (y < 0) ? 0 : (y >= mBounds.y) ? (mBounds.y - 1) : y;
				return mData[ x, y ];
			}

			public void Iterate( GridFilter filter )
			{
				for( int x = 0; x < mBounds.x; ++x )
				{
					for( int y = 0; y < mBounds.y; ++y )
					{
						filter( this, x, y );
					}
				}
			}
		}

		// private constructor as per XSingleton
		private XWorld()
		{}


		public void Init()
		{
			mWorldRendered = false;
			xCoord map_size = new xCoord( 160, 90 );
			mMap = new SafeGrid< xMapCell >();
			xMapCell init_val = new xMapCell();
			init_val.mTerrain = xeTerrainType.Invalid;
			init_val.mColor = new Color();
			mMap.Init( map_size, init_val );
			Generate();
		}


		private void Generate()
		{
			Random rand = new Random();

			/*
			for ( int x = 0; x < mMapSize.x; ++x )
			{
				for ( int y = 0; y < mMapSize.y; ++y )
				{
					mCells[ x, y ].mTerrain = xeTerrainType.Sand;
					mCells[ x, y ].mColor = new Color( (float)(rand.NextDouble()),
														(float)(rand.NextDouble()),
														(float)(rand.NextDouble()) );
				}
			}
			*/

			// tuning
			const double	k_spike_density			= 0.01d;
			const double	k_spike_height			= 1000d;
			const double	k_spike_variance		= 0.6d;
			const int		k_smoothing_passes		= 50; // choose even, more efficient
			const double    k_smoothing_scalar		= 0.95d;

			// tuning derrivatives
			double min_spike_height = k_spike_height * (1d - k_spike_variance);
			double max_spike_height = k_spike_height * (1d + k_spike_variance);
			double spike_height_spread = max_spike_height - min_spike_height;
			int num_spikes = (int)(k_spike_density * mMap.mBounds.x * mMap.mBounds.y);
			SafeGrid< double >[] heights = new SafeGrid< double >[ 2 ];

			for( int h = 0; h < 2; ++h )
			{
				heights[ h ] = new SafeGrid<double>();
				heights[ h ].Init( mMap.mBounds, 0d );
			}

			// assign spikes
			for ( int i = 0; i < num_spikes; ++i )
			{
				int x = rand.Next() % mMap.mBounds.x;
				int y = rand.Next() % mMap.mBounds.y;

				double spike_height = min_spike_height + rand.NextDouble() * spike_height_spread;
				heights[ 0 ].mData[ x, y ] = spike_height;
			}

			// smooth
			int n = 0;
			for( int i = 0; i < k_smoothing_passes; ++i )
			{
				int target = n == 0 ? 1 : 0;

				for( int x = 0; x < mMap.mBounds.x; ++x )
				{
					for( int y = 0; y < mMap.mBounds.y;  ++y )
					{
						double lo_x = heights[ n ].GetValueSafe( x - 1, y );
						double hi_x = heights[ n ].GetValueSafe( x + 1, y );
						double lo_y = heights[ n ].GetValueSafe( x, y - 1 );
						double hi_y = heights[ n ].GetValueSafe( x, y + 1 );
						double here = heights[ n ].mData[ x, y ];
						double blended = 0.25d * (lo_x + lo_y + hi_x + hi_y);
						double result = k_smoothing_scalar * here + (1d - k_smoothing_scalar) * blended;
						heights[ target ].mData[ x, y ] = result;
					}
				}

				n = n == 0 ? 1 : 0;
			}

			// interpret

		}


		public xCoord GetMapSize()
		{
			return mMap.mBounds;
		}


		public xMapCell GetMapCell( xCoord coord )
		{
			XUtils.Assert( coord.x >= 0 && coord.y >= 0 && coord.x < mMap.mBounds.x && coord.y < mMap.mBounds.y );
			return mMap.mData[ coord.x, coord.y ];
		}


		public void RenderWorldLines( GameTime game_time )
		{
			XSimpleDraw simple_draw_world = XSimpleDraw.Instance( xeSimpleDrawType.World_Transient );

			Vector3 start = new Vector3();
			Vector3 end = new Vector3();

			// each cell is 1 unit on edge in world space
			start.Y = 0;
			end.Y = mMap.mBounds.y;

			for( int x = 0; x <= mMap.mBounds.x; ++x )
			{
				start.X = x;
				end.X = x;

				simple_draw_world.DrawLine( start, end, Color.Yellow, Color.Black );
			}

			start.X = 0;
			end.X = mMap.mBounds.x;

			for( int y = 0; y <= mMap.mBounds.y; ++y )
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

				for( int x = 0; x < mMap.mBounds.x; ++x )
				{
					for( int y = 0; y < mMap.mBounds.y; ++y )
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
