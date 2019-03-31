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

			FontTest,
			GameplayUI,
			Tactical,
			Frontend,
			FrontendButton,
			FrontendTitle,
			FrontendControl,

			Num
		}

		public class Style
		{
			public Style( eFont normal, eFont title, Color text, Color background, Color border, float button_padding_scalar )
			{
				mNormalFont = normal;
				mTitleFont = title;
				mTextColor = text;
				mBackgroundColor = background;
				mBorderColor = border;
				mButtonPaddingScalar = button_padding_scalar;
			}

			public eFont mNormalFont;
			public eFont mTitleFont;
			public Color mTextColor;
			public Color mBackgroundColor;
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
			Color ui_background = new Color( 0.35f, 0.35f, 0.35f, 1.0f );
			Color ui_meta = Color.Lerp( ui_background, Color.White, 0.5f );

			// fonttest: make a button with this to tune (discover) size and offset (change the font)
			AddStyle( eStyle.FontTest, new Style(	eFont.Consolas48, eFont.Not_Available, Color.White, 
													Color.DarkViolet, Color.Black, 0.0f ) );

			AddStyle( eStyle.GameplayUI, new Style(	eFont.Consolas16, eFont.Consolas24, Color.White, 
													new Color( Color.Black, k_UI_alpha ), Color.White, 0.65f ) );

			AddStyle( eStyle.Tactical, new Style(	eFont.LucidaConsole16, eFont.LucidaConsole24, 
													new Color( Color.White, k_Tactical_Alpha ), Color.Transparent,
													new Color( Color.White, k_Tactical_Alpha ), 0.65f ) );

			AddStyle( eStyle.Frontend, new Style(	eFont.Consolas36, eFont.Consolas48, Color.White,
													ui_background, Color.White, 0.0f ) );

			AddStyle( eStyle.FrontendButton, new Style( eFont.Consolas36, eFont.Consolas48, Color.White,
														ui_background, ui_background, 0.0f ) );

			AddStyle( eStyle.FrontendTitle, new Style(	eFont.Consolas48, eFont.Not_Available, ui_meta,
														ui_background, ui_background, 0.0f ) );

			AddStyle( eStyle.FrontendControl, new Style(	eFont.Consolas36, eFont.Not_Available, ui_meta,
															ui_background, ui_background, 0.0f ) );
		}
	}
}
