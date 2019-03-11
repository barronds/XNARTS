using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace XNARTS
{
	public class XUI : XSingleton< XUI >
	{
		public XBroadcaster< ButtonEvent > mBroadcaster_ButtonEvent;

		public class ElementID : IEquatable< ElementID >
		{
			private long mID;
			private static long sNext = 1;

			public ElementID()
			{
				mID = sNext;
				++sNext;
			}

			public bool Equals( ElementID other )
			{
				return mID == other.mID;
			}

			public static bool operator==( ElementID lhs, ElementID rhs )
			{
				return lhs.Equals( rhs );
			}

			public static bool operator !=( ElementID lhs, ElementID rhs )
			{
				return !(lhs.Equals( rhs ));
			}

			public override bool Equals( Object obj )
			{
				if ( (obj == null) || !this.GetType().Equals( obj.GetType() ) )
				{
					return false;
				}
				else
				{
					ElementID e = (ElementID)obj;
					return e == this;
				}
			}

			public override int GetHashCode()
			{
				return (int)mID;
			}
		}
		public interface IButton
		{
			bool		Contains( Vector2 point );
			ElementID	GetID();
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

			public ButtonEvent( Type type, ElementID id )
			{
				mType = type;
				mID = id;
			}

			public Type			mType;
			public ElementID	mID;

		}

		public IButton CreateRoundButton(	Vector2 pos, 
											double radius, 
											String text, 
											Color color, 
											Color pressed_color,
											Color border_color, 
											double border_width )
		{
			return new RoundButton( pos, radius, text, color, pressed_color, border_color, border_width );
		}


		private class ButtonCore
		{
			public String mText;
			public Color mColor;
			public Color mPressedColor;
			public Color mBorderColor;
			public double mBorderWidth;
			public ElementID mID;

			public ButtonCore( String text, Color color, Color pressed_color, Color border_color, double border_width )
			{
				mText = text;
				mColor = color;
				mPressedColor = pressed_color;
				mBorderColor = border_color;
				mBorderWidth = border_width;
				mID = new ElementID();
			}
		}


		private class RoundButton : IButton
		{
			private Vector2 mPos;
			private double mRadius;
			private double mRadiusSqr;
			private ButtonCore mButtonCore;

			public RoundButton( Vector2 pos, 
								double radius,	 
								String text, 
								Color color, 
								Color pressed_color, 
								Color border_color, 
								double border_width )
			{
				mPos = pos;
				mRadius = radius;
				mRadiusSqr = radius * radius;
				mButtonCore = new ButtonCore( text, color, pressed_color, border_color, border_width );
			}

			ElementID IButton.GetID()
			{
				return mButtonCore.mID;
			}

			bool IButton.Contains(Vector2 point)
			{
				Vector2 d = point - mPos;
				double dist_sqr = d.LengthSquared();
				return dist_sqr < mRadiusSqr;
			}
		}

		private class RectangularButton : IButton
		{
			private xAABB2 mAABB;
			private ButtonCore mButtonCore;

			public RectangularButton(	Vector2 lo,
										Vector2 hi,
										String text,
										Color color,
										Color pressed_color,
										Color border_color,
										double border_width )
			{
				mAABB = new xAABB2( lo, hi );
				mButtonCore = new ButtonCore( text, color, pressed_color, border_color, border_width );
			}

			ElementID IButton.GetID()
			{
				return mButtonCore.mID;
			}

			bool IButton.Contains( Vector2 point )
			{
				return mAABB.Contains( point );
			}
		}

		private XUI()
		{
			// private constructor as per singleton
			mBroadcaster_ButtonEvent = new XBroadcaster<ButtonEvent>();
		}
	}
}
