using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using XNARTS.UnitTests;

namespace XNARTS.RTSMath
{
    struct tCoord
    {
        public int x { get; set; }
        public int y { get; set; }

        public tCoord( int x, int y )
        {
            this.x = x;
            this.y = y;
        }

        public tCoord( double x, double y )
        {
            this.x = (int)x;
            this.y = (int)y;
        }

        public float getLength()
        {
            return (float)System.Math.Sqrt( this.x * this.x + this.y * this.y );
        }

        public tCoord clamp( tCoord min, tCoord max )
        {
            Debug.Assert( !(min.x > max.x || min.y > max.y) );
            int clamped_x = System.Math.Max( System.Math.Min( max.x, this.x ), min.x );
            int clamped_y = System.Math.Max( System.Math.Min( max.y, this.y ), min.y );
            return new tCoord( clamped_x, clamped_y );
        }

        public static tCoord operator -( tCoord coord )
        {
            return new tCoord( 0 - coord.x, 0 - coord.y );
        }

        public static tCoord operator *( float s, tCoord coord )
        {
            return new tCoord( coord.x * s, coord.y * s );
        }

        public static tCoord operator *( tCoord coord, float s )
        {
            return s * coord;
        }

        public static tCoord operator +( tCoord a, tCoord b )
        {
            return new tCoord( a.x + b.x, a.y + b.y );
        }

        public static tCoord operator -( tCoord a, tCoord b )
        {
            return new tCoord( a.x - b.x, a.y - b.y );
        }

        public static bool operator ==( tCoord a, tCoord b )
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=( tCoord a, tCoord b )
        {
            return a.x != b.x || a.y != b.y;
        }

        public override bool Equals( Object obj )
        {
            if ( obj is tCoord )
            {
                return (tCoord)obj == this;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static void unitTest()
        {
            //Framework.Assert( false, "hello" );
            tCoord a = new tCoord();
            Framework.Assert( a.x == 0 );
            Framework.Assert( a.y == 0 );

            tCoord b = a;
            Framework.Assert( b.x == 0 );
            Framework.Assert( b.y == 0 );

            tCoord c = new tCoord( -10, 20.0f );
            Framework.Assert( c.x == -10 );
            Framework.Assert( c.y == 20 );

            tCoord d = 5 * c;
            tCoord e = new tCoord( -50, 100 );
            Framework.Assert( d == e );
            Framework.Assert( d != b );
            Framework.Assert( d.Equals( e ) );
            Framework.Assert( !d.Equals( a ) );

#pragma warning disable CS1718
            Framework.Assert( d == d );
#pragma warning restore CS1718

            tCoord f = new tCoord( 2, 6 );
            tCoord g = new tCoord( -11, 33 );
            tCoord h = f + g;
            tCoord i = f - g;
            tCoord j = f - f;
            tCoord k = g - g - g;
            Framework.Assert( h == new tCoord( -9, 39 ) );
            Framework.Assert( i == new tCoord( 13, -27 ) );
            Framework.Assert( j == new tCoord() );
            Framework.Assert( k == -g );

            tCoord l = new tCoord( 2, -5 );
            tCoord m = 4 * l;
            tCoord n = -3.5f * l;
            tCoord o = l * 4;
            Framework.Assert( m == new tCoord( 8, -20 ) );
            Framework.Assert( n == new tCoord( -7, 17 ) );
            Framework.Assert( o == m );

            tCoord p = new tCoord( 11.4, -55.6 );
            tCoord q = new tCoord( 11.6, -55.4 );
            Framework.Assert( p == new tCoord( 11, -55 ) );
            Framework.Assert( p == new tCoord( 11, -55 ) );

            Framework.AssertVal( a.getLength(), 0, 0.0001 );
            Framework.AssertVal( f.getLength(), Math.Sqrt( 40 ), 0.0001 );
            Framework.AssertVal( g.getLength(), Math.Sqrt( 33 * 33 + 11 * 11 ), 0.001 );

            tCoord r = g.clamp( new tCoord( 0, 0 ), new tCoord( 100, 100 ) );
            tCoord s = g.clamp( new tCoord( -30, -10 ), new tCoord( 0, -1 ) );
            tCoord t = g.clamp( new tCoord( -1000, -1000 ), new tCoord( 1000, 1000 ) );
            tCoord u = g.clamp( new tCoord(), new tCoord() );
            Framework.Assert( r == new tCoord( 0, 33 ) );
            Framework.Assert( s == new tCoord( -11, -1 ) );
            Framework.Assert( t == g );
            Framework.Assert( u == new tCoord( 0, 0 ) );
        }
    }
}
