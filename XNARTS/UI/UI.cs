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
		XBroadcaster<ButtonEvent> XIBroadcaster<ButtonEvent>.GetBroadcaster()
		{
			return mBroadcaster_ButtonEvent;
		}

		private XBroadcaster< ButtonEvent > mBroadcaster_ButtonEvent;
		private XSimpleDraw					mSimpleDraw;
		private SortedList< long, IButton >	mButtons;
		private static long                 sPrevID = 0;
		private IButton                     mTestButton;

		public interface IButton
		{
			bool Contains( Vector2 point );
			long GetID();
			void Draw( XSimpleDraw simple_draw );
		}

		public class ButtonEvent
		{
			public enum Type
			{
				Invalid = -1,

				Down,
				Held,
				Up,

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
											Vector2 text_offset,
											Color text_color,
											Color background_color, 
											Color pressed_color,
											Color border_color, 
											double border_width )
		{
			IButton button = new RoundButton(	pos, radius, text, font, text_offset, text_color, background_color, 
												pressed_color, border_color, border_width, NextID() );
			mButtons.Add( button.GetID(), button );
			return button;
		}

		public IButton CreateRectangularButton( Vector2 pos,
												Vector2 size,
												String text,
												eFont font,
												Vector2 text_offset,
												Color text_color,
												Color background_color,
												Color pressed_color,
												Color border_color,
												double border_width )
		{
			IButton button = new RectangularButton(	pos, size, text, font, text_offset, text_color, background_color, 
													pressed_color, border_color, border_width, NextID() );
			mButtons.Add( button.GetID(), button );
			return button;
		}

		private class ButtonCore
		{
			public Vector2 mPos;
			public String mText;
			public eFont  mFont;
			public Vector2 mTextOffset;
			public Color mTextColor;
			public Color mBackgroundColor;
			public Color mPressedColor;
			public Color mBorderColor;
			public double mBorderWidth;
			public long mID;

			public ButtonCore(	Vector2 pos,
								String text, 
								eFont font,
								Vector2 text_offset,
								Color text_color,
								Color background_color, 
								Color pressed_color, 
								Color border_color, 
								double border_width,
								long id)
			{
				mPos = pos;
				mText = text;
				mFont = font;
				mTextOffset = text_offset;
				mTextColor = text_color;
				mBackgroundColor = background_color;
				mPressedColor = pressed_color;
				mBorderColor = border_color;
				mBorderWidth = border_width;
				mID = id;
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
								Vector2 text_offset,
								Color text_color,
								Color background_color, 
								Color pressed_color, 
								Color border_color, 
								double border_width, 
								long id )
			{
				mRadius = radius;
				mRadiusSqr = radius * radius;
				mButtonCore = new ButtonCore(	pos, text, font, text_offset, text_color, background_color, 
												pressed_color, border_color, border_width, id );
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
				// draw border and background, then draw core
				mButtonCore.Draw( simple_draw );
			}
		}

		private class RectangularButton : IButton
		{
			private xAABB2 mAABB;
			private ButtonCore mButtonCore;

			public RectangularButton(	Vector2 pos,
										Vector2 size,
										String text,
										eFont font,
										Vector2 text_offset,
										Color text_color,
										Color background_color,
										Color pressed_color,
										Color border_color,
										double border_width,
										long id )
			{
				mAABB = new xAABB2( pos, pos + size );
				mButtonCore = new ButtonCore(	pos, text, font, text_offset, text_color, background_color, 
												pressed_color, border_color, border_width, id );
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
				simple_draw.DrawQuad( lo, hi, mButtonCore.mBackgroundColor );
				mButtonCore.Draw( simple_draw );
			}
		}

		private XListener< XTouch.SinglePokeData > mListener_SinglePoke;

		private XUI()
		{
			// private constructor as per singleton
			mBroadcaster_ButtonEvent = new XBroadcaster<ButtonEvent>();
			mListener_SinglePoke = new XListener<XTouch.SinglePokeData>();
			mButtons = new SortedList<long, IButton>();
		}

		public void Init()
		{
			mSimpleDraw = XSimpleDraw.GetInstance( xeSimpleDrawType.ScreenSpace_Transient );
			((XIBroadcaster< XTouch.SinglePokeData >)XTouch.Instance()).GetBroadcaster().Subscribe( mListener_SinglePoke );

			mTestButton = CreateRectangularButton(	new Vector2( 400, 400 ), new Vector2( 200, 65 ), "First Button",
													eFont.Consolas16, new Vector2( 30, 20 ), Color.White, Color.Brown,
													Color.Beige, Color.White, 1 );
		}

		public void Update( GameTime t )
		{
			if( mListener_SinglePoke.GetNumEvents() > 0 )
			{
				XTouch.SinglePokeData data = mListener_SinglePoke.ReadNext();
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
