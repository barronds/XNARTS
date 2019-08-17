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
		private XBroadcaster< ButtonUpEvent >		_mBroadcaster_ButtonUpEvent;
		private XBroadcaster< ButtonDownEvent >		_mBroadcaster_ButtonDownEvent;
		private XBroadcaster< ButtonHeldEvent >		_mBroadcaster_ButtonHeldEvent;
		private XBroadcaster< ButtonAbortEvent >	_mBroadcaster_ButtonAbortEvent;
		private SortedList< long, _IButton >		_mButtons;
		private _IButton							_mCurrentlyPressed;

		public XBroadcaster<ButtonUpEvent> GetBroadcaster_ButtonUpEvent()
		{
			return _mBroadcaster_ButtonUpEvent;
		}
		public XBroadcaster<ButtonDownEvent> GetBroadcaster_ButtonDownEvent()
		{
			return _mBroadcaster_ButtonDownEvent;
		}
		public XBroadcaster<ButtonHeldEvent> GetBroadcaster_ButtonHeldEvent()
		{
			return _mBroadcaster_ButtonHeldEvent;
		}
		public XBroadcaster<ButtonAbortEvent> GetBroadcaster_ButtonAbortEvent()
		{
			return _mBroadcaster_ButtonAbortEvent;
		}

		public interface _IButton
		{
			bool Contains( Vector2 point );
			long GetID();
			void Draw( XSimpleDraw simple_draw );
			void SetPressed( bool pressed );
			void SetActive( bool active );
			bool IsActive();
			xAABB2 GetAABB();
			void Translate( Vector2 d );
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

		public _IButton _CreateRectangularButton( Vector2 pos,
												String text,
												eStyle style )
		{
			Style s = XUI.Instance().GetStyle( style );
			XFontDraw.FontInfo info = XFontDraw.Instance().GetFontInfo( s.mNormalFont );
			Vector2 font_size = info.mSize;
			_IButton button = new _RectangularButton( pos, text, s, NextUID(), font_size );
			_mButtons.Add( button.GetID(), button );
			return button;
		}

		public void _DestroyButton( _IButton button )
		{
			if ( _mCurrentlyPressed != null && _mCurrentlyPressed.GetID() == button.GetID() )
			{
				SendButtonAbortEvent();
			}

			_mButtons.Remove( button.GetID() );
		}

		private class _ButtonCore
		{
			public Vector2 mPos;
			public String mText;
			public Style mStyle;
			public Vector2 mTextOffset;
			public Color mPressedColor;
			public long mID;
			public bool mPressed;
			public bool mActive;

			public _ButtonCore( Vector2 pos,
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
				mActive = true;

				const float k_pressed_blend = 0.37f;
				mPressedColor = Color.Lerp( mStyle.mBackgroundColor, Color.White, k_pressed_blend );
			}

			public void Translate( Vector2 d )
			{
				mPos += d;
			}

			public void Draw( XSimpleDraw simple_draw )
			{
				// draw the text
				if( mText.Length > 50 )
				{
					Console.WriteLine( mTextOffset + ", " + mText );
				}
				XFontDraw.Instance().DrawString( mStyle.mNormalFont, mPos + mTextOffset, mStyle.mTextColor, mText );
			}
		}

		private class _RectangularButton : _IButton
		{
			private xAABB2 mAABB;
			private Vector3 mCorner2;
			private Vector3 mCorner3;
			private _ButtonCore mButtonCore;

			public _RectangularButton( Vector2 pos,
										String text,
										Style style,
										long id,
										Vector2 font_size )
			{
				// padding calculated, and button size and offset
				float k_padding = style.mButtonPadding;
				Vector2 new_text_offset = new Vector2( k_padding, k_padding );
				float button_width = text.Length * font_size.X + 2 * k_padding;
				float button_height = font_size.Y + 2 * k_padding;
				Vector2 new_size = new Vector2( button_width, button_height );
				mAABB = new xAABB2( pos, pos + new_size );
				mCorner2 = new Vector3( pos.X + new_size.X, pos.Y, 2 );
				mCorner3 = new Vector3( pos.X, pos.Y + new_size.Y, 2 );

				mButtonCore = new _ButtonCore( pos, text, style, new_text_offset, id );
			}

			long _IButton.GetID()
			{
				return mButtonCore.mID;
			}
			bool _IButton.Contains( Vector2 point )
			{
				return mAABB.Contains( point );
			}

			void _IButton.Draw( XSimpleDraw simple_draw )
			{
				// draw border and background, then draw core
				Vector3 lo = new Vector3( mAABB.GetMin(), 2 ); // zero might not be right z
				Vector3 hi = new Vector3( mAABB.GetMax(), 2 );

				Color body_color = mButtonCore.mPressed ? mButtonCore.mPressedColor : mButtonCore.mStyle.mBackgroundColor;
				Color border_color = mButtonCore.mStyle.mBorderColor;

				simple_draw.DrawQuad( lo, hi, body_color );

				simple_draw.DrawLine( lo, mCorner2, border_color );
				simple_draw.DrawLine( mCorner2, hi, border_color );
				simple_draw.DrawLine( hi, mCorner3, border_color );
				simple_draw.DrawLine( mCorner3, lo, border_color );

				mButtonCore.Draw( simple_draw );
			}

			void _IButton.SetPressed( bool pressed )
			{
				mButtonCore.mPressed = pressed;
			}
			void _IButton.SetActive( bool active )
			{
				mButtonCore.mActive = active;
			}
			bool _IButton.IsActive()
			{
				return mButtonCore.mActive;
			}
			xAABB2 _IButton.GetAABB()
			{
				return mAABB;
			}

			void _IButton.Translate( Vector2 d )
			{
				mAABB.Translate( d );
				Vector3 d3 = new Vector3( d, 0 );
				mCorner2 += d3;
				mCorner3 += d3;
				mButtonCore.Translate( d );
			}
		}

		private void SendButtonEvent< T >( bool pressed_now, XBroadcaster< T > b, T e ) where T : class
		{
			XUtils.Assert( _mCurrentlyPressed != null );
			_mCurrentlyPressed.SetPressed( pressed_now );
			b.Post( e );

			if( !pressed_now )
			{
				_mCurrentlyPressed = null;
			}
		}
		private void SendButtonUpEvent()
		{
			SendButtonEvent( false, _mBroadcaster_ButtonUpEvent, new ButtonUpEvent( _mCurrentlyPressed.GetID() ) );
		}
		private void SendButtonDownEvent()
		{
			SendButtonEvent( true, _mBroadcaster_ButtonDownEvent, new ButtonDownEvent( _mCurrentlyPressed.GetID() ) );
		}
		private void SendButtonHeldEvent()
		{
			SendButtonEvent( true, _mBroadcaster_ButtonHeldEvent, new ButtonHeldEvent( _mCurrentlyPressed.GetID() ) );
		}
		private void SendButtonAbortEvent()
		{
			SendButtonEvent( false, _mBroadcaster_ButtonAbortEvent, new ButtonAbortEvent( _mCurrentlyPressed.GetID() ) );
		}

		private void _Constructor_Buttons()
		{
			_mBroadcaster_ButtonUpEvent = new XBroadcaster<ButtonUpEvent>();
			_mBroadcaster_ButtonDownEvent = new XBroadcaster<ButtonDownEvent>();
			_mBroadcaster_ButtonHeldEvent = new XBroadcaster<ButtonHeldEvent>();
			_mBroadcaster_ButtonAbortEvent = new XBroadcaster<ButtonAbortEvent>();
			_mButtons = new SortedList<long, _IButton>();
			_mCurrentlyPressed = null;
		}
	}
}
