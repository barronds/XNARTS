
///<reference path='..\tsm\tsmVec2.ts' />
///<reference path='..\utils.ts' />
///<reference path='..\three.js.ts' />
///<reference path='mapTypes.ts' />
///<reference path='randomMap.ts' />


class WorldMap 
{
	public static sInstance	: WorldMap;

	private mNumCells		: TSM.vec2;
	private mCells			: Array< Array< MapCell > >;
	private mTHREEObjects	: Array< THREE.Object3D >;
	private mScene			: THREE.Scene;


	public static Init( map_gen_tuning : MapGenTuning ): void
	{
		WorldMap.sInstance = new WorldMap( map_gen_tuning );
	}


	constructor( map_gen_tuning : MapGenTuning )
	{
		map_gen_tuning.Validate();

		this.mNumCells = map_gen_tuning.mNumCells;
		this.mTHREEObjects = new Array( 3 + this.mNumCells.x + this.mNumCells.y );

		// allocate map cells
		this.mCells = new Array( this.mNumCells.x );

		for( var x = 0; x < this.mNumCells.x; x++ )
		{
			this.mCells[ x ] = new Array( this.mNumCells.y );

			for( var y = 0; y < this.mNumCells.y; y++ )
			{
				this.mCells[x][y] = new MapCell();		
			}
		}

		this.GenerateTerrain( map_gen_tuning );
	}


	public Prerender( scene : THREE.Scene ) : void
	{
		this.mScene = scene;

		// squares
		var material = new THREE.MeshBasicMaterial( { vertexColors: THREE.VertexColors, side: THREE.DoubleSide } );
		var geom = new THREE.Geometry();
		var face_vertex_offset = 0;
		var face_index = 0;

		for( var x = 0; x < this.mNumCells.x; x++ )
		{
			for( var y = 0; y < this.mNumCells.y; y++, face_vertex_offset += 4, face_index += 2 )
			{
				var v1 = new THREE.Vector3( x, y, 0 );
				var v2 = new THREE.Vector3( x, (y + 1), 0 );
				var v3 = new THREE.Vector3( (x + 1), y, 0 );
				var v4 = new THREE.Vector3( (x + 1), (y + 1), 0 );

				geom.vertices.push( v1 );
				geom.vertices.push( v2 );
				geom.vertices.push( v3 );
				geom.vertices.push( v4 );

				geom.faces.push( new THREE.Face3( face_vertex_offset + 0, face_vertex_offset + 2, face_vertex_offset + 1 ) );
				geom.faces.push( new THREE.Face3( face_vertex_offset + 2, face_vertex_offset + 3, face_vertex_offset + 1 ) );

				geom.faces[ face_index + 0 ].vertexColors[ 0 ] = new THREE.Color( this.mCells[ x ][ y ].mColour );
				geom.faces[ face_index + 0 ].vertexColors[ 1 ] = new THREE.Color( this.mCells[ x ][ y ].mColour );
				geom.faces[ face_index + 0 ].vertexColors[ 2 ] = new THREE.Color( this.mCells[ x ][ y ].mColour );

				geom.faces[ face_index + 1 ].vertexColors[ 0 ] = new THREE.Color( this.mCells[ x ][ y ].mColour );
				geom.faces[ face_index + 1 ].vertexColors[ 1 ] = new THREE.Color( this.mCells[ x ][ y ].mColour );
				geom.faces[ face_index + 1 ].vertexColors[ 2 ] = new THREE.Color( this.mCells[ x ][ y ].mColour );
			}
		}
		
		geom.computeFaceNormals();
		var mesh = new THREE.Mesh( geom, material );
		scene.add( mesh );	
		this.mTHREEObjects.push( mesh );
		
		// lines
		var line_material = new THREE.LineBasicMaterial( { color : 0x000000 } );

		for( var x = 0; x <= this.mNumCells.x; ++x )
		{
			var geometry = new THREE.Geometry();
			geometry.vertices.push( new THREE.Vector3( x, 0, 0.1 ) );
			geometry.vertices.push( new THREE.Vector3( x, this.mNumCells.y, 0.1 ) );
			var line = new THREE.Line( geometry, line_material );
			scene.add( line );
			this.mTHREEObjects.push( line );
		}

		for( var y = 0; y <= this.mNumCells.y; ++y )
		{
			var geometry = new THREE.Geometry();
			geometry.vertices.push( new THREE.Vector3( 0, y, 0.1 ) );
			geometry.vertices.push( new THREE.Vector3( this.mNumCells.x, y, 0.1 ) );
			var line = new THREE.Line( geometry, line_material );
			scene.add( line );
			this.mTHREEObjects.push( line );
		}
	}


	public Update( dt : number ): void
	{
	}


