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
			FrontendTest,
			Screen,

			Num
		}

		public class Style
		{
			public eFont	mNormalFont;
			public eStyle	mEnumValue;
			public Color	mTextColor;
			public Color	mBackgroundColor;
			public Color    mInteractionBackgroundColor;
			public Color	mBorderColor;
			public float	mButtonPadding;
			public float	mPlacementPadding;
			public float    mPackingPadding;

			public Style(	eStyle enum_value, eFont normal, Color text, Color background, Color interation_background, 
							Color border, float button_padding_scalar, float placement_padding_scalar, 
							float packing_padding_scalar )
			{
				mEnumValue = enum_value;
				mNormalFont = normal;
				mTextColor = text;
				mBackgroundColor = background;
				mInteractionBackgroundColor = interation_background;
				mBorderColor = border;

				CalcPadding( button_padding_scalar, placement_padding_scalar, packing_padding_scalar );
			}

			private void CalcPadding( float button_padding_scalar, float placement_padding_scalar, float packing_padding_scalar )
			{
				Vector2 font_size = XFontDraw.Instance().GetFontInfo( mNormalFont ).mSize;
				float heuristic_size = font_size.X + font_size.Y;
				mButtonPadding = button_padding_scalar * heuristic_size;
				mPlacementPadding = placement_padding_scalar * heuristic_size;
				mPackingPadding = packing_padding_scalar * heuristic_size;
			}
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
			const float k_pressed_blend = 0.37f;
			Color ui_background = new Color( 0.35f, 0.35f, 0.35f, 1.0f );
			Color ui_meta = Color.Lerp( ui_background, Color.White, 0.5f );
			Color ui_alpha_black = new Color( Color.Black, k_UI_alpha );

			// fonttest: make a button with this to tune (discover) size and offset (change the font)
			AddStyle( eStyle.FontTest, new Style(	eStyle.FontTest, eFont.Consolas48, Color.White, Color.DarkViolet,
													Color.Lerp( Color.DarkViolet, Color.White, k_pressed_blend ),
													Color.Black, 0.0f, 0.0f, 0.0f ) );

			AddStyle( eStyle.GameplayUI, new Style( eStyle.GameplayUI, eFont.Consolas16, Color.White,
													ui_alpha_black, Color.Lerp( ui_alpha_black, Color.White, k_pressed_blend ),
													Color.White, 0.65f, 1.3f, 0.325f ) );

			AddStyle( eStyle.Tactical, new Style(	eStyle.Tactical, eFont.LucidaConsole16, 
													new Color( Color.White, k_Tactical_Alpha ), Color.Transparent,
													Color.Lerp( Color.Transparent, Color.White, k_pressed_blend ),
													new Color( Color.White, k_Tactical_Alpha ), 0.65f, 1.3f, 0.325f ) );

			AddStyle( eStyle.Frontend, new Style(	eStyle.Frontend, eFont.Consolas36, Color.White, ui_background,
													Color.Lerp( ui_background, Color.White, k_pressed_blend ),
													Color.White, 0.0f, 0.0f, 0.0f ) );

			AddStyle( eStyle.FrontendButton, new Style(	eStyle.FrontendButton, eFont.Consolas36, Color.White, ui_background,
														Color.Lerp( Color.DarkViolet, Color.White, k_pressed_blend ),
														ui_background, 0.0f, 0.0f, 0.0f ) );

			AddStyle( eStyle.FrontendTest, new Style(	eStyle.FrontendTest, eFont.Consolas36, Color.White, ui_background,
														Color.Lerp( ui_background, Color.White, k_pressed_blend ),
														Color.White, 0.25f, 0.5f, 0.125f ) );

			AddStyle( eStyle.FrontendTitle, new Style(	eStyle.FrontendTitle, eFont.Consolas48, ui_meta, ui_background,
														Color.Lerp( ui_background, Color.White, k_pressed_blend ),
														ui_background, 0.0f, 0.0f, 0.0f ) );

			AddStyle( eStyle.FrontendControl, new Style(	eStyle.FrontendControl, eFont.Consolas36, ui_meta, ui_background,
															Color.Lerp( ui_background, Color.White, k_pressed_blend ),
															ui_background, 0.0f, 0.0f, 0.0f ) );

			AddStyle( eStyle.Screen, new Style( eStyle.Screen, eFont.Consolas36, ui_meta, ui_background,
												Color.Lerp( ui_background, Color.White, k_pressed_blend ),
												ui_background, 2.0f, 4.0f, 1.0f ) );
		}
	}
}
