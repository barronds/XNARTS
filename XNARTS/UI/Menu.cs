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
		public class BasicMenu : LinearStack
		{
			Style mButtonStyle;
			Dictionary< long, int > mUIDMap;

			public BasicMenu( eDirection direction ) : base( direction )
			{
				mUIDMap = new Dictionary<long, int>();
			}

			public void AssembleMenu( Style style, String[] texts )
			{
				XUtils.Assert( texts.Count() > 0 );
				mButtonStyle = style;
				Button[] buttons = new Button[ texts.Count() ];

				for( int i = 0; i < texts.Count(); ++i )
				{
					XUtils.Assert( texts.Length > 0 );
					buttons[ i ] = new Button();
					buttons[ i ].AssembleButton( style, texts[ i ] );
					mUIDMap.Add( buttons[ i ].GetUID(), i );
				}

				// but all buttons need to be as big (perp) as the biggest
				float max_perp = 0.0f;
				Vector2 perp = GetPerp();
				Vector2 dir = GetDir();

				for( int i = 0; i < buttons.Count(); ++i )
				{
					Vector2 size = buttons[ i ].GetAssembledSize();
					max_perp = Math.Max( max_perp, Vector2.Dot( size, perp ) );
				}

				for( int i = 0; i < buttons.Count(); ++i )
				{
					Vector2 new_size = Vector2.Dot( buttons[ i ].GetAssembledSize(), dir ) * dir + max_perp * perp;
					buttons[ i ].ReassembleWidget( new_size );
				}

				AssembleLinearStack( buttons, style );
			}

			public void ReassembleMenu( float button_perp )
			{
				for( int i = 0; i < GetNumChildren(); ++i )
				{
					Vector2 dir = GetDir();
					Vector2 perp = GetPerp();
					Vector2 size = GetChild( i ).GetAssembledSize();
					Vector2 new_size = Vector2.Dot( size, dir ) * dir + button_perp * perp;
					GetChild( i ).ReassembleWidget( new_size );
				}

				ReassembleLinearStack();
			}

			public void PlaceMenu( Widget parent, Style style, UIPosSpec spec )
			{
				PlacePanel( parent, style, spec );
				PlaceButtons();
			}

			public int GetInputIndex( long uid )
			{
				if( mUIDMap.ContainsKey( uid ) )
				{
					mUIDMap.TryGetValue( uid, out int text_index );
					return text_index;
				}

				return -1;
			}

			public void Destroy()
			{
				XUI ui = XUI.Instance();

				for ( int i = 0; i < GetNumChildren(); ++i )
				{
					Button b = (Button)GetChild( i );
					ui.RemoveActiveButton( b );
				}
			}

			public float GetButtonWidth()
			{
				return GetChild( 0 ).GetAssembledSize().X;
			}

			private void PlaceButtons()
			{
				XUI ui = XUI.Instance();

				for( int i = 0; i < GetNumChildren(); ++i )
				{
					Button b = (Button)GetChild( i );
					b.PlaceButton( this, mButtonStyle, new UIPosSpec( GetRelativePlacement( i ) ) );
					ui.AddActiveButton( b );
				}
			}
		}


		public class FullMenu : LinearStack
		{
			Style mTitleStyle;
			Style mOptionsStyle;
			Style mControlsStyle;

			private enum eChild
			{
				Spacer0,
				Title,
				Spacer1,
				Options,
				Controls,
				Spacer2,
			}

			public FullMenu() : base( eDirection.Vertical )
			{ }

			public void AssembleFullMenu(	Style style, String title, Style title_style, 
											String[] options, Style options_style, 
											String[] controls, Style controls_style )
			{
				XUtils.Assert( options.Count() > 0 && controls.Count() > 0 );

				mTitleStyle = title_style;
				mOptionsStyle = options_style;
				mControlsStyle = controls_style;

				Label title_label = new Label();
				title_label.AssembleLabel( title_style, title );

				Spacer spacer0 = new Spacer();
				Spacer spacer1 = new Spacer();
				Spacer spacer2 = new Spacer();
				const float kSpacerScalar = 0.4f;
				Vector2 spacer_size = kSpacerScalar * title_label.GetAssembledSize();
				spacer0.AssembleSpacer( spacer_size );
				spacer1.AssembleSpacer( spacer_size );
				spacer2.AssembleSpacer( spacer_size );

				BasicMenu options_menu = new BasicMenu( eDirection.Vertical );
				options_menu.AssembleMenu( options_style, options );

				BasicMenu controls_menu = new BasicMenu( eDirection.Vertical );
				controls_menu.AssembleMenu( controls_style, controls );

				// make each menu the width of the max of each
				float[] width_arr = {	title_label.GetAssembledSize().X, 
										options_menu.GetButtonWidth(), 
										controls_menu.GetButtonWidth() };

				float max_width = XMath.MaxArr( width_arr );
				options_menu.ReassembleMenu( max_width );
				controls_menu.ReassembleMenu( max_width );

				// order matters here, must correspond to eChild layout
				Widget[] widgets = { spacer0, title_label, spacer1, options_menu, controls_menu, spacer2 };
				AssembleLinearStack( widgets, style );
			}

			public void PlaceFullMenu( Widget parent, Style style, UIPosSpec spec )
			{
				PlacePanel( parent, style, spec );
				PlaceWidgets();
			}

			public int GetOptionsInputIndex( long uid )
			{
				return GetMenu( eChild.Options ).GetInputIndex( uid );
			}

			public int GetControlsInputIndex( long uid )
			{
				return GetMenu( eChild.Controls ).GetInputIndex( uid );
			}

			public void Destroy()
			{
				GetMenu( eChild.Options ).Destroy();
				GetMenu( eChild.Controls ).Destroy();
			}

			private void PlaceWidgets()
			{
				GetSpacer( eChild.Spacer0 ).PlaceSpacer( this, mTitleStyle, new UIPosSpec( GetRelativePlacement( (int)eChild.Spacer0 ) ) );
				GetSpacer( eChild.Spacer1 ).PlaceSpacer( this, mTitleStyle, new UIPosSpec( GetRelativePlacement( (int)eChild.Spacer1 ) ) );
				GetSpacer( eChild.Spacer2 ).PlaceSpacer( this, mTitleStyle, new UIPosSpec( GetRelativePlacement( (int)eChild.Spacer2 ) ) );

				GetTitle().PlaceLabel( this, mTitleStyle, new UIPosSpec( GetRelativePlacement( (int)eChild.Title ) ) );

				GetMenu( eChild.Options ).PlaceMenu( this, mOptionsStyle, new UIPosSpec( GetRelativePlacement( (int)eChild.Options ) ) );
				GetMenu( eChild.Controls ).PlaceMenu( this, mControlsStyle, new UIPosSpec( GetRelativePlacement( (int)eChild.Controls ) ) );
			}

			private BasicMenu GetMenu( eChild child )
			{
				XUtils.Assert( child == eChild.Controls || child == eChild.Options );
				return (BasicMenu)GetChild( (int)child );
			}

			private Label GetTitle()
			{
				return (Label)GetChild( (int)eChild.Title );
			}

			private Spacer GetSpacer( eChild spacer )
			{
				return (Spacer)GetChild( (int)spacer );
			}
		}

	}
}
