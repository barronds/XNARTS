using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace XNARTS
{
	public partial class XUI
	{
		private Dictionary< eFont, Vector2 > mFontSizes;

		private void Constructor_Fonts()
		{
			// manually stock the font sizes dictionary with emperically determined sizes
			mFontSizes = new Dictionary<eFont, Vector2>();
			mFontSizes.Add( eFont.Consolas16, new Vector2( 12.0f, 19.0f ) );
		}
	}
}
