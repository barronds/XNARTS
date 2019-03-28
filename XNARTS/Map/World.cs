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

	// maybe promote this to utility class
	public class XSafeGrid<T> where T : struct
	{
		public xCoord mBounds;
		public T [,] mData;

		public delegate void GridFilter( XSafeGrid<T> grid, int x, int y );
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
			for ( int x = 0; x < mBounds.x; ++x )
			{
				for ( int y = 0; y < mBounds.y; ++y )
				{
					filter( this, x, y );
				}
			}
		}
	}

	public class XWorld : XSingleton< XWorld >, XIBroadcaster< XWorld.WorldRegenerated >
	{
		public class WorldRegenerated
		{ }

		private XBroadcaster< WorldRegenerated >    mBroadcaster_WorldRegenerated;
		private bool								mRendered;
		private XWorldGen							mGen;
		private XWorldGen.Set						mGenSet;
		private int									mMapScale;
		private XWorldGen.eMapType					mMapType;
		private XSafeGrid< xMapCell >				mMap;
		private XListener< XKeyInput.KeyUp >		mListenter_KeyUp;
		private XListener< XUI.ButtonUpEvent >		mListener_Button;
		private XUI.IButton							mRegnerateMapButton;
		private XUI.IButton                         mMapTypeButton;
		private XUI.IButton                         mMapSizeButton;

		// private constructor as per XSingleton
		private XWorld()
		{
			mBroadcaster_WorldRegenerated = new XBroadcaster< WorldRegenerated >();
			mGen = new XWorldGen();
			mMapType = XWorldGen.eMapType.Default;
			mGenSet = mGen.GetTuningSet( mMapType );
			mMapScale = 2;
		}

		public void Init()
		{
			mRendered = false;

			mListenter_KeyUp = new XListener<XKeyInput.KeyUp>( 1, eEventQueueFullBehaviour.Ignore, "WorldKeyUp" );
			((XIBroadcaster<XKeyInput.KeyUp>)XKeyInput.Instance()).GetBroadcaster().Subscribe( mListenter_KeyUp );

			mListener_Button = new XListener<XUI.ButtonUpEvent>( 1, eEventQueueFullBehaviour.Ignore, "WorldButton" );
			((XIBroadcaster<XUI.ButtonUpEvent>)XUI.Instance()).GetBroadcaster().Subscribe( mListener_Button );
			XUI ui = XUI.Instance();

			mRegnerateMapButton = ui.CreateRectangularButton( new Vector2( 30, 30 ), "Regenerate Map", XUI.eStyle.GameplayUI );
			mMapTypeButton = ui.CreateRectangularButton( new Vector2( 30, 125 ), "Change Map Type", XUI.eStyle.GameplayUI );
			mMapSizeButton = ui.CreateRectangularButton( new Vector2( 30, 220 ), "Change Map Size", XUI.eStyle.GameplayUI );

			mMapSizeButton = ui.CreateRectangularButton( new Vector2( 30, 315 ),
				"Hy0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789", 
				XUI.eStyle.Frontend );

			String[] texts = { "Hello", "Goodbye", "Meatball Soup", "Four", "5" };
			ui.CreateSelector( new Vector2( 1000, 400 ), "missing", XUI.eStyle.Frontend, texts );

			Generate();
		}

		XBroadcaster< WorldRegenerated > XIBroadcaster< WorldRegenerated >.GetBroadcaster()
		{
			return mBroadcaster_WorldRegenerated;
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
			if ( !mRendered )
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

				mRendered = true;
			}
		}

		public void Update()
		{
			ProcessInput();
		}
		private void ProcessInput()
		{
			bool generate_map = false;
			bool resize_map = false;
			bool change_map_type = false;

			var key_enumerator = mListenter_KeyUp.GetEnumerator();
			XKeyInput.KeyUp key_msg = null;

			while( (key_msg = key_enumerator.MoveNext()) != null )
			{
				if ( key_msg.mKey == Microsoft.Xna.Framework.Input.Keys.W )
				{
					generate_map = true;
				}
				else if ( key_msg.mKey == Microsoft.Xna.Framework.Input.Keys.T )
				{
					change_map_type = true;
				}
				else if( key_msg.mKey == Microsoft.Xna.Framework.Input.Keys.S )
				{
					resize_map = true;
				}
			}

			var button_enumerator = mListener_Button.GetEnumerator();
			XUI.ButtonUpEvent button_msg = null;

			while( (button_msg = button_enumerator.MoveNext()) != null )
			{
				if ( button_msg.mID == mRegnerateMapButton.GetID() )
				{
					generate_map = true;
				}
				else if ( button_msg.mID == mMapTypeButton.GetID() )
				{
					change_map_type = true;
				}
				else if ( button_msg.mID == mMapSizeButton.GetID() )
				{
					resize_map = true;
				}
			}

			if ( change_map_type )
			{
				// loop through map types
				mMapType = (XWorldGen.eMapType)(((int)mMapType + 1) % (int)XWorldGen.eMapType.Num);
				mGenSet = mGen.GetTuningSet( mMapType );
				generate_map = true;
			}

			if( resize_map )
			{
				++mMapScale;
				generate_map = true;

				if ( mMapScale > mGen.GetMaxMapScale() )
				{
					mMapScale = 1;
				}
			}

			if ( generate_map )
			{
				XSimpleDraw simple_draw = XSimpleDraw.Instance( xeSimpleDrawType.WorldSpace_Persistent_Map );
				simple_draw.CancelPrimitives();
				mRendered = false;
				Generate();

				WorldRegenerated world_regenerated = new WorldRegenerated();
				mBroadcaster_WorldRegenerated.Post( world_regenerated );
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
			// set up the grid
			xCoord min_map_size = mGen.GetMinMapSize();
			xCoord map_size = mMapScale * min_map_size;
			mMap = new XSafeGrid<xMapCell>();
			xMapCell init_val = new xMapCell();
			init_val.mTerrain = xeTerrainType.Invalid;
			init_val.mColor = new Color();
			mMap.Init( map_size, init_val );

			Random rand = new Random();

			// tuning derrivatives
			double min_spike_height = mGenSet.mSpikeHeight * (1d - mGenSet.mSpikeVariance);
			double max_spike_height = mGenSet.mSpikeHeight * (1d + mGenSet.mSpikeVariance);
			double spike_height_spread = max_spike_height - min_spike_height;
			int num_spikes = (int)(mGenSet.mSpikeDensity * mMap.mBounds.x * mMap.mBounds.y);
			XSafeGrid< double >[] heights = new XSafeGrid< double >[ 2 ];

			for ( int h = 0; h < 2; ++h )
			{
				heights[ h ] = new XSafeGrid<double>();
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
			int smoothing_passes =	(mGenSet.mSmoothingPasses % 2) == 1	? 
									mGenSet.mSmoothingPasses + 1		: 
									mGenSet.mSmoothingPasses			;

			for ( int i = 0; i < smoothing_passes; ++i )
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
					double result = mGenSet.mSmoothingScalar * here + (1d - mGenSet.mSmoothingScalar) * blended;
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
				grid.mData[ x, y ] = XMath.Clamp( grid.mData[ x, y ], mGenSet.mMinNormalizedHeight, mGenSet.mMaxNormalizedHeight );
			} );

			mMap.Iterate( ( grid, x, y ) =>
			{
				xeTerrainType terrain = xeTerrainType.Snow;

				for ( int t = 0; t < (int)xeTerrainType.Num - 1; ++t )
				{
					if ( heights[ 0 ].mData[ x, y ] <= mGenSet.mHeightThresh[ t ] )
					{
						terrain = (xeTerrainType)t;
						break;
					}
				}

				grid.mData[ x, y ].mTerrain = terrain;
			} );

			mGenSet.mPostProcess( mMap );
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
			const float k_checkerboard_scalar = 0.94f; // 0.985f was good before Mar18, added font, maybe alpha premultiply

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
