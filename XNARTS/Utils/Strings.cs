using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNARTS
{
	public partial class XUtils
	{
		public static String GetNSpaces( int n )
		{
			XUtils.Assert( n > -1 );

			switch ( n )
			{
				case 0: return "";
				case 1: return " ";
				case 2: return "  ";
				case 3: return "   ";
				case 4: return "    ";
				case 5: return "     ";
				case 6: return "      ";
				case 7: return "       ";
				default: return "        " + GetNSpaces( n - 8 );
			}
		}

	}
}
