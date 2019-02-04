using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNARTS
{
	public enum xeSimpleDrawType
	{
		Screen_Transient,
		Screen_Persistent,
		World_Transient,
		World_Persistent
	}


    public class XSimpleDraw : XPluralton< xeSimpleDrawType, XSimpleDraw >
    {
        public struct xBatchId
        {
            public int mId;

            public xBatchId( int id )
            {
                mId = id;
            }
        }

        const int               kMaxLines = 50;
        int                     mNumLines;
        VertexPositionColor []  mLines;

		const int				kMaxTriangles = 100000;
		int						mNumTriangles;
		VertexPositionColor []	mTriangles;

        private GraphicsDevice	mGraphicsDevice;
		private bool			mPersistent;


		// private constructor as per pluralton
		private XSimpleDraw()
		{}


        public void Init( GraphicsDevice graphics_device, bool persistent )
        {
            mGraphicsDevice = graphics_device;
			mPersistent = persistent;
            mLines = new VertexPositionColor[ 2 * kMaxLines ];
            mNumLines = 0;
			mTriangles = new VertexPositionColor[ 3 * kMaxTriangles ];
			mNumTriangles = 0;
        }


        public void DrawLine( Vector3 start_pos, Vector3 end_pos, Color start_color, Color end_color )
        {
            if ( mNumLines == kMaxLines )
            {
                XUtils.Assert( false, "max lines exceeded" );
                return;
            }

            VertexPositionColor start = new VertexPositionColor( start_pos, start_color );
            VertexPositionColor end = new VertexPositionColor( end_pos, end_color );

            int start_vert = 2 * mNumLines;
            mLines[ start_vert ] = start;
            mLines[ start_vert + 1 ] = end;
            ++ mNumLines;
        }


		// watch the winding order for which way the normal goes, by right hand rule
		public void DrawTriangle( Vector3 a, Vector3 b, Vector3 c, Color color )
		{
			if( mNumTriangles == kMaxTriangles )
			{
				XUtils.Assert( false, "max triangles exceeded" );
			}

			VertexPositionColor va = new VertexPositionColor( a, color );
			VertexPositionColor vb = new VertexPositionColor( b, color );
			VertexPositionColor vc = new VertexPositionColor( c, color );

			int start_vert = 3 * mNumTriangles;
			mTriangles[ start_vert ] = va;
			mTriangles[ start_vert + 1 ] = vb;
			mTriangles[ start_vert + 2 ] = vc;
			++ mNumTriangles;
		}


		// low, high means low and high values of x, y.  hopefully both vectors have same z value
		public void DrawQuad( Vector3 low, Vector3 high, Color color )
		{
			Vector3 high_x = new Vector3( high.X, low.Y, high.Z );
			Vector3 high_y = new Vector3( low.X, high.Y, low.Z );

			DrawTriangle( low, high_x, high_y, color );
			DrawTriangle( high_x, high, high_y, color );			
		}


        public void DrawAllPrimitives()
        {
            // lines
            if( mNumLines > 0 )
            {
                mGraphicsDevice.DrawUserPrimitives( PrimitiveType.LineList, mLines, 0, mNumLines );

				if( !mPersistent )
				{
				    mNumLines = 0;
				}
            }

			// triangles
			if( mNumTriangles > 0 )
			{
				mGraphicsDevice.DrawUserPrimitives( PrimitiveType.TriangleList, mTriangles, 0, mNumTriangles );

				if( !mPersistent )
				{
					mNumTriangles = 0;
				}
			}
        }




        // not yet implemented 

        public xBatchId StartLineStrip( int line_count )
        {
            // return index into line strip list as id
            xBatchId id;
            id.mId = 0;
            return id;
        }

        public void AddLineStripVertex( Vector3 pos, Color color )
        {

        }

        public void EndLineStrip( xBatchId line_strip_id )
        {
            // verify all promised are there
        }
    }
}
