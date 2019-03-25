using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace XNARTS
{
	public partial class XUI :	XIBroadcaster<XUI.ButtonUpEvent>,
								XIBroadcaster<XUI.ButtonDownEvent>,
								XIBroadcaster<XUI.ButtonHeldEvent>,
								XIBroadcaster<XUI.ButtonAbortEvent>
	{
		private XBroadcaster< ButtonUpEvent >		mBroadcaster_ButtonUpEvent;
		private XBroadcaster< ButtonDownEvent >		mBroadcaster_ButtonDownEvent;
		private XBroadcaster< ButtonHeldEvent >		mBroadcaster_ButtonHeldEvent;
		private XBroadcaster< ButtonAbortEvent >	mBroadcaster_ButtonAbortEvent;
		private SortedList< long, IButton >			mButtons;
		private IButton								mCurrentlyPressed;

		XBroadcaster<ButtonUpEvent> XIBroadcaster<ButtonUpEvent>.GetBroadcaster()
		{
			return mBroadcaster_ButtonUpEvent;
		}
		XBroadcaster<ButtonDownEvent> XIBroadcaster<ButtonDownEvent>.GetBroadcaster()
		{
			return mBroadcaster_ButtonDownEvent;
		}
		XBroadcaster<ButtonHeldEvent> XIBroadcaster<ButtonHeldEvent>.GetBroadcaster()
		{
			return mBroadcaster_ButtonHeldEvent;
		}
		XBroadcaster<ButtonAbortEvent> XIBroadcaster<ButtonAbortEvent>.GetBroadcaster()
		{
			return mBroadcaster_ButtonAbortEvent;
		}
		public interface IButton
		{
			bool Contains( Vector2 point );
			long GetID();
			void Draw( XSimpleDraw simple_draw );
			void SetPressed( bool pressed );
		}

		public class ButtonDownEvent
		{
			private long mID;

			public ButtonDownEvent( long id )
			{
				mID = id;
			}
		}
		public class ButtonUpEvent
		{
			public long mID;

			public ButtonUpEvent( long id )
			{
				mID = id;
			}
		}
		public class ButtonHeldEvent
		{
			private long mID;

			public ButtonHeldEvent( long id )
			{
				mID = id;
			}
		}
		public class ButtonAbortEvent
		{
			private long mID;

			public ButtonAbortEvent( long id )
			{
				mID = id;
			}
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
				SendButtonAbortEvent();
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

		private void SendButtonUpEvent()
		{
			XUtils.Assert( mCurrentlyPressed != null );
			mCurrentlyPressed.SetPressed( false );
			ButtonUpEvent e = new ButtonUpEvent( mCurrentlyPressed.GetID() );
			mBroadcaster_ButtonUpEvent.Post( e );
			mCurrentlyPressed = null;
		}
		private void SendButtonDownEvent()
		{
			XUtils.Assert( mCurrentlyPressed != null );
			mCurrentlyPressed.SetPressed( true );
			ButtonDownEvent e = new ButtonDownEvent( mCurrentlyPressed.GetID() );
			mBroadcaster_ButtonDownEvent.Post( e );
		}
		private void SendButtonHeldEvent()
		{
			XUtils.Assert( mCurrentlyPressed != null );
			mCurrentlyPressed.SetPressed( true );
			ButtonHeldEvent e = new ButtonHeldEvent( mCurrentlyPressed.GetID() );
			mBroadcaster_ButtonHeldEvent.Post( e );
		}
		private void SendButtonAbortEvent()
		{
			XUtils.Assert( mCurrentlyPressed != null );
			mCurrentlyPressed.SetPressed( false );
			ButtonAbortEvent e = new ButtonAbortEvent( mCurrentlyPressed.GetID() );
			mBroadcaster_ButtonAbortEvent.Post( e );
			mCurrentlyPressed = null;
		}

		private void Constructor_Buttons()
		{
			mBroadcaster_ButtonUpEvent = new XBroadcaster<ButtonUpEvent>();
			mBroadcaster_ButtonDownEvent = new XBroadcaster<ButtonDownEvent>();
			mBroadcaster_ButtonHeldEvent = new XBroadcaster<ButtonHeldEvent>();
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
