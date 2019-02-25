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
	}
}
