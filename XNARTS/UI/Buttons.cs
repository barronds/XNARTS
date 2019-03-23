using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace XNARTS
{
	public partial class XUI : XIBroadcaster<XUI.ButtonEvent>
	{
		private XBroadcaster< ButtonEvent >	mBroadcaster_ButtonEvent;
		private SortedList< long, IButton >	mButtons;
		private IButton						mCurrentlyPressed;

		XBroadcaster<ButtonEvent> XIBroadcaster<ButtonEvent>.GetBroadcaster()
		{
			return mBroadcaster_ButtonEvent;
		}
		public interface IButton
		{
			bool Contains( Vector2 point );
			long GetID();
			void Draw( XSimpleDraw simple_draw );
			void SetPressed( bool pressed );
		}
		public class ButtonEvent
		{
			public enum Type
			{
				Invalid = -1,

				Down,
				Held,
				Up,
				Abort, // non-triggering up, end to a hold

				Num
			}

			public ButtonEvent( Type type, long id )
			{
				mType = type;
				mID = id;
			}

			public Type mType;
			public long mID;

		}

		public IButton CreateRoundButton(	Vector2 pos,
											String text,
											eStyle style )
		{
			Style s = XUI.Instance().GetStyle( style );
			Vector2 font_size = mFontSizes[ s.mMediumFont ];
			IButton button = new RoundButton( pos, text, s, NextID(), font_size );
			mButtons.Add( button.GetID(), button );
			return button;
		}

		public IButton CreateRectangularButton( Vector2 pos,
												String text,
												eStyle style )
		{
			Style s = XUI.Instance().GetStyle( style );
			Vector2 font_size = mFontSizes[ s.mMediumFont ];
			IButton button = new RectangularButton( pos, text, s, NextID(), font_size );
			mButtons.Add( button.GetID(), button );
			return button;
		}

		public void DestroyButton( IButton button )
		{
			if ( mCurrentlyPressed != null && mCurrentlyPressed.GetID() == button.GetID() )
			{
				SendButtonEvent( ButtonEvent.Type.Abort );
			}

			mButtons.Remove( button.GetID() );
		}

		private class ButtonCore
		{
			public Vector2 mPos;
			public String mText;
			public Style mStyle;
			public Vector2 mTextOffset;
			public Color mPressedColor;
			public long mID;
			public bool mPressed;

			public ButtonCore( Vector2 pos,
								String text,
								Style style,
								Vector2 text_offset,
								long id )
			{
				mPos = pos;
				mText = text;
				mStyle = style;
				mTextOffset = text_offset;
				mID = id;
				mPressed = false;

				const float k_pressed_blend = 0.37f;
				mPressedColor = Color.Lerp( mStyle.mButtonColor, Color.White, k_pressed_blend );
			}

			public void Draw( XSimpleDraw simple_draw )
			{
				// draw the text
				XFontDraw.Instance().DrawString( mStyle.mMediumFont, mPos + mTextOffset, mStyle.mTextColor, mText );
			}
		}


		private class RoundButton : IButton
		{
			private double mRadius;
			private double mRadiusSqr;
			private ButtonCore mButtonCore;

			public RoundButton( Vector2 pos,
								String text,
								Style style,
								long id,
								Vector2 font_size )
			{
				// TODO: calc radius from font size
				float radius = 100;
				mRadius = radius;
				mRadiusSqr = radius * radius;
				// TODO: not finished, place text calculation
				mButtonCore = new ButtonCore( pos, text, style, new Vector2( 0, 0 ), id );
			}

			long IButton.GetID()
			{
				return mButtonCore.mID;
			}

			bool IButton.Contains( Vector2 point )
			{
				Vector2 d = point - mButtonCore.mPos;
				double dist_sqr = d.LengthSquared();
				return dist_sqr < mRadiusSqr;
			}

			void IButton.Draw( XSimpleDraw simple_draw )
			{
				// draw border and background, then draw 
				// TODO: circle part, add circles to simple draw
				mButtonCore.Draw( simple_draw );
			}

			void IButton.SetPressed( bool pressed )
			{
				mButtonCore.mPressed = pressed;
			}
		}

		private class RectangularButton : IButton
		{
			private xAABB2 mAABB;
			private Vector3 mCorner2;
			private Vector3 mCorner3;
			private ButtonCore mButtonCore;

			public RectangularButton( Vector2 pos,
										String text,
										Style style,
										long id,
										Vector2 font_size )
			{
				// padding calculated, and button size and offset
				const float k_padding_scalar = 0.65f;
				float k_padding = k_padding_scalar * (font_size.X + font_size.Y);
				Vector2 new_text_offset = new Vector2( k_padding, k_padding );
				float button_width = text.Length * font_size.X + 2 * k_padding;
				float button_height = font_size.Y + 2 * k_padding;
				Vector2 new_size = new Vector2( button_width, button_height );
				mAABB = new xAABB2( pos, pos + new_size );
				mCorner2 = new Vector3( pos.X + new_size.X, pos.Y, 2 );
				mCorner3 = new Vector3( pos.X, pos.Y + new_size.Y, 2 );

				mButtonCore = new ButtonCore( pos, text, style, new_text_offset, id );
			}

			long IButton.GetID()
			{
				return mButtonCore.mID;
			}

			bool IButton.Contains( Vector2 point )
			{
				return mAABB.Contains( point );
			}

			void IButton.Draw( XSimpleDraw simple_draw )
			{
				// draw border and background, then draw core
				Vector3 lo = new Vector3( mAABB.GetMin(), 2 ); // zero might not be right z
				Vector3 hi = new Vector3( mAABB.GetMax(), 2 );
				Color body_color = mButtonCore.mPressed ? mButtonCore.mPressedColor : mButtonCore.mStyle.mButtonColor;
				simple_draw.DrawQuad( lo, hi, body_color );
				Color border_color = mButtonCore.mStyle.mBorderColor;
				simple_draw.DrawLine( lo, mCorner2, border_color, border_color );
				simple_draw.DrawLine( mCorner2, hi, border_color, border_color );
				simple_draw.DrawLine( hi, mCorner3, border_color, border_color );
				simple_draw.DrawLine( mCorner3, lo, border_color, border_color );
				mButtonCore.Draw( simple_draw );
			}

			void IButton.SetPressed( bool pressed )
			{
				mButtonCore.mPressed = pressed;
			}
		}

		private void SendButtonEvent( ButtonEvent.Type type )
		{
			XUtils.Assert( mCurrentlyPressed != null );
			bool pressed = type == ButtonEvent.Type.Down || type == ButtonEvent.Type.Held;
			mCurrentlyPressed.SetPressed( pressed );
			ButtonEvent e = new ButtonEvent( type, mCurrentlyPressed.GetID() );
			mBroadcaster_ButtonEvent.Post( e );

			if ( !pressed )
			{
				mCurrentlyPressed = null;
			}
		}

		private void Constructor_Buttons()
		{
			mBroadcaster_ButtonEvent = new XBroadcaster<ButtonEvent>();
			mButtons = new SortedList<long, IButton>();
			mCurrentlyPressed = null;
		}

		private void Draw_Buttons()
		{
			var enumerator = mButtons.GetEnumerator();

			while ( enumerator.MoveNext() )
			{
				enumerator.Current.Value.Draw( mSimpleDraw );
			}
		}
	}
}
