
///<reference path='..\tsm\tsmVec2.ts' />
///<reference path='..\utils.ts' />
///<reference path='..\three.js.ts' />
///<reference path='mapTypes.ts' />


class RandomMap
{

	public static GetDefaultMapSize() : TSM.vec2
	{
		return new TSM.vec2( [ 320, 163 ] );
	}


	public static GetParticularMapGenTuning( num_cells : TSM.vec2, map_flavor : eRandomMapFlavor ) : MapGenTuning
	{
		var tuning = new MapGenTuning();
		tuning.mNumCells = num_cells;
		tuning.mTerrainTypeThreshold =	new Array< number >( eTerrainType.Num - 1 );
		RandomMap.FillMapTuning( tuning, map_flavor );
		return tuning;
	}


	public static GetRandomMapGenTuning( num_cells : TSM.vec2 ) : MapGenTuning
	{
		var map_flavor = <eRandomMapFlavor>Utils.GetRandomInt( 0, eRandomMapFlavor.Num - 1 );
		return RandomMap.GetParticularMapGenTuning( num_cells, map_flavor );
	}

		
	private static FillMapTuning( tuning : MapGenTuning, map_flavor : eRandomMapFlavor ) : void
	{
		switch( map_flavor ) 
		{
			case eRandomMapFlavor.Default :
			{
				tuning.mPeakDensity =			0.003;
				tuning.mMaxPeakHeightScalar =	2.0;
				tuning.mNumPasses =				351; 
				tuning.mSmoothness =			0.8;
				tuning.mMinNormalHeight =		0;
				tuning.mMaxNormalHeight =		1;

				tuning.mTerrainTypeThreshold[ eTerrainType.DeepWater ] =	0.02;
				tuning.mTerrainTypeThreshold[ eTerrainType.Shallows ] =		0.04;
				tuning.mTerrainTypeThreshold[ eTerrainType.Beach ] =		0.08;
				tuning.mTerrainTypeThreshold[ eTerrainType.Plains ] =		0.30;
				tuning.mTerrainTypeThreshold[ eTerrainType.Forest ] =		0.52;
				tuning.mTerrainTypeThreshold[ eTerrainType.Rocks ] =		0.73;
				tuning.mTerrainTypeThreshold[ eTerrainType.Glacier ] =		1.00;
			}
			break;

			case eRandomMapFlavor.PacificIslands :
			{
				tuning.mPeakDensity =			0.005;
				tuning.mMaxPeakHeightScalar =	2.0;
				tuning.mNumPasses =				351; 
				tuning.mSmoothness =			0.8;
				tuning.mMinNormalHeight =		0;
				tuning.mMaxNormalHeight =		0.5;

				tuning.mTerrainTypeThreshold[ eTerrainType.DeepWater ] =	0.25;
				tuning.mTerrainTypeThreshold[ eTerrainType.Shallows ] =		0.35;
				tuning.mTerrainTypeThreshold[ eTerrainType.Beach ] =		0.43;
				tuning.mTerrainTypeThreshold[ eTerrainType.Plains ] =		0.46;
				tuning.mTerrainTypeThreshold[ eTerrainType.Forest ] =		0.51;
				tuning.mTerrainTypeThreshold[ eTerrainType.Rocks ] =		0.73;
				tuning.mTerrainTypeThreshold[ eTerrainType.Glacier ] =		1.00;
			}
			break;

			case eRandomMapFlavor.MountainRange :
			{
				tuning.mPeakDensity =			0.005;
				tuning.mMaxPeakHeightScalar =	2.0;
				tuning.mNumPasses =				351; 
				tuning.mSmoothness =			0.65;
				tuning.mMinNormalHeight =		0.1;
				tuning.mMaxNormalHeight =		1;

				tuning.mTerrainTypeThreshold[ eTerrainType.DeepWater ] =	0.01;
				tuning.mTerrainTypeThreshold[ eTerrainType.Shallows ] =		0.02;
				tuning.mTerrainTypeThreshold[ eTerrainType.Beach ] =		0.03;
				tuning.mTerrainTypeThreshold[ eTerrainType.Plains ] =		0.15;
				tuning.mTerrainTypeThreshold[ eTerrainType.Forest ] =		0.25;
				tuning.mTerrainTypeThreshold[ eTerrainType.Rocks ] =		0.40;
				tuning.mTerrainTypeThreshold[ eTerrainType.Glacier ] =		1.00;
			}
			break;

			case eRandomMapFlavor.BigSimpleIslands :
			{
				tuning.mPeakDensity =			0.0003;
				tuning.mMaxPeakHeightScalar =	2.0;
				tuning.mNumPasses =				551; 
				tuning.mSmoothness =			0.2;
				tuning.mMinNormalHeight =		0;
				tuning.mMaxNormalHeight =		1;

				tuning.mTerrainTypeThreshold[ eTerrainType.DeepWater ] =	0.1;
				tuning.mTerrainTypeThreshold[ eTerrainType.Shallows ] =		0.2;
				tuning.mTerrainTypeThreshold[ eTerrainType.Beach ] =		0.3;
				tuning.mTerrainTypeThreshold[ eTerrainType.Plains ] =		0.4;
				tuning.mTerrainTypeThreshold[ eTerrainType.Forest ] =		0.5;
				tuning.mTerrainTypeThreshold[ eTerrainType.Rocks ] =		0.7;
				tuning.mTerrainTypeThreshold[ eTerrainType.Glacier ] =		1.00;
			}
			break;

			case eRandomMapFlavor.Continental :
			{
				tuning.mPeakDensity =			0.0007;
				tuning.mMaxPeakHeightScalar =	2.0;
				tuning.mNumPasses =				551; 
				tuning.mSmoothness =			0.1;
				tuning.mMinNormalHeight =		0;
				tuning.mMaxNormalHeight =		1;

				tuning.mTerrainTypeThreshold[ eTerrainType.DeepWater ] =	0.02;
				tuning.mTerrainTypeThreshold[ eTerrainType.Shallows ] =		0.05;
				tuning.mTerrainTypeThreshold[ eTerrainType.Beach ] =		0.15;
				tuning.mTerrainTypeThreshold[ eTerrainType.Plains ] =		0.3;
				tuning.mTerrainTypeThreshold[ eTerrainType.Forest ] =		0.5;
				tuning.mTerrainTypeThreshold[ eTerrainType.Rocks ] =		0.65;
				tuning.mTerrainTypeThreshold[ eTerrainType.Glacier ] =		1.00;
			}
			break;

			case eRandomMapFlavor.TundraCoastal :
			{
				tuning.mPeakDensity =			0.0007;
				tuning.mMaxPeakHeightScalar =	2.0;
				tuning.mNumPasses =				551; 
				tuning.mSmoothness =			0.1;
				tuning.mMinNormalHeight =		0;
				tuning.mMaxNormalHeight =		1;

				tuning.mTerrainTypeThreshold[ eTerrainType.DeepWater ] =	0.08;
				tuning.mTerrainTypeThreshold[ eTerrainType.Shallows ] =		0.13;
				tuning.mTerrainTypeThreshold[ eTerrainType.Beach ] =		0.15;
				tuning.mTerrainTypeThreshold[ eTerrainType.Plains ] =		0.259999999;
				tuning.mTerrainTypeThreshold[ eTerrainType.Forest ] =		0.26;
				tuning.mTerrainTypeThreshold[ eTerrainType.Rocks ] =		0.29;
				tuning.mTerrainTypeThreshold[ eTerrainType.Glacier ] =		1.00;
			}
			break;

			case eRandomMapFlavor.ArcticCoastal :
			{
				tuning.mPeakDensity =			0.0007;
				tuning.mMaxPeakHeightScalar =	2.0;
				tuning.mNumPasses =				551; 
				tuning.mSmoothness =			0.1;
				tuning.mMinNormalHeight =		0;
				tuning.mMaxNormalHeight =		1;

				tuning.mTerrainTypeThreshold[ eTerrainType.DeepWater ] =	0.08;
				tuning.mTerrainTypeThreshold[ eTerrainType.Shallows ] =		0.259999997;
				tuning.mTerrainTypeThreshold[ eTerrainType.Beach ] =		0.259999998;
				tuning.mTerrainTypeThreshold[ eTerrainType.Plains ] =		0.259999999;
				tuning.mTerrainTypeThreshold[ eTerrainType.Forest ] =		0.26;
				tuning.mTerrainTypeThreshold[ eTerrainType.Rocks ] =		0.29;
				tuning.mTerrainTypeThreshold[ eTerrainType.Glacier ] =		1.00;
			}
			break;

			case eRandomMapFlavor.Prairie :
			{
				tuning.mPeakDensity =			0.0014;
				tuning.mMaxPeakHeightScalar =	2.0;
				tuning.mNumPasses =				551; 
				tuning.mSmoothness =			0.25;
				tuning.mMinNormalHeight =		0;
				tuning.mMaxNormalHeight =		1;

				tuning.mTerrainTypeThreshold[ eTerrainType.DeepWater ] =	0.00;
				tuning.mTerrainTypeThreshold[ eTerrainType.Shallows ] =		0.05;
				tuning.mTerrainTypeThreshold[ eTerrainType.Beach ] =		0.07;
				tuning.mTerrainTypeThreshold[ eTerrainType.Plains ] =		0.50;
				tuning.mTerrainTypeThreshold[ eTerrainType.Forest ] =		0.60;
				tuning.mTerrainTypeThreshold[ eTerrainType.Rocks ] =		0.99;
				tuning.mTerrainTypeThreshold[ eTerrainType.Glacier ] =		1.00;
			}
			break;

			case eRandomMapFlavor.Desert :
			{
				tuning.mPeakDensity =			0.009;
				tuning.mMaxPeakHeightScalar =	2.0;
				tuning.mNumPasses =				251; 
				tuning.mSmoothness =			0.25;
				tuning.mMinNormalHeight =		0;
				tuning.mMaxNormalHeight =		0.99;

				tuning.mTerrainTypeThreshold[ eTerrainType.DeepWater ] =	0.00;
				tuning.mTerrainTypeThreshold[ eTerrainType.Shallows ] =		0.1;
				tuning.mTerrainTypeThreshold[ eTerrainType.Beach ] =		0.68;
				tuning.mTerrainTypeThreshold[ eTerrainType.Plains ] =		0.70;
				tuning.mTerrainTypeThreshold[ eTerrainType.Forest ] =		0.700000001;
				tuning.mTerrainTypeThreshold[ eTerrainType.Rocks ] =		0.99999999;
				tuning.mTerrainTypeThreshold[ eTerrainType.Glacier ] =		1.00;
			}
			break;

			case eRandomMapFlavor.Jungle :
			{
				tuning.mPeakDensity =			0.009;
				tuning.mMaxPeakHeightScalar =	2.0;
				tuning.mNumPasses =				251; 
				tuning.mSmoothness =			0.35;
				tuning.mMinNormalHeight =		0;
				tuning.mMaxNormalHeight =		0.69;

				tuning.mTerrainTypeThreshold[ eTerrainType.DeepWater ] =	0.00;
				tuning.mTerrainTypeThreshold[ eTerrainType.Shallows ] =		0.15;
				tuning.mTerrainTypeThreshold[ eTerrainType.Beach ] =		0.16;
				tuning.mTerrainTypeThreshold[ eTerrainType.Plains ] =		0.19;
				tuning.mTerrainTypeThreshold[ eTerrainType.Forest ] =		0.64;
				tuning.mTerrainTypeThreshold[ eTerrainType.Rocks ] =		0.7;
				tuning.mTerrainTypeThreshold[ eTerrainType.Glacier ] =		1.00;
			}
			break;

			case eRandomMapFlavor.Grassland :
			{
				tuning.mPeakDensity =			0.009;
				tuning.mMaxPeakHeightScalar =	2.0;
				tuning.mNumPasses =				251; 
				tuning.mSmoothness =			0.35;
				tuning.mMinNormalHeight =		0;
				tuning.mMaxNormalHeight =		0.69;

				tuning.mTerrainTypeThreshold[ eTerrainType.DeepWater ] =	0.00;
				tuning.mTerrainTypeThreshold[ eTerrainType.Shallows ] =		0.15;
				tuning.mTerrainTypeThreshold[ eTerrainType.Beach ] =		0.18;
				tuning.mTerrainTypeThreshold[ eTerrainType.Plains ] =		0.64;
				tuning.mTerrainTypeThreshold[ eTerrainType.Forest ] =		0.685;
				tuning.mTerrainTypeThreshold[ eTerrainType.Rocks ] =		0.7;
				tuning.mTerrainTypeThreshold[ eTerrainType.Glacier ] =		1.00;
			}
			break;

			default : 
			{
				RandomMap.FillMapTuning( tuning, eRandomMapFlavor.Default ); 
			}
			break;
		}	
		
	}

}



