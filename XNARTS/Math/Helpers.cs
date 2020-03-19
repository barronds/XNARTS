using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNARTS
{
	public class XMath
	{
		public static float Abs( float f )
		{
			return f > 0f ? f : -f;
		}

		public static double Clamp( double val, double min, double max )
		{
			return val < min ? min : val > max ? max : val;
		}

		public static float Clamp( float val, float min, float max )
		{
			return val < min ? min : val > max ? max : val;
		}

		public static double Sqr( double val )
		{
			return val * val;
		}

		public static float Sqr( float val )
		{
			return val * val;
		}

		public static float Sqrt( float val )
		{
			return (float)Math.Sqrt( (double)val );
		}

		public static float MaxArr( float[] values )
		{
			XUtils.Assert( values.Length > 0 );
			float max = values[ 0 ];

			for( int i = 1; i < values.Length; ++i )
			{
				max = values[ i ] > max ? values[ i ] : max;
			}

			return max;
		}
	}
}
