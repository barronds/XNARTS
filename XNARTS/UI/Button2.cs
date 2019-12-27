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
			private bool	mPressedVisual;
			private Label   mLabel;

			public Button()
			{
				mLabel = new Label();
				AddChild( mLabel );
			}

			public void AssembleButton( Style style, String text )
			{
				mPressedVisual = false;
				mLabel.AssembleLabel( style, text );
				eFont font = style.mNormalFont;
				float padding = style.mButtonPadding;
				Vector2 size = mLabel.GetAssembledSize() + 2.0f * new Vector2( padding, padding );
				AssembleWidget( size );
			}

			public void PlaceButton( Widget parent, Style style, Vector2 pos, eInitialState state )
			{
				float padding = style.mButtonPadding;
				xAABB2 aabb = new xAABB2( pos, pos + mLabel.GetAssembledSize() + 2.0f * new Vector2( padding, padding ) );
				PlacePanel( parent, style, aabb, state );
				PlaceButtonLabel( mLabel, style );
			}

			public void PlaceButton( Widget parent, Style style, ePlacement placement, eInitialState state )
			{
				PlacePanel( parent, style, placement, state );
				PlaceButtonLabel( mLabel, style );
			}

			private void PlaceButtonLabel( Label label, Style style )
			{
				label.PlaceLabel( this, style, ePlacement.Centered, eInitialState.Dormant );
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

		public Button CreateButton(	Style button_style, String text, Widget parent, Style placement_style, 
									Vector2 pos, Widget.eInitialState initial_state )
		{
			Button b = new Button();
			b.AssembleButton( button_style, text );
			b.PlaceButton( parent, placement_style, pos, initial_state );
			return b;
		}

		public Button CreateButton( Style button_style, String text, Widget parent, Style placement_style,
									ePlacement placement, Widget.eInitialState initial_state )
		{
			Button b = new Button();
			b.AssembleButton( button_style, text );
			b.PlaceButton( parent, placement_style, placement, initial_state );
			return b;
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
