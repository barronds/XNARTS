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
				Init( style, text, out Vector2 label_size, out Label label );
				float padding = style.mButtonPadding;
				xAABB2 aabb = new xAABB2( pos, pos + label_size + 2.0f * new Vector2( padding, padding ) );
				Place( parent, style, aabb, state );
				PlaceLabel( label, style );
			}

			public Button( Widget parent, Style style, String text, ePlacement placement, eInitialState state )
			{
				Init( style, text, out Vector2 label_size, out Label label );
				Place( parent, style, placement, state );
				PlaceLabel( label, style );
			}

			private void Init( Style style, String text, out Vector2 label_size, out Label label )
			{
				mPressedVisual = false;

				label = new Label();
				AddChild( label );
				label.Assemble( style, text );
				label_size = label.GetAssembledSize();

				eFont font = style.mNormalFont;
				float padding = style.mButtonPadding;
				Vector2 size = label_size + 2.0f * new Vector2( padding, padding );
				AssembleWidget( size );
			}

			private void PlaceLabel( Label label, Style style )
			{
				label.Place( this, style, ePlacement.Centered, eInitialState.Dormant );
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
				base.Render( simple_draw );
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
