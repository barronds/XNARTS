using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace XNARTS
{
	public class XUI : XSingleton< XUI >, XIBroadcaster< XUI.ButtonEvent >
	{
		private XBroadcaster< ButtonEvent >		mBroadcaster_ButtonEvent;
		private XSimpleDraw						mSimpleDraw;
		private SortedList< long, IButton >		mButtons;
		private static long						sPrevID = 0;
		private IButton							mCurrentlyPressed;
		private Dictionary< eFont, Vector2 >    mFontSizes;

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

			public Type	mType;
			public long	mID;

		}

		public IButton CreateRoundButton(	Vector2 pos, 
											double radius, 
											String text,
											eFont font,
											Color text_color,
											Color background_color, 
											Color pressed_color,
											Color border_color )
		{
			IButton button = new RoundButton(	pos, radius, text, font, text_color, background_color, 
												border_color, NextID(), mFontSizes[ font ] );
			mButtons.Add( button.GetID(), button );
			return button;
		}

		public IButton CreateRectangularButton( Vector2 pos,
												String text,
												eFont font,
												Color text_color,
												Color background_color,
												Color border_color )
		{
			IButton button = new RectangularButton(	pos, text, font, text_color, background_color, 
													border_color, NextID(), mFontSizes[ font ] );
			mButtons.Add( button.GetID(), button );
			return button;
		}

		public void DestroyButton( IButton button )
		{
			if( mCurrentlyPressed != null && mCurrentlyPressed.GetID() == button.GetID() )
			{
				SendButtonEvent( ButtonEvent.Type.Abort );
			}

			mButtons.Remove( button.GetID() );
		}

		private class ButtonCore
		{
			public Vector2 mPos;
			public String mText;
			public eFont  mFont;
			public Vector2 mTextOffset;
			public Color mTextColor;
			public Color mBackgroundColor;
			public Color mBorderColor;
			public Color mPressedColor;
			public long mID;
			public bool mPressed;

			public ButtonCore(	Vector2 pos,
								String text, 
								eFont font,
								Vector2 text_offset,
								Color text_color,
								Color background_color, 
								Color border_color,
								long id )
			{
				mPos = pos;
				mText = text;
				mFont = font;
				mTextOffset = text_offset;
				mTextColor = text_color;
				mBackgroundColor = background_color;
				mBorderColor = border_color;
				mID = id;
				mPressed = false;

				const float k_pressed_blend = 0.37f;
				mPressedColor = Color.Lerp( mBackgroundColor, Color.White, k_pressed_blend );
			}

			public void Draw( XSimpleDraw simple_draw )
			{
				// draw the text
				XFontDraw.Instance().DrawString( mFont, mPos + mTextOffset, mTextColor, mText );
			}
		}


		private class RoundButton : IButton
		{
			private double mRadius;
			private double mRadiusSqr;
			private ButtonCore mButtonCore;

			public RoundButton( Vector2 pos, 
								double radius,	 
								String text, 
								eFont font,
								Color text_color,
								Color background_color, 
								Color border_color, 
								long id, 
								Vector2 font_size )
			{
				mRadius = radius;
				mRadiusSqr = radius * radius;
				// TODO: not finished, place text calculation
				mButtonCore = new ButtonCore( pos, text, font, new Vector2( 0, 0 ), text_color, background_color, border_color, id );
			}

			long IButton.GetID()
			{
				return mButtonCore.mID;
			}

			bool IButton.Contains(Vector2 point)
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

			public RectangularButton(	Vector2 pos,
										String text,
										eFont font,
										Color text_color,
										Color background_color,
										Color border_color,
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

				mButtonCore = new ButtonCore(	pos, text, font, new_text_offset, text_color, background_color, 
												border_color, id );
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
				Color body_color = mButtonCore.mPressed ? mButtonCore.mPressedColor : mButtonCore.mBackgroundColor;
				simple_draw.DrawQuad( lo, hi, body_color );
				simple_draw.DrawLine( lo, mCorner2, mButtonCore.mBorderColor, mButtonCore.mBorderColor );
				simple_draw.DrawLine( mCorner2, hi, mButtonCore.mBorderColor, mButtonCore.mBorderColor );
				simple_draw.DrawLine( hi, mCorner3, mButtonCore.mBorderColor, mButtonCore.mBorderColor );
				simple_draw.DrawLine( mCorner3, lo, mButtonCore.mBorderColor, mButtonCore.mBorderColor );
				mButtonCore.Draw( simple_draw );
			}

			void IButton.SetPressed( bool pressed )
			{
				mButtonCore.mPressed = pressed;
			}
		}

		private XListener< XTouch.SinglePokeData > mListener_SinglePoke;

		private XUI()
		{
			// private constructor as per singleton
			mBroadcaster_ButtonEvent = new XBroadcaster<ButtonEvent>();
			mListener_SinglePoke = new XListener<XTouch.SinglePokeData>();
			mButtons = new SortedList<long, IButton>();
			mCurrentlyPressed = null;

			// manually stock the font sizes dictionary with emperically determined sizes
			mFontSizes = new Dictionary<eFont, Vector2>();
			mFontSizes.Add( eFont.Consolas16, new Vector2( 12.0f, 19.0f ) );
		}

		public void Init()
		{
			mSimpleDraw = XSimpleDraw.GetInstance( xeSimpleDrawType.ScreenSpace_Transient );
			((XIBroadcaster< XTouch.SinglePokeData >)XTouch.Instance()).GetBroadcaster().Subscribe( mListener_SinglePoke );
		}

		private void SendButtonEvent( ButtonEvent.Type type )
		{
			XUtils.Assert( mCurrentlyPressed != null );
			bool pressed = type == ButtonEvent.Type.Down || type == ButtonEvent.Type.Held;
			mCurrentlyPressed.SetPressed( pressed );
			ButtonEvent e = new ButtonEvent( type, mCurrentlyPressed.GetID() );
			mBroadcaster_ButtonEvent.Post( e );
			
			if( !pressed )
			{
				mCurrentlyPressed = null;
			}
		}

		public void Update( GameTime t )
		{
			// things to check for:
			// - currently pressed button might trigger a button up
			// - currently pressed button might get aborted by touch gesture or drift
			// - curretnly pressed button might still be pressed
			// - on press start, a new button may be pressed

			XTouch.SinglePokeData data =    (mListener_SinglePoke.GetNumEvents() > 0)   ?
											mListener_SinglePoke.ReadNext()				:
											null                                        ;

			if ( mCurrentlyPressed != null )
			{
				XUtils.Assert( data != null, "should have hold, end, or abort" );

				if( data.mDetail == XTouch.ePokeDetail.Hold )
				{
					if( mCurrentlyPressed.Contains( data.mCurrentPos ) )
					{
						// pressed is still pressed
						SendButtonEvent( ButtonEvent.Type.Held );
					}
					else
					{
						// have strayed off button with a hold, un-press
						SendButtonEvent( ButtonEvent.Type.Abort );
					}
				}
				else if( data.mDetail == XTouch.ePokeDetail.End_Abort )
				{
					// touch decided this gesture is no good, un-press
					SendButtonEvent( ButtonEvent.Type.Abort );
				}
				else if( data.mDetail == XTouch.ePokeDetail.End_Normal )
				{
					// this is a pressed button
					SendButtonEvent( ButtonEvent.Type.Up );
				}
				else
				{
					XUtils.Assert( false, "not expecting another state when mCurrentlyPressed is valid" );
				}
			}
			else if( data != null && data.mDetail == XTouch.ePokeDetail.Start )
			{
				// new press, let's see if it hits a button
				var enumerator = mButtons.GetEnumerator();

				while ( enumerator.MoveNext() )
				{
					if ( enumerator.Current.Value.Contains( data.mCurrentPos ) )
					{
						mCurrentlyPressed = enumerator.Current.Value;
						SendButtonEvent( ButtonEvent.Type.Down );
						break;
					}
				}
			}
		}

		public void Draw()
		{
			var enumerator = mButtons.GetEnumerator();
			
			while( enumerator.MoveNext() )
			{
				enumerator.Current.Value.Draw( mSimpleDraw );
			}
		}

		private long NextID()
		{
			++sPrevID;
			return sPrevID;
		}
	}
}
