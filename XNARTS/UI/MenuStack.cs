using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNARTS
{
	// see Menu for design

	// assumed at this time that we would only want one global menu stack, as it is not ui practice to be able
	// to navigate two or more menus at once.  also facilitates global access.

	public class MenuStack : XSingleton< MenuStack >
	{

		private MenuStack()
		{
			// as per XSingleton
		}
	}
}
