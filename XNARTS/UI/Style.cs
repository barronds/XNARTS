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
		public enum eStyle
		{
			Invalid = -1,

			GameplayUI,
			Frontend,

			Num
		}

		private class Style
		{
			public Style( eFont large, eFont medium, eFont small, Color text, Color widget, Color button, Color border )
			{
				mLargeFont = large;
				mMediumFont = medium;
				mSmallFont = small;
				mTextColor = text;
				mWidgetColor = widget;
				mButtonColor = button;
				mBorderColor = border;
			}

			public eFont mLargeFont;
			public eFont mMediumFont;
			public eFont mSmallFont;
			public Color mTextColor;
			public Color mWidgetColor;
			public Color mButtonColor;
			public Color mBorderColor;
		}

		private Dictionary< eStyle, Style > mStyles;

		private void Constructor_Style()
		{
			// manually stock styles
			mStyles = new Dictionary<eStyle, Style>();

		}
	}
}
