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

			ScandinavianIslands,
			SouthPacificIslands,
			CarribeanIslands,
			GulfIslands,
			ArcticIslands,
			NorthSeaIslands,
			Prarie,
			Badlands,
			Desert,
			DesertMountains,
			Tundra,
			NorthernForest,
			TemperateForest,
			TropicalForest,
			Mediterranean,

			Num,

			Default = GulfIslands
		}

		public class Set
		{
			public bool     mInitialized;
			public double	mSpikeDensity;
			public double	mSpikeHeight;
			public double	mSpikeVariance;
			public double	mMinNormalizedHeight;
			public double	mMaxNormalizedHeight;
			public int		mSmoothingPasses;
			public double	mSmoothingScalar;
			public int		mGridWidth;
			public int		mGridHeight;
			public double[] mHeightThresh;
		}

		private Set[] mSets;

		public XWorldGen()
		{
			mSets = new Set[ (int)eMapType.Num ];

			for( int i = 0; i < (int)eMapType.Num; ++i )
			{
				mSets[ i ] = new Set();
				mSets[ i ].mHeightThresh = new double[ (int)xeTerrainType.Num - 1 ];
				mSets[ i ].mInitialized = false;
			}

			Set s = mSets[ (int)eMapType.ScandinavianIslands ];

			s.mInitialized			= true;
			s.mSpikeDensity			= 0.04;
			s.mSpikeHeight			= 300;
			s.mSpikeVariance		= 0.6;
			s.mMinNormalizedHeight	= 0;
			s.mMaxNormalizedHeight	= 1;
			s.mSmoothingPasses		= 200;
			s.mSmoothingScalar		= 0.5;
			s.mGridWidth			= 320;
			s.mGridHeight			= 180;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ]		= 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ]	= 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ]			= 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ]		= 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ]		= 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ]			= 0.9d;

			s = mSets[ (int)eMapType.ScandinavianIslands ];

			s.mInitialized			= true;
			s.mSpikeDensity			= 0.04;
			s.mSpikeHeight			= 300;
			s.mSpikeVariance		= 0.6;
			s.mMinNormalizedHeight	= 0;
			s.mMaxNormalizedHeight	= 1;
			s.mSmoothingPasses		= 200;
			s.mSmoothingScalar		= 0.5;
			s.mGridWidth			= 320;
			s.mGridHeight			= 180;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ]		= 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ]	= 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ]			= 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ]		= 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ]		= 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ]			= 0.9d;

			s = mSets[ (int)eMapType.SouthPacificIslands ];

			s.mInitialized			= true;
			s.mSpikeDensity			= 0.04;
			s.mSpikeHeight			= 300;
			s.mSpikeVariance		= 0.6;
			s.mMinNormalizedHeight	= 0;
			s.mMaxNormalizedHeight	= 1;
			s.mSmoothingPasses		= 200;
			s.mSmoothingScalar		= 0.5;
			s.mGridWidth			= 320;
			s.mGridHeight			= 180;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ]		= 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ]	= 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ]			= 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ]		= 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ]		= 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ]			= 0.9d;

			s = mSets[ (int)eMapType.CarribeanIslands ];

			s.mInitialized			= true;
			s.mSpikeDensity			= 0.04;
			s.mSpikeHeight			= 300;
			s.mSpikeVariance		= 0.6;
			s.mMinNormalizedHeight	= 0;
			s.mMaxNormalizedHeight	= 1;
			s.mSmoothingPasses		= 200;
			s.mSmoothingScalar		= 0.5;
			s.mGridWidth			= 320;
			s.mGridHeight			= 180;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ]		= 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ]	= 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ]			= 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ]		= 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ]		= 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ]			= 0.9d;

			s = mSets[ (int)eMapType.GulfIslands ];

			s.mInitialized			= true;
			s.mSpikeDensity			= 0.04;
			s.mSpikeHeight			= 300;
			s.mSpikeVariance		= 0.6;
			s.mMinNormalizedHeight	= 0;
			s.mMaxNormalizedHeight	= 1;
			s.mSmoothingPasses		= 200;
			s.mSmoothingScalar		= 0.5;
			s.mGridWidth			= 320;
			s.mGridHeight			= 180;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ]		= 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ]	= 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ]			= 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ]		= 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ]		= 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ]			= 0.9d;

			s = mSets[ (int)eMapType.ArcticIslands ];

			s.mInitialized			= true;
			s.mSpikeDensity			= 0.04;
			s.mSpikeHeight			= 300;
			s.mSpikeVariance		= 0.6;
			s.mMinNormalizedHeight	= 0;
			s.mMaxNormalizedHeight	= 1;
			s.mSmoothingPasses		= 200;
			s.mSmoothingScalar		= 0.5;
			s.mGridWidth			= 320;
			s.mGridHeight			= 180;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ]		= 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ]	= 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ]			= 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ]		= 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ]		= 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ]			= 0.9d;

			s = mSets[ (int)eMapType.NorthSeaIslands ];

			s.mInitialized			= true;
			s.mSpikeDensity			= 0.04;
			s.mSpikeHeight			= 300;
			s.mSpikeVariance		= 0.6;
			s.mMinNormalizedHeight	= 0;
			s.mMaxNormalizedHeight	= 1;
			s.mSmoothingPasses		= 200;
			s.mSmoothingScalar		= 0.5;
			s.mGridWidth			= 320;
			s.mGridHeight			= 180;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ]		= 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ]	= 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ]			= 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ]		= 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ]		= 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ]			= 0.9d;

			s = mSets[ (int)eMapType.Prarie ];

			s.mInitialized			= true;
			s.mSpikeDensity			= 0.04;
			s.mSpikeHeight			= 300;
			s.mSpikeVariance		= 0.6;
			s.mMinNormalizedHeight	= 0;
			s.mMaxNormalizedHeight	= 1;
			s.mSmoothingPasses		= 200;
			s.mSmoothingScalar		= 0.5;
			s.mGridWidth			= 320;
			s.mGridHeight			= 180;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ]		= 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ]	= 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ]			= 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ]		= 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ]		= 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ]			= 0.9d;

			s = mSets[ (int)eMapType.Badlands ];

			s.mInitialized			= true;
			s.mSpikeDensity			= 0.04;
			s.mSpikeHeight			= 300;
			s.mSpikeVariance		= 0.6;
			s.mMinNormalizedHeight	= 0;
			s.mMaxNormalizedHeight	= 1;
			s.mSmoothingPasses		= 200;
			s.mSmoothingScalar		= 0.5;
			s.mGridWidth			= 320;
			s.mGridHeight			= 180;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ]		= 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ]	= 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ]			= 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ]		= 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ]		= 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ]			= 0.9d;

			s = mSets[ (int)eMapType.Desert ];

			s.mInitialized			= true;
			s.mSpikeDensity			= 0.04;
			s.mSpikeHeight			= 300;
			s.mSpikeVariance		= 0.6;
			s.mMinNormalizedHeight	= 0;
			s.mMaxNormalizedHeight	= 1;
			s.mSmoothingPasses		= 200;
			s.mSmoothingScalar		= 0.5;
			s.mGridWidth			= 320;
			s.mGridHeight			= 180;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ]		= 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ]	= 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ]			= 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ]		= 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ]		= 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ]			= 0.9d;

			s = mSets[ (int)eMapType.DesertMountains ];

			s.mInitialized			= true;
			s.mSpikeDensity			= 0.04;
			s.mSpikeHeight			= 300;
			s.mSpikeVariance		= 0.6;
			s.mMinNormalizedHeight	= 0;
			s.mMaxNormalizedHeight	= 1;
			s.mSmoothingPasses		= 200;
			s.mSmoothingScalar		= 0.5;
			s.mGridWidth			= 320;
			s.mGridHeight			= 180;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ]		= 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ]	= 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ]			= 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ]		= 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ]		= 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ]			= 0.9d;

			s = mSets[ (int)eMapType.Tundra ];

			s.mInitialized			= true;
			s.mSpikeDensity			= 0.04;
			s.mSpikeHeight			= 300;
			s.mSpikeVariance		= 0.6;
			s.mMinNormalizedHeight	= 0;
			s.mMaxNormalizedHeight	= 1;
			s.mSmoothingPasses		= 200;
			s.mSmoothingScalar		= 0.5;
			s.mGridWidth			= 320;
			s.mGridHeight			= 180;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ]		= 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ]	= 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ]			= 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ]		= 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ]		= 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ]			= 0.9d;

			s = mSets[ (int)eMapType.NorthernForest ];

			s.mInitialized			= true;
			s.mSpikeDensity			= 0.04;
			s.mSpikeHeight			= 300;
			s.mSpikeVariance		= 0.6;
			s.mMinNormalizedHeight	= 0;
			s.mMaxNormalizedHeight	= 1;
			s.mSmoothingPasses		= 200;
			s.mSmoothingScalar		= 0.5;
			s.mGridWidth			= 320;
			s.mGridHeight			= 180;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ]		= 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ]	= 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ]			= 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ]		= 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ]		= 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ]			= 0.9d;

			s = mSets[ (int)eMapType.TemperateForest ];

			s.mInitialized			= true;
			s.mSpikeDensity			= 0.04;
			s.mSpikeHeight			= 300;
			s.mSpikeVariance		= 0.6;
			s.mMinNormalizedHeight	= 0;
			s.mMaxNormalizedHeight	= 1;
			s.mSmoothingPasses		= 200;
			s.mSmoothingScalar		= 0.5;
			s.mGridWidth			= 320;
			s.mGridHeight			= 180;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ]		= 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ]	= 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ]			= 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ]		= 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ]		= 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ]			= 0.9d;

			s = mSets[ (int)eMapType.TropicalForest ];

			s.mInitialized			= true;
			s.mSpikeDensity			= 0.04;
			s.mSpikeHeight			= 300;
			s.mSpikeVariance		= 0.6;
			s.mMinNormalizedHeight	= 0;
			s.mMaxNormalizedHeight	= 1;
			s.mSmoothingPasses		= 200;
			s.mSmoothingScalar		= 0.5;
			s.mGridWidth			= 320;
			s.mGridHeight			= 180;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ]		= 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ]	= 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ]			= 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ]		= 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ]		= 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ]			= 0.9d;

			s = mSets[ (int)eMapType.Mediterranean ];

			s.mInitialized			= true;
			s.mSpikeDensity			= 0.04;
			s.mSpikeHeight			= 300;
			s.mSpikeVariance		= 0.6;
			s.mMinNormalizedHeight	= 0;
			s.mMaxNormalizedHeight	= 1;
			s.mSmoothingPasses		= 200;
			s.mSmoothingScalar		= 0.5;
			s.mGridWidth			= 320;
			s.mGridHeight			= 180;

			s.mHeightThresh[ (int)xeTerrainType.DeepWater ]		= 0.5d;
			s.mHeightThresh[ (int)xeTerrainType.ShallowWater ]	= 0.6d;
			s.mHeightThresh[ (int)xeTerrainType.Sand ]			= 0.65d;
			s.mHeightThresh[ (int)xeTerrainType.Grassland ]		= 0.7d;
			s.mHeightThresh[ (int)xeTerrainType.Forest ]		= 0.8d;
			s.mHeightThresh[ (int)xeTerrainType.Rock ]			= 0.9d;
		}


		public Set GetTuningSet( eMapType map_type )
		{
			return mSets[ (int)map_type ];
		} 
	}
}
