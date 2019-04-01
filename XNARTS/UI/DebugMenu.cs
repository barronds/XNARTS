using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNARTS
{
	class XDebugMenu : XSingleton< XDebugMenu >
	{
		// just holds the root of the debug menu.  
		// individual systems should own the rest fo the tree.


		private XDebugMenu()
		{ }
	}
}