	private GenerateTerrain( tuning : MapGenTuning ) : void
	{
		// allocate heights[][][]
		var heights = new Array( this.mNumCells.x );

		for( var x = 0; x < this.mNumCells.x; ++ x )
		{
			heights[ x ] = new Array( this.mNumCells.y );

			for( var y = 0; y < this.mNumCells.y; ++ y )
			{
				heights[ x ][ y ] = new Array( 2 );
				heights[ x ][ y ][ 0 ] = 0;
				heights[ x ][ y ][ 1 ] = 0;
			}
		}

		// place some peaks to be smoothed
		var kMaxPeakHeight = tuning.mMaxPeakHeightScalar / tuning.mPeakDensity;
		var numPeaks = Math.floor( this.mNumCells.x * this.mNumCells.y * tuning.mPeakDensity );

		for( var i = 0; i < numPeaks; ++i )
		{
			var peakX = Math.floor( Math.random() * (this.mNumCells.x - 1) );
			var peakY = Math.floor( Math.random() * (this.mNumCells.y - 1) );
			var peakHeight = Math.random() * kMaxPeakHeight;
			heights[ peakX ][ peakY ][ 0 ] = peakHeight;
		}

		var kReciprocalSmoothness = 1 - tuning.mSmoothness;
		var source = 0;

		for( var i = 0; i < tuning.mNumPasses; ++i )
		{
			var dest = source == 0 ? 1 : 0 ;

			for( var x = 0; x < this.mNumCells.x; ++x )
			{
				for( var y = 0; y < this.mNumCells.y; ++y )
				{
					var loX = Math.max( 0, x - 1 );
					var hiX = Math.min( this.mNumCells.x - 1, x + 1 );
					var loY = Math.max( 0, y - 1 );
					var hiY = Math.min( this.mNumCells.y - 1, y + 1 );

					var loXVal = heights[ loX ][ y ][ source ];
					var hiXVal = heights[ hiX ][ y ][ source ];
					var loYVal = heights[ x ][ loY ][ source ];
					var hiYVal = heights[ x ][ hiY ][ source ];
					var hereVal = heights[ x ][ y ][ source ];

					var avgVal = 0.25 * (loXVal + hiXVal + loYVal + hiYVal);
					heights[ x ][ y ][ dest ] = tuning.mSmoothness * hereVal + kReciprocalSmoothness * avgVal; 
				}
			}

			source = source == 1 ? 0 : 1 ;
		}

		// normalize
		var maxValue = 0;

		for( var x = 0; x < this.mNumCells.x; ++x )
		{
			for( var y = 0; y < this.mNumCells.y; ++y )
			{
				maxValue = Math.max( maxValue, heights[ x ][ y ][ 1 ] );
			}
		}		

		for( var x = 0; x < this.mNumCells.x; ++x )
		{
			for( var y = 0; y < this.mNumCells.y; ++y )
			{
				heights[ x ][ y ][ 1 ] /= maxValue;
				heights[ x ][ y ][ 1 ] = Utils.Clamp( heights[ x ][ y ][ 1 ], tuning.mMinNormalHeight, tuning.mMaxNormalHeight );
			}
		}		

		for( var x = 0; x < this.mNumCells.x; ++x )
		{
			for( var y = 0; y < this.mNumCells.y; ++y )
			{
				var terrain = WorldMap.GetTerrainTypeByElevation( heights[ x ][ y ][ 1 ], tuning );
				this.mCells[ x ][ y ].mTerrainType = terrain;
				var color = WorldMap.GetColourByTerrainType( terrain );
				this.mCells[ x ][ y ].mColour = color;
			}
		}
	}


	private static RegenerateTerrain() : void
	{
		// pull the map rendering stuff out of the scene, then reinitialize this whole class.
		var obj = WorldMap.sInstance.mTHREEObjects.pop();

		while( obj != null )
		{
			WorldMap.sInstance.mScene.remove( obj );
			obj = WorldMap.sInstance.mTHREEObjects.pop();
		}

		// pull the data we need out of the WorldMap instance before we destroy it
		var num_cells = WorldMap.sInstance.mNumCells;
		var scene = WorldMap.sInstance.mScene;
		WorldMap.sInstance = null;

		// re-initialize the class
		WorldMap.Init( RandomMap.GetRandomMapGenTuning( num_cells ) );
		WorldMap.sInstance.Prerender( scene );	
	}


	private static GetTerrainTypeByElevation( elevation : number, tuning : MapGenTuning ) : eTerrainType
	{
		if		( elevation < tuning.mTerrainTypeThreshold[ eTerrainType.DeepWater ] ) 	return eTerrainType.DeepWater;
		else if	( elevation < tuning.mTerrainTypeThreshold[ eTerrainType.Shallows ] )	return eTerrainType.Shallows;
		else if	( elevation < tuning.mTerrainTypeThreshold[ eTerrainType.Beach ] )		return eTerrainType.Beach;
		else if	( elevation < tuning.mTerrainTypeThreshold[ eTerrainType.Plains ] )		return eTerrainType.Plains;
		else if	( elevation < tuning.mTerrainTypeThreshold[ eTerrainType.Forest ] )		return eTerrainType.Forest;
		else if	( elevation < tuning.mTerrainTypeThreshold[ eTerrainType.Rocks ] )		return eTerrainType.Rocks;
		else 																			return eTerrainType.Glacier;	
	}


	private static GetColourByTerrainType( terrain_type : eTerrainType ) : number
	{
		switch( terrain_type )
		{
			case eTerrainType.DeepWater :	return 0x102080;
			case eTerrainType.Shallows :	return 0x1054a4;
			case eTerrainType.Beach :		return 0xa0a068;
			case eTerrainType.Plains :		return 0x509038;
			case eTerrainType.Forest :		return 0x186020;
			case eTerrainType.Rocks :		return 0x808080;
			case eTerrainType.Glacier :		return 0xe0e0e0;
			case eTerrainType.City :		return 0xb08080;
			
			default :						return 0xaaaaaa;
		}
	}


	public static OnKeyPress( e : KeyboardEvent ) : void
	{
		// clean up the old map, make a new one.
		if( WorldMap.sInstance != null && e.keyCode == "M".charCodeAt( 0 ) )
		{
			WorldMap.RegenerateTerrain();
		}
	}

}


