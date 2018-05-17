using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNARTS
{
	public class XConfig : XSingleton< XConfig >
	{
		public xCoord mMapSize = new xCoord( 16, 9 );


		// private constructor for XSingleton
		private XConfig()
		{}
	}
}
