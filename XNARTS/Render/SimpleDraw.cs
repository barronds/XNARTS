using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNARTS.Render
{
    public class SimpleDraw
    {
        public struct tBatchId
        {
            public int mId;

            public tBatchId( int id )
            {
                mId = id;
            }
        }

        const int               kMaxLines = 10;
        int                     mNumLines;
        VertexPositionColor []  mLines;


        private GraphicsDevice mGraphicsDevice;

        public SimpleDraw( GraphicsDevice graphics_device )
        {
            mGraphicsDevice = graphics_device;
            mLines = new VertexPositionColor[ 2 * kMaxLines ];
            mNumLines = 0;
        }

        public void DrawLine( Vector3 start_pos, Vector3 end_pos, Color start_color, Color end_color )
        {
            if ( mNumLines == kMaxLines )
            {
                Debug.Assert( false, "max lines exceeded" );
                return;
            }

            VertexPositionColor start = new VertexPositionColor( start_pos, start_color );
            VertexPositionColor end = new VertexPositionColor( end_pos, end_color );

            int start_vert = 2 * mNumLines;
            mLines[ start_vert ] = start;
            mLines[ start_vert + 1 ] = end;
            ++mNumLines;
        }

        public void DrawAllPrimitives()
        {
            // lines
            if ( mNumLines > 0 )
            {
                mGraphicsDevice.DrawUserPrimitives( PrimitiveType.LineList, mLines, 0, mNumLines );
                mNumLines = 0;
            }
        }




        // not yet implemented 

        public tBatchId StartLineStrip( int line_count )
        {
            // return index into line strip list as id
            tBatchId id;
            id.mId = 0;
            return id;
        }

        public void AddLineStripVertex( Vector3 pos, Color color )
        {

        }

        public void EndLineStrip( tBatchId line_strip_id )
        {
            // verify all promised are there
        }
    }
}
