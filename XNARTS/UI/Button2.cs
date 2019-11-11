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
		public class Button : Panel
		{
			private bool mPressedVisual;

			public Button( Widget parent, Style style, String text, Vector2 pos, eInitialState state )
			{
				mPressedVisual = false;

				Label label = new Label( text );
				label.Asssemble( style );
				Vector2 label_size = label.GetSize();

				eFont font = style.mNormalFont;
				Vector2 font_size = XFontDraw.Instance().GetFontInfo( font ).mSize;
				float padding = style.mButtonPadding;
				xAABB2 aabb = new xAABB2( pos, pos + label_size + 2.0f * new Vector2( padding, padding ) );
				InitPanel( parent, style, aabb, state );

				label.Place( this, style, ePlacement.Centered, eInitialState.Dormant );
				//Label label = new Label( this, text, style, ePlacement.Centered, eInitialState.Dormant );
				AddChild( label );
			}

			public Button( Widget parent, Style style, String text, ePlacement placement, eInitialState state )
			{
				Label label = new Label( text );
				label.Asssemble( style );
				Vector2 label_size = label.GetSize();

				mPressedVisual = false;
				eFont font = style.mNormalFont;
				Vector2 font_size = XFontDraw.Instance().GetFontInfo( font ).mSize;
				float padding = style.mButtonPadding;
				Vector2 size = label_size + 2.0f * new Vector2( padding, padding );
				InitPanel( parent, style, size, placement, state );

				label.Place( this, style, ePlacement.Centered, eInitialState.Dormant );
				//Label label = new Label( this, text, style, ePlacement.Centered, eInitialState.Dormant );
				AddChild( label );
			}

			public void SetPressedVisual( bool pressed )
			{
				mPressedVisual = pressed;
			}

			public override void FacilitateInteractability( bool interactable )
			{
				if( interactable )
				{
					XUI.Instance().AddActiveButton( this );
				}
				else
				{
					XUI.Instance().RemoveActiveButton( this );
				}
			}

			public override void Render( XSimpleDraw simple_draw )
			{
				XUtils.Assert( IsInitialized() );
				xAABB2 aabb = GetPosition().GetScreenAABB();
				Style s = GetStyle();
				Color border = s.mBorderColor;
				Color background = mPressedVisual ? s.mInteractionBackgroundColor : s.mBackgroundColor;
				XUI.Instance().Util_DrawBox( simple_draw, background, s.mBorderColor, aabb );
				RenderChildren( simple_draw );
			}
		}

		private void SendButtonEvent<T>( bool pressed_now, XBroadcaster<T> b, T e ) where T : class
		{
			XUtils.Assert( mCurrentlyPressed != null );
			mCurrentlyPressed.SetPressedVisual( pressed_now );
			b.Post( e );

			if ( !pressed_now )
			{
				mCurrentlyPressed = null;
			}
		}
		private void SendButtonUpEvent()
		{
			SendButtonEvent( false, mBroadcaster_ButtonUpEvent, new ButtonUpEvent( mCurrentlyPressed.GetUID() ) );
		}
		private void SendButtonDownEvent()
		{
			SendButtonEvent( true, mBroadcaster_ButtonDownEvent, new ButtonDownEvent( mCurrentlyPressed.GetUID() ) );
		}
		private void SendButtonHeldEvent()
		{
			SendButtonEvent( true, mBroadcaster_ButtonHeldEvent, new ButtonHeldEvent( mCurrentlyPressed.GetUID() ) );
		}
		private void SendButtonAbortEvent()
		{
			SendButtonEvent( false, mBroadcaster_ButtonAbortEvent, new ButtonAbortEvent( mCurrentlyPressed.GetUID() ) );
		}

		public XBroadcaster< ButtonUpEvent >		mBroadcaster_ButtonUpEvent;
		public XBroadcaster< ButtonDownEvent >		mBroadcaster_ButtonDownEvent;
		public XBroadcaster< ButtonHeldEvent >		mBroadcaster_ButtonHeldEvent;
		public XBroadcaster< ButtonAbortEvent >		mBroadcaster_ButtonAbortEvent;
		private XListener< XTouch.SinglePokeData >  mListener_SinglePoke;
		private Button                              mCurrentlyPressed;

		// all the buttons on the currently active UI 'layer'.  ie, could have input disabled and still be in this list.
		// ie, all buttons that could be interacted with should they have their input enabled.
		private List< Button >                      mActiveButtons; 

		public void Constructor_Buttons()
		{
			mBroadcaster_ButtonUpEvent = new XBroadcaster<ButtonUpEvent>();
			mBroadcaster_ButtonDownEvent = new XBroadcaster<ButtonDownEvent>();
			mBroadcaster_ButtonHeldEvent = new XBroadcaster<ButtonHeldEvent>();
			mBroadcaster_ButtonAbortEvent = new XBroadcaster<ButtonAbortEvent>();
			mActiveButtons = new List<Button>();
			mCurrentlyPressed = null;
		}
	}
}
