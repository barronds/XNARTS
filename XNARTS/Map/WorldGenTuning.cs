using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNARTS
{
	public class XWorldGen
	{
		public enum eMapType
		{
			Invalid = -1,

			ScandinavianCoast,
			TropicalIslands,
			GulfIslands,
			ArcticIslands,
			NorthSeaIslands,
			Prairie,
			Badlands,
			Desert,
			DesertMountains,
			Tundra,
			NorthernForest,
			TemperateForest,
			TropicalForest,
			Mediterranean,

			Num,

			Default = NorthernForest
		}

		public delegate void dPostProcess( XSafeGrid<xMapCell> map );

		public class Set
		{
			public bool         mInitialized;
			public double       mSpikeDensity;
			public double       mSpikeHeight;
			public double       mSpikeVariance;
			public double       mMinNormalizedHeight;
			public double       mMaxNormalizedHeight;
			public int          mSmoothingPasses;
			public double       mSmoothingScalar;
			public dPostProcess mPostProcess;
			public double[]     mHeightThresh;
		}

		private Set[] mSets;

		public XWorldGen()
		{
			mSets = new Set[ (int)eMapType.Num ];

			for ( int i = 0; i < (int)eMapType.Num; ++i )
			{
				mSets[ i ] = new Set();
				mSets[ i ].mHeightThresh = new double[ (int)xeTerrainType.Num - 1 ];
				mSets[ i ].mInitialized = false;
			}

			Set s = mSets[ (int)eMapType.ScandinavianCoast ];

			s.mInitialized = true;
			s.mSpikeDensity = 0.04;
			s.mSpikeHeight = 300;
			s.mSpikeVariance = 0.6;
			s.mMinNormalizedHeight = 0;
			s.mMaxNormalizedHeight = 1;
			s.mSmoothingPasses = 200;
			s.mSmoothingScalar = 0.5;
			s.mPostProcess = PostProcess_ScandinavianCoast;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ] = 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ] = 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ] = 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ] = 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ] = 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ] = 0.9d;

			s = mSets[ (int)eMapType.ScandinavianCoast ];

			s.mInitialized = true;
			s.mSpikeDensity = 0.08;
			s.mSpikeHeight = 300;
			s.mSpikeVariance = 0.6;
			s.mMinNormalizedHeight = 0;
			s.mMaxNormalizedHeight = 1;
			s.mSmoothingPasses = 200;
			s.mSmoothingScalar = 0.5;
			s.mPostProcess = PostProcess_DoNothing;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ] = 0.58d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ] = 0.61d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ] = 0.64d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ] = 0.67d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ] = 0.70d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ] = 0.80d;

			s = mSets[ (int)eMapType.TropicalIslands ];

			s.mInitialized = true;
			s.mSpikeDensity = 0.04;
			s.mSpikeHeight = 300;
			s.mSpikeVariance = 0.6;
			s.mMinNormalizedHeight = 0;
			s.mMaxNormalizedHeight = 1;
			s.mSmoothingPasses = 200;
			s.mSmoothingScalar = 0.5;
			s.mPostProcess = PostProcess_TropicalIslands;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ] = 0.45d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ] = 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ] = 0.75d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ] = 0.75;
			s.mHeightThresh[ (int)xeTerrainType.Forest ] = 1;
			s.mHeightThresh[ (int)xeTerrainType.Rock ] = 1;

			s = mSets[ (int)eMapType.GulfIslands ];

			s.mInitialized = true;
			s.mSpikeDensity = 0.04;
			s.mSpikeHeight = 300;
			s.mSpikeVariance = 0.6;
			s.mMinNormalizedHeight = 0;
			s.mMaxNormalizedHeight = 1;
			s.mSmoothingPasses = 200;
			s.mSmoothingScalar = 0.5;
			s.mPostProcess = PostProcess_DoNothing;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ] = 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ] = 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ] = 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ] = 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ] = 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ] = 0.9d;

			s = mSets[ (int)eMapType.ArcticIslands ];

			s.mInitialized = true;
			s.mSpikeDensity = 0.04;
			s.mSpikeHeight = 300;
			s.mSpikeVariance = 0.6;
			s.mMinNormalizedHeight = 0;
			s.mMaxNormalizedHeight = 1;
			s.mSmoothingPasses = 200;
			s.mSmoothingScalar = 0.5;
			s.mPostProcess = PostProcess_ArcticIslands;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ] = 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ] = 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ] = 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ] = 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ] = 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ] = 0.7d;

			s = mSets[ (int)eMapType.NorthSeaIslands ];

			s.mInitialized = true;
			s.mSpikeDensity = 0.04;
			s.mSpikeHeight = 300;
			s.mSpikeVariance = 0.6;
			s.mMinNormalizedHeight = 0;
			s.mMaxNormalizedHeight = 1;
			s.mSmoothingPasses = 200;
			s.mSmoothingScalar = 0.5;
			s.mPostProcess = PostProcess_NorthSeaIslands;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ] = 0.56d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ] = 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ] = 0.63d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ] = 0.75d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ] = 0.82d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ] = 0.82d;

			s = mSets[ (int)eMapType.Prairie ];

			s.mInitialized = true;
			s.mSpikeDensity = 0.16;
			s.mSpikeHeight = 300;
			s.mSpikeVariance = 0.6;
			s.mMinNormalizedHeight = 0;
			s.mMaxNormalizedHeight = 1;
			s.mSmoothingPasses = 200;
			s.mSmoothingScalar = 0.5;
			s.mPostProcess = PostProcess_Prairie;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ] = 0.6;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ] = 0.65;
			s.mHeightThresh[ (int)xeTerrainType.Sand ] = 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ] = 0.75d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ] = 0.85d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ] = 0.9d;

			s = mSets[ (int)eMapType.Badlands ];

			s.mInitialized = true;
			s.mSpikeDensity = 0.14;
			s.mSpikeHeight = 300;
			s.mSpikeVariance = 0.6;
			s.mMinNormalizedHeight = 0;
			s.mMaxNormalizedHeight = 1;
			s.mSmoothingPasses = 200;
			s.mSmoothingScalar = 0.5;
			s.mPostProcess = PostProcess_Badlands;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ] = 0.6;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ] = 0.6;
			s.mHeightThresh[ (int)xeTerrainType.Sand ] = 0.6;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ] = 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ] = 0.75d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ] = 0.9d;

			s = mSets[ (int)eMapType.Desert ];

			s.mInitialized = true;
			s.mSpikeDensity = 0.15;
			s.mSpikeHeight = 300;
			s.mSpikeVariance = 0.6;
			s.mMinNormalizedHeight = 0;
			s.mMaxNormalizedHeight = 1;
			s.mSmoothingPasses = 200;
			s.mSmoothingScalar = 0.5;
			s.mPostProcess = PostProcess_Desert;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ] = 0.4d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ] = 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ] = 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ] = 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ] = 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ] = 0.9d;

			s = mSets[ (int)eMapType.DesertMountains ];

			s.mInitialized = true;
			s.mSpikeDensity = 0.04;
			s.mSpikeHeight = 300;
			s.mSpikeVariance = 0.6;
			s.mMinNormalizedHeight = 0;
			s.mMaxNormalizedHeight = 1;
			s.mSmoothingPasses = 200;
			s.mSmoothingScalar = 0.5;
			s.mPostProcess = PostProcess_DesertMountains;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ] = 0.4d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ] = 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ] = 0.55d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ] = 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ] = 0.75d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ] = 0.9d;

			s = mSets[ (int)eMapType.Tundra ];

			s.mInitialized = true;
			s.mSpikeDensity = 0.14;
			s.mSpikeHeight = 300;
			s.mSpikeVariance = 0.6;
			s.mMinNormalizedHeight = 0;
			s.mMaxNormalizedHeight = 1;
			s.mSmoothingPasses = 200;
			s.mSmoothingScalar = 0.5;
			s.mPostProcess = PostProcess_Tundra;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ] = 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ] = 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ] = 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ] = 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ] = 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ] = 0.9d;

			s = mSets[ (int)eMapType.NorthernForest ];

			s.mInitialized = true;
			s.mSpikeDensity = 0.24;
			s.mSpikeHeight = 300;
			s.mSpikeVariance = 0.6;
			s.mMinNormalizedHeight = 0;
			s.mMaxNormalizedHeight = 1;
			s.mSmoothingPasses = 200;
			s.mSmoothingScalar = 0.5;
			s.mPostProcess = PostProcess_NorthernForest;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ] = 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ] = 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ] = 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ] = 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ] = 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ] = 0.85d;

			s = mSets[ (int)eMapType.TemperateForest ];

			s.mInitialized = true;
			s.mSpikeDensity = 0.04;
			s.mSpikeHeight = 300;
			s.mSpikeVariance = 0.6;
			s.mMinNormalizedHeight = 0;
			s.mMaxNormalizedHeight = 1;
			s.mSmoothingPasses = 200;
			s.mSmoothingScalar = 0.5;
			s.mPostProcess = PostProcess_DoNothing;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ] = 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ] = 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ] = 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ] = 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ] = 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ] = 0.9d;

			s = mSets[ (int)eMapType.TropicalForest ];

			s.mInitialized = true;
			s.mSpikeDensity = 0.04;
			s.mSpikeHeight = 300;
			s.mSpikeVariance = 0.6;
			s.mMinNormalizedHeight = 0;
			s.mMaxNormalizedHeight = 1;
			s.mSmoothingPasses = 200;
			s.mSmoothingScalar = 0.5;
			s.mPostProcess = PostProcess_DoNothing;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ] = 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ] = 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ] = 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ] = 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ] = 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ] = 0.9d;

			s = mSets[ (int)eMapType.Mediterranean ];

			s.mInitialized = true;
			s.mSpikeDensity = 0.08;
			s.mSpikeHeight = 300;
			s.mSpikeVariance = 0.6;
			s.mMinNormalizedHeight = 0;
			s.mMaxNormalizedHeight = 1;
			s.mSmoothingPasses = 200;
			s.mSmoothingScalar = 0.5;
			s.mPostProcess = PostProcess_Mediterranean;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ] = 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ] = 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ] = 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ] = 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ] = 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ] = 1;
		}


		public Set GetTuningSet( eMapType map_type )
		{
			return mSets[ (int)map_type ];
		}
		public xCoord GetMinMapSize()
		{
			return new xCoord( 160, 90 );
		}
		public int GetMaxMapScale()
		{
			return 5;
		}

		private void PostProcess_DoNothing( XSafeGrid<xMapCell> map )
		{ }
		private void PostProcess_ArcticIslands( XSafeGrid<xMapCell> map )
		{
			// swap rock and snow
			map.Iterate( ( grid, x, y ) =>
			{
				xeTerrainType t = grid.mData[ x, y ].mTerrain;

				if ( t == xeTerrainType.Snow )
				{
					grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
				}
				else if ( t == xeTerrainType.Rock )
				{
					grid.mData[ x, y ].mTerrain = xeTerrainType.Snow;
				}
			} );
		}
		private void PostProcess_Mediterranean( XSafeGrid<xMapCell> map )
		{
			PostProcess_DoNothing( map );
		}
		private void PostProcess_NorthSeaIslands( XSafeGrid<xMapCell> map )
		{
			// rock - grass - rock - snow.  so sand and forest become rock
			map.Iterate( ( grid, x, y ) =>
			{
				xeTerrainType t = grid.mData[ x, y ].mTerrain;

				if ( t == xeTerrainType.Sand )
				{
					grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
				}
				else if ( t == xeTerrainType.Forest )
				{
					grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
				}
			} );
		}
		private void PostProcess_Prairie( XSafeGrid<xMapCell> map )
		{
			Random r = new Random();

			// deep - shallow
			// shallow - half trees with grass
			// sand - sparse trees with grass
			// grass - very sparse trees with grass
			// forest - grass
			// rock - grass with sprinkled rocks
			// snow - mostly rock
			map.Iterate( ( grid, x, y ) =>
			{
				xeTerrainType t = grid.mData[ x, y ].mTerrain;

				if ( t == xeTerrainType.DeepWater )
				{
					grid.mData[ x, y ].mTerrain = xeTerrainType.ShallowWater;
				}
				else if ( t == xeTerrainType.ShallowWater )
				{
					if ( r.NextDouble() < 0.5 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Grassland;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Forest;
					}
				}
				else if ( t == xeTerrainType.Sand )
				{
					if ( r.NextDouble() < 0.5 / 3 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Forest;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Grassland;
					}
				}
				else if ( t == xeTerrainType.Grassland )
				{
					if ( r.NextDouble() < 0.5 / 9 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Forest;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Grassland;
					}
				}
				else if ( t == xeTerrainType.Forest )
				{
					grid.mData[ x, y ].mTerrain = xeTerrainType.Grassland;
				}
				else if ( t == xeTerrainType.Rock )
				{
					if ( r.NextDouble() < 0.33 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Grassland;
					}
				}
				else if ( t == xeTerrainType.Snow )
				{
					if ( r.NextDouble() < 0.67 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Grassland;
					}
				}
			} );
		}
		private void PostProcess_TropicalIslands( XSafeGrid<xMapCell> map )
		{
			PostProcess_DoNothing( map );
		}
		private void PostProcess_Desert( XSafeGrid<xMapCell> map )
		{
			Random r = new Random();

			// deep - pure desert
			// shallow - extremely sparse grass and rocks
			// sand - very sparse grass and rocks
			map.Iterate( ( grid, x, y ) =>
			{
				xeTerrainType t = grid.mData[ x, y ].mTerrain;

				if ( t == xeTerrainType.DeepWater )
				{
					grid.mData[ x, y ].mTerrain = xeTerrainType.ShallowWater;
				}
				else if ( t == xeTerrainType.ShallowWater )
				{
					grid.mData[ x, y ].mTerrain = xeTerrainType.Sand;
				}
				else if ( t == xeTerrainType.Sand )
				{
					double d = r.NextDouble();

					if ( d < 0.01 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Grassland;
					}
					else if ( d < 0.02 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Sand;
					}
				}
				else if ( t == xeTerrainType.Grassland )
				{
					double d = r.NextDouble();

					if ( d < 0.02 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Grassland;
					}
					else if ( d < 0.04 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Sand;
					}
				}
				else if ( t == xeTerrainType.Forest )
				{
					double d = r.NextDouble();

					if ( d < 0.04 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Grassland;
					}
					else if ( d < 0.35 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Sand;
					}
				}
				else if ( t == xeTerrainType.Rock )
				{
					double d = r.NextDouble();

					if ( d < 0.03 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Grassland;
					}
					else if ( d < 0.7 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Sand;
					}
				}
				else if ( t == xeTerrainType.Snow )
				{
					grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
				}
			} );
		}
		private void PostProcess_Tundra( XSafeGrid<xMapCell> map )
		{
			Random r = new Random();

			// deep - water
			// shallow - snow
			// sand - sand, rock, snow
			// grassland - sand, rock, snow, grass
			// forest - rock, snow, grass
			// rock - rock, snow
			// snow - more snow, less rock
			map.Iterate( ( grid, x, y ) =>
			{
				xeTerrainType t = grid.mData[ x, y ].mTerrain;

				if ( t == xeTerrainType.DeepWater )
				{
					grid.mData[ x, y ].mTerrain = xeTerrainType.ShallowWater;
				}
				else if ( t == xeTerrainType.ShallowWater )
				{
					grid.mData[ x, y ].mTerrain = xeTerrainType.Snow;
				}
				else if ( t == xeTerrainType.Sand )
				{
					double d = r.NextDouble();

					if ( d < 0.75 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Snow;
					}
					else if ( d < 0.85 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Sand;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
					}
				}
				else if ( t == xeTerrainType.Grassland )
				{
					double d = r.NextDouble();

					if ( d < 0.1 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Sand;
					}
					else if ( d < 0.3 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
					}
					else if ( d < 0.8 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Snow;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Grassland;
					}
				}
				else if ( t == xeTerrainType.Forest )
				{
					double d = r.NextDouble();

					if ( d < 0.3 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Grassland;
					}
					else if ( d < 0.6 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Snow;
					}
				}
				else if ( t == xeTerrainType.Rock )
				{
					double d = r.NextDouble();

					if ( d < 0.6 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Snow;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
					}
				}
				else
				{
					double d = r.NextDouble();

					if ( d < 0.85 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Snow;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
					}
				}
			} );
		}
		private void PostProcess_DesertMountains( XSafeGrid<xMapCell> map )
		{
			Random r = new Random();

			map.Iterate( ( grid, x, y ) =>
			{
				xeTerrainType t = grid.mData[ x, y ].mTerrain;

				if ( t == xeTerrainType.DeepWater )
				{
					double d = r.NextDouble();

					if ( d < 0.5 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Grassland;
					}
					else if ( d < 0.9 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Sand;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Forest;
					}
				}
				else if ( t == xeTerrainType.ShallowWater )
				{
					double d = r.NextDouble();

					if ( d < 0.15 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
					}
					else if ( d < 0.6 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Grassland;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Sand;
					}
				}
				else if ( t == xeTerrainType.Sand )
				{
					double d = r.NextDouble();

					if ( d < 0.1 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Grassland;
					}
					else if ( d < 0.65 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Sand;
					}
				}
				else if ( t == xeTerrainType.Grassland )
				{
					double d = r.NextDouble();

					if ( d < 0.5 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Sand;
					}
				}
				else if ( t == xeTerrainType.Forest )
				{
					double d = r.NextDouble();

					if ( d < 0.7 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Sand;
					}
				}
				else if ( t == xeTerrainType.Rock )
				{
					double d = r.NextDouble();

					if ( d < 0.2 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Snow;
					}
					else if ( d < 0.8 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Sand;
					}
				}
				else if ( t == xeTerrainType.Snow )
				{
					double d = r.NextDouble();

					if ( d < 0.4 )
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Rock;
					}
					else
					{
						grid.mData[ x, y ].mTerrain = xeTerrainType.Snow;
					}
				}
			} );
		}
		private void PostProcess_ScandinavianCoast( XSafeGrid<xMapCell> map )
		{
			PostProcess_DoNothing( map );
		}
		private void PostProcess_Badlands( XSafeGrid<xMapCell> map )
		{
			map.Iterate( ( grid, x, y ) =>
			{
				xeTerrainType t = grid.mData[ x, y ].mTerrain;

				if ( t == xeTerrainType.DeepWater )
				{
					grid.mData[ x, y ].mTerrain = xeTerrainType.Grassland;
				}
				else if ( t == xeTerrainType.ShallowWater )
				{
					grid.mData[ x, y ].mTerrain = xeTerrainType.Grassland;
				}
				else if ( t == xeTerrainType.Sand )
				{
					grid.mData[ x, y ].mTerrain = xeTerrainType.Grassland;
				}
				else if ( t == xeTerrainType.Forest )
				{
					grid.mData[ x, y ].mTerrain = xeTerrainType.Sand;
				}
			} );
		}
		private void PostProcess_NorthernForest( XSafeGrid<xMapCell> map )
		{
			map.Iterate( ( grid, x, y ) =>
			{
				xeTerrainType t = grid.mData[ x, y ].mTerrain;

				if ( t == xeTerrainType.Sand )
				{
					grid.mData[ x, y ].mTerrain = xeTerrainType.Grassland;
				}
			} );
		}

		
	}
}
