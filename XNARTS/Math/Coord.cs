using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace XNARTS
{
    public struct xCoord
    {
        public int x { get; set; }
        public int y { get; set; }

        public xCoord( int x, int y )
        {
            this.x = x;
            this.y = y;
        }

        public xCoord( double x, double y )
        {
            this.x = (int)x;
            this.y = (int)y;
        }

        public float getLength()
        {
            return (float)System.Math.Sqrt( this.x * this.x + this.y * this.y );
        }

        public xCoord clamp( xCoord min, xCoord max )
        {
			
            XUtils.Assert( !(min.x > max.x || min.y > max.y) );
            int clamped_x = System.Math.Max( System.Math.Min( max.x, this.x ), min.x );
            int clamped_y = System.Math.Max( System.Math.Min( max.y, this.y ), min.y );
            return new xCoord( clamped_x, clamped_y );
        }

        public static xCoord operator -( xCoord coord )
        {
            return new xCoord( 0 - coord.x, 0 - coord.y );
        }

        public static xCoord operator *( float s, xCoord coord )
        {
            return new xCoord( coord.x * s, coord.y * s );
        }

        public static xCoord operator *( xCoord coord, float s )
        {
            return s * coord;
        }

        public static xCoord operator +( xCoord a, xCoord b )
        {
            return new xCoord( a.x + b.x, a.y + b.y );
        }

        public static xCoord operator -( xCoord a, xCoord b )
        {
            return new xCoord( a.x - b.x, a.y - b.y );
        }

        public static bool operator ==( xCoord a, xCoord b )
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=( xCoord a, xCoord b )
        {
            return a.x != b.x || a.y != b.y;
        }

        public override bool Equals( Object obj )
        {
            if ( obj is xCoord )
            {
                return (xCoord)obj == this;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static void unitTest()
        {
            //XUtils.Assert( false, "hello" );
            xCoord a = new xCoord();
            XUtils.Assert( a.x == 0 );
			XUtils.Assert( a.y == 0 );

            xCoord b = a;
			XUtils.Assert( b.x == 0 );
			XUtils.Assert( b.y == 0 );

            xCoord c = new xCoord( -10, 20.0f );
            XUtils.Assert( c.x == -10 );
            XUtils.Assert( c.y == 20 );

            xCoord d = 5 * c;
            xCoord e = new xCoord( -50, 100 );
            XUtils.Assert( d == e );
            XUtils.Assert( d != b );
            XUtils.Assert( d.Equals( e ) );
            XUtils.Assert( !d.Equals( a ) );

#pragma warning disable CS1718
            XUtils.Assert( d == d );
#pragma warning restore CS1718

            xCoord f = new xCoord( 2, 6 );
            xCoord g = new xCoord( -11, 33 );
            xCoord h = f + g;
            xCoord i = f - g;
            xCoord j = f - f;
            xCoord k = g - g - g;
            XUtils.Assert( h == new xCoord( -9, 39 ) );
            XUtils.Assert( i == new xCoord( 13, -27 ) );
            XUtils.Assert( j == new xCoord() );
            XUtils.Assert( k == -g );

            xCoord l = new xCoord( 2, -5 );
            xCoord m = 4 * l;
            xCoord n = -3.5f * l;
            xCoord o = l * 4;
            XUtils.Assert( m == new xCoord( 8, -20 ) );
            XUtils.Assert( n == new xCoord( -7, 17 ) );
            XUtils.Assert( o == m );

            xCoord p = new xCoord( 11.4, -55.6 );
            xCoord q = new xCoord( 11.6, -55.4 );
            XUtils.Assert( p == new xCoord( 11, -55 ) );
            XUtils.Assert( p == new xCoord( 11, -55 ) );

			XUtils.AssertVal( a.getLength(), 0, 0.0001 );
			XUtils.AssertVal( f.getLength(), System.Math.Sqrt( 40 ), 0.0001 );
			XUtils.AssertVal( g.getLength(), System.Math.Sqrt( 33 * 33 + 11 * 11 ), 0.001 );

            xCoord r = g.clamp( new xCoord( 0, 0 ), new xCoord( 100, 100 ) );
            xCoord s = g.clamp( new xCoord( -30, -10 ), new xCoord( 0, -1 ) );
            xCoord t = g.clamp( new xCoord( -1000, -1000 ), new xCoord( 1000, 1000 ) );
            xCoord u = g.clamp( new xCoord(), new xCoord() );
            XUtils.Assert( r == new xCoord( 0, 33 ) );
            XUtils.Assert( s == new xCoord( -11, -1 ) );
            XUtils.Assert( t == g );
            XUtils.Assert( u == new xCoord( 0, 0 ) );
        }
    }
}
