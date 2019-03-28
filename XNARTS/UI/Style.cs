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
			Tactical,
			Frontend,

			Num
		}

		public class Style
		{
			public Style(	eFont huge, eFont large, eFont medium, eFont small, eFont tiny, eFont very_tiny, 
							Color text, Color widget, Color button, Color border, float button_padding_scalar )
			{
				mHugeFont = huge;
				mLargeFont = large;
				mMediumFont = medium;
				mSmallFont = small;
				mTinyFont = tiny;
				mVeryTinyFont = very_tiny;
				mTextColor = text;
				mWidgetColor = widget;
				mButtonColor = button;
				mBorderColor = border;
				mButtonPaddingScalar = button_padding_scalar;
			}

			public eFont mHugeFont;
			public eFont mLargeFont;
			public eFont mMediumFont;
			public eFont mSmallFont;
			public eFont mTinyFont;
			public eFont mVeryTinyFont;
			public Color mTextColor;
			public Color mWidgetColor;
			public Color mButtonColor;
			public Color mBorderColor;
			public float mButtonPaddingScalar;
		}

		private Dictionary< eStyle, Style > mStyles;

		public Style GetStyle( eStyle style )
		{
			return mStyles[ style ];
		}
		private void AddStyle( eStyle key, Style value )
		{
			mStyles.Add( key, value );
		}

		private void Constructor_Style()
		{
			// manually stock styles
			mStyles = new Dictionary<eStyle, Style>();
			const float k_UI_alpha = 0.25f;
			const float k_Tactical_Alpha = 0.5f;

			AddStyle( eStyle.GameplayUI, new Style(	eFont.Consolas36, eFont.Consolas24, eFont.Consolas16, eFont.Consolas13, 
													eFont.Not_Available, eFont.Not_Available, Color.White, 
													new Color( Color.Gray, k_UI_alpha ), new Color( Color.Black, k_UI_alpha ), 
													Color.White, 0.65f ) );

			AddStyle( eStyle.Tactical, new Style(	eFont.LucidaConsole36, eFont.LucidaConsole24, eFont.LucidaConsole16, 
													eFont.LucidaConsole12, eFont.LucidaConsole10, eFont.LucidaConsole8, 
													new Color( Color.White, k_Tactical_Alpha ), Color.Transparent, 
													Color.Transparent, new Color( Color.White, k_Tactical_Alpha ), 0.65f ) );

			AddStyle( eStyle.Frontend, new Style(	eFont.Not_Available, eFont.Consolas36, eFont.Consolas36, eFont.Not_Available, 
													eFont.Not_Available, eFont.Not_Available, Color.White, Color.DarkViolet, 
													Color.DarkViolet, Color.Black, 0.0f ) );
		}
	}
}
