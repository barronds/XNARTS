﻿using System;
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
		ScreenSpace_Transient,
		ScreenSpace_Persistent,
		WorldSpace_Transient,
		WorldSpace_Persistent,
		WorldSpace_Persistent_Map,
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

        int						mMaxLines;
        int                     mNumLines;
        VertexPositionColor []  mLines;

		int						mMaxTriangles;
		int						mNumTriangles;
		VertexPositionColor []	mTriangles;

        private GraphicsDevice	mGraphicsDevice;
		private bool			mPersistent;


		// private constructor as per pluralton
		private XSimpleDraw()
		{}


        public void Init( GraphicsDevice graphics_device, bool persistent, int max_lines, int max_triangles )
        {
            mGraphicsDevice = graphics_device;
			mPersistent = persistent;
			mMaxLines = max_lines;
			mMaxTriangles = max_triangles;
            mLines = new VertexPositionColor[ 2 * mMaxLines ];
            mNumLines = 0;
			mTriangles = new VertexPositionColor[ 3 * mMaxTriangles ];
			mNumTriangles = 0;
        }


        public void DrawLine( Vector3 start_pos, Vector3 end_pos, Color color )
        {
            if ( mNumLines == mMaxLines )
            {
                XUtils.Assert( false, "max lines exceeded" );
                return;
            }

            VertexPositionColor start = new VertexPositionColor( start_pos, color );
            VertexPositionColor end = new VertexPositionColor( end_pos, color );

            int start_vert = 2 * mNumLines;
            mLines[ start_vert ] = start;
            mLines[ start_vert + 1 ] = end;
            ++ mNumLines;
        }


		// watch the winding order for which way the normal goes, by right hand rule
		public void DrawTriangle( Vector3 a, Vector3 b, Vector3 c, Color color )
		{
			if( mNumTriangles == mMaxTriangles )
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
			// triangles
			if ( mNumTriangles > 0 )
			{
				mGraphicsDevice.DrawUserPrimitives( PrimitiveType.TriangleList, mTriangles, 0, mNumTriangles );

				if ( !mPersistent )
				{
					mNumTriangles = 0;
				}
			}

			// lines
			if ( mNumLines > 0 )
            {
                mGraphicsDevice.DrawUserPrimitives( PrimitiveType.LineList, mLines, 0, mNumLines );

				if( !mPersistent )
				{
				    mNumLines = 0;
				}
            }
        }


		public void CancelPrimitives()
		{
			XUtils.Assert( mPersistent, "this method only necessary for persistent simple draws" );
			mNumLines = 0;
			mNumTriangles = 0;
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
