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
		Snow,

		Num
	}


	public struct xMapCell
	{
		public xeTerrainType    mTerrain;
		public Color			mColor;
		public Color            mBlendedColor;
	}


	public class XWorld : XSingleton< XWorld >
	{
		public class WorldRegenerated
		{ }

		public XBroadcaster< WorldRegenerated >	mBroadcaster_WorldRegenerated { get; }
		private bool							mWorldRendered;
		private SafeGrid< xMapCell >			mMap;
		private XListener< XKeyInput.KeyUp >    mListenter_KeyUp;


		// maybe promote this to utility class
		private class SafeGrid< T > where T : struct
		{
			public xCoord mBounds;
			public T [,] mData;

			public delegate void GridFilter( SafeGrid< T > grid, int x, int y );

			public void Init( xCoord bounds, T init_value )
			{
				XUtils.Assert( bounds.x > 0 && bounds.y > 0 );
				mData = new T[ bounds.x, bounds.y ];
				mBounds = bounds;

				Iterate( ( grid, x, y ) => { grid.mData[ x, y ] = init_value; } );
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
		{
			mBroadcaster_WorldRegenerated = new XBroadcaster< WorldRegenerated >();
		}


		public void Init()
		{
			mWorldRendered = false;

			mListenter_KeyUp = new XListener<XKeyInput.KeyUp>( 1, eEventQueueFullBehaviour.Ignore );
			XKeyInput.Instance().mBroadcaster_KeyUp.Subscribe( mListenter_KeyUp );

			xCoord map_size = new xCoord( 320, 180 );
			mMap = new SafeGrid< xMapCell >();
			xMapCell init_val = new xMapCell();
			init_val.mTerrain = xeTerrainType.Invalid;
			init_val.mColor = new Color();
			mMap.Init( map_size, init_val );
			Generate();
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
			XSimpleDraw simple_draw_world = XSimpleDraw.Instance( xeSimpleDrawType.WorldSpace_Transient );

			Vector3 start = new Vector3();
			Vector3 end = new Vector3();

			// each cell is 1 unit on edge in world space
			start.Y = 0;
			end.Y = mMap.mBounds.y;

			for ( int x = 0; x <= mMap.mBounds.x; ++x )
			{
				start.X = x;
				end.X = x;

				simple_draw_world.DrawLine( start, end, Color.Yellow, Color.Black );
			}

			start.X = 0;
			end.X = mMap.mBounds.x;

			for ( int y = 0; y <= mMap.mBounds.y; ++y )
			{
				start.Y = y;
				end.Y = y;

				simple_draw_world.DrawLine( start, end, Color.DarkGreen, Color.White );
			}
		}
		public void RenderWorld( GameTime game_time )
		{
			if ( mListenter_KeyUp.GetNumEvents() > 0 )
			{
				XKeyInput.KeyUp msg = mListenter_KeyUp.ReadNext();

				if ( msg.mKey == Microsoft.Xna.Framework.Input.Keys.W )
				{
					Console.WriteLine( "please generate world" );
					XSimpleDraw simple_draw = XSimpleDraw.Instance( xeSimpleDrawType.WorldSpace_Persistent_Map );
					simple_draw.CancelPrimitives();
					mWorldRendered = false;
					Generate();

					WorldRegenerated world_regenerated = new WorldRegenerated();
					mBroadcaster_WorldRegenerated.Post( world_regenerated );
				}
			}

			if ( !mWorldRendered )
			{
				XSimpleDraw simple_draw = XSimpleDraw.Instance( xeSimpleDrawType.WorldSpace_Persistent_Map );
				System.Random rand = new Random();

				mMap.Iterate( ( grid, x, y ) =>
				{
					Vector3 low = new Vector3( x, y, 0f );
					Vector3 high = new Vector3( x + 1, y + 1, 0f );
					Color color = grid.mData[ x, y ].mColor;

					simple_draw.DrawQuad( low, high, color );
				} );

				mWorldRendered = true;
			}
		}

		private Vector3 ColorToVector3( Color c )
		{
			return c.ToVector3();
		}
		private Color Vector3ToColor( Vector3 v )
		{
			return new Color( v.X, v.Y, v.Z );
		}


		private void Generate_Physical()
		{
			Random rand = new Random();

			// tuning
			const double    k_spike_density         = 0.04d;
			const double    k_spike_height          = 300d;
			const double    k_spike_variance        = 0.6d;
			const double    k_min_normalized_height = 0.0d;
			const double    k_max_normalized_height = 1.0d;
			const int       k_smoothing_passes      = 200; // choose even, more efficient
			const double    k_smoothing_scalar      = 0.5d;

			// tuning derrivatives
			double min_spike_height = k_spike_height * (1d - k_spike_variance);
			double max_spike_height = k_spike_height * (1d + k_spike_variance);
			double spike_height_spread = max_spike_height - min_spike_height;
			int num_spikes = (int)(k_spike_density * mMap.mBounds.x * mMap.mBounds.y);
			SafeGrid< double >[] heights = new SafeGrid< double >[ 2 ];

			for ( int h = 0; h < 2; ++h )
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
			for ( int i = 0; i < k_smoothing_passes; ++i )
			{
				int target = n == 0 ? 1 : 0;

				heights[ n ].Iterate( ( grid, x, y ) =>
				{
					double lo_x = grid.GetValueSafe( x - 1, y );
					double hi_x = grid.GetValueSafe( x + 1, y );
					double lo_y = grid.GetValueSafe( x, y - 1 );
					double hi_y = grid.GetValueSafe( x, y + 1 );
					double here = grid.mData[ x, y ];
					double blended = 0.25d * (lo_x + lo_y + hi_x + hi_y);
					double result = k_smoothing_scalar * here + (1d - k_smoothing_scalar) * blended;
					heights[ target ].mData[ x, y ] = result;
				} );

				n = n == 0 ? 1 : 0;
			}

			// normalize
			double max_height = 0.0d;

			heights[ 0 ].Iterate( ( grid, x, y ) =>
			{
				if( grid.mData[ x, y ] > max_height )
				{
					max_height = grid.mData[ x, y ];
				}
			} );

			double normalizer = max_height > 0.0d ? 1d / max_height : 1d;

			heights[ 0 ].Iterate( ( grid, x, y ) =>
			{
				grid.mData[ x, y ] *= normalizer;
			} );

			// height capping
			heights[ 0 ].Iterate( ( grid, x, y ) =>
			{
				grid.mData[ x, y ] = XMath.Clamp( grid.mData[ x, y ], k_min_normalized_height, k_max_normalized_height );
			} );

			double[] height_thresh = new double[ (int)xeTerrainType.Num - 1 ];
			height_thresh[ (int)xeTerrainType.DeepWater ] = 0.5d;
			height_thresh[ (int)xeTerrainType.ShallowWater ] = 0.6d;
			height_thresh[ (int)xeTerrainType.Sand ] = 0.65d;
			height_thresh[ (int)xeTerrainType.Grassland ] = 0.7d;
			height_thresh[ (int)xeTerrainType.Forest ] = 0.8d;
			height_thresh[ (int)xeTerrainType.Rock ] = 0.9d;

			mMap.Iterate( ( grid, x, y ) =>
			{
				xeTerrainType terrain = xeTerrainType.Snow;

				for ( int t = 0; t < (int)xeTerrainType.Num - 1; ++t )
				{
					if ( heights[ 0 ].mData[ x, y ] < height_thresh[ t ] )
					{
						terrain = (xeTerrainType)t;
						break;
					}
				}

				grid.mData[ x, y ].mTerrain = terrain;
			} );
		}


		private void Generate_AssignTerrainColors()
		{
			Color[] terrain_colors = new Color[ (int)xeTerrainType.Num ];
			terrain_colors[ (int)xeTerrainType.DeepWater ] = new Color( 0.25f, 0.35f, 0.6f );
			terrain_colors[ (int)xeTerrainType.ShallowWater ] = new Color( 0.3f, 0.5f, 0.75f );
			terrain_colors[ (int)xeTerrainType.Sand ] = new Color( 0.75f, 0.7f, 0.3f );
			terrain_colors[ (int)xeTerrainType.Grassland ] = new Color( 0.4f, 0.6f, 0.3f );
			terrain_colors[ (int)xeTerrainType.Forest ] = new Color( 0.15f, 0.45f, 0.3f );
			terrain_colors[ (int)xeTerrainType.Rock ] = new Color( 0.5f, 0.5f, 0.5f );
			terrain_colors[ (int)xeTerrainType.Snow ] = new Color( 0.9f, 0.9f, 0.9f );

			mMap.Iterate( ( grid, x, y ) =>
			{
				grid.mData[ x, y ].mColor = terrain_colors[ (int)(grid.mData[ x, y ].mTerrain) ];
			} );
		}


		private void Generate_BlendColors()
		{
			const int k_num_color_blend_passes = 0; // 0 is disabled
			const float k_here_weight = 0.5f;
			const float k_diagonal_weight = 0.707f;
			const float k_perpendicular_weight = 1f;
			const float k_total_weight = 4 * (k_diagonal_weight + k_perpendicular_weight);

			for ( int i = 0; i < k_num_color_blend_passes; ++i )
			{
				mMap.Iterate( ( grid, x, y ) =>
				{
					Color color_here = grid.mData[ x, y ].mColor;

					Color color_lo_x = grid.GetValueSafe( x - 1, y ).mColor;
					Color color_hi_x = grid.GetValueSafe( x + 1, y ).mColor;
					Color color_lo_y = grid.GetValueSafe( x, y - 1 ).mColor;
					Color color_hi_y = grid.GetValueSafe( x, y + 1 ).mColor;

					Color color_lo_x_lo_y = grid.GetValueSafe( x - 1, y - 1 ).mColor;
					Color color_lo_x_hi_y = grid.GetValueSafe( x - 1, y + 1 ).mColor;
					Color color_hi_x_lo_y = grid.GetValueSafe( x + 1, y - 1 ).mColor;
					Color color_hi_x_hi_y = grid.GetValueSafe( x + 1, y + 1 ).mColor;

					Vector3 v_here = ColorToVector3( color_here );

					Vector3 v_lo_x = ColorToVector3( color_lo_x );
					Vector3 v_hi_x = ColorToVector3( color_hi_x );
					Vector3 v_lo_y = ColorToVector3( color_lo_y );
					Vector3 v_hi_y = ColorToVector3( color_hi_y );

					Vector3 v_lo_x_lo_y = ColorToVector3( color_lo_x_lo_y );
					Vector3 v_lo_x_hi_y = ColorToVector3( color_lo_x_hi_y );
					Vector3 v_hi_x_lo_y = ColorToVector3( color_hi_x_lo_y );
					Vector3 v_hi_x_hi_y = ColorToVector3( color_hi_x_hi_y );

					// perpendicular weight 1, diagonal weight 0.707.  total weight 4(1.707)
					Vector3 neighbour_sum = k_perpendicular_weight * v_lo_x +
											k_perpendicular_weight * v_hi_x +
											k_perpendicular_weight * v_lo_y +
											k_perpendicular_weight * v_hi_y +
											k_diagonal_weight * v_lo_x_lo_y +
											k_diagonal_weight * v_lo_x_hi_y +
											k_diagonal_weight * v_hi_x_lo_y +
											k_diagonal_weight * v_hi_x_hi_y;

					Vector3 blended_color = k_here_weight * v_here + (1f - k_here_weight) * (1f / k_total_weight) * neighbour_sum;
					grid.mData[ x, y ].mBlendedColor = Vector3ToColor( blended_color );
				} );

				mMap.Iterate( ( grid, x, y ) =>
				{
					grid.mData[ x, y ].mColor = grid.mData[ x, y ].mBlendedColor;
				} );
			}
		}


		private void Generate_ColorLerp()
		{
			const bool k_do_color_lerp = true;
			const float k_lerp_fraction = 0.3f;
			Color lerp_color = new Color( 0.5f, 0.5f, 0.5f );

			if ( k_do_color_lerp )
			{
				mMap.Iterate( ( grid, x, y ) =>
				{
					grid.mData[ x, y ].mColor = Color.Lerp( grid.mData[ x, y ].mColor, lerp_color, k_lerp_fraction );
				} );
			}
		}


		private void Generate_Checkerboard()
		{
			const bool k_do_checkerboard = true;
			const float k_checkerboard_scalar = 0.985f;

			if ( k_do_checkerboard )
			{
				mMap.Iterate( ( grid, x, y ) =>
				{
					if ( ((x + y) % 2) == 0 )
					{
						grid.mData[ x, y ].mColor = Color.Multiply( grid.mData[ x, y ].mColor, k_checkerboard_scalar );
					}
				} );
			}
		}


		private void Generate()
		{
			Generate_Physical();
			Generate_AssignTerrainColors();
			Generate_BlendColors();
			Generate_ColorLerp();
			Generate_Checkerboard();
		}


	}
}
