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
		public class BasicMenu : VerticalStack
		{
			Style mButtonStyle;
			Dictionary< long, int > mUIDMap;

			public BasicMenu()
			{
				mUIDMap = new Dictionary<long, int>();
			}

			// use min_string_length > 0 if you want buttons to have a min width.
			// useful for creating menus within a vertical stack having buttons
			// of the same width.  use GetLongestString().
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

				// but all buttons need to be as big as the biggest
				float max_width = 0.0f;

				for( int i = 0; i < buttons.Count(); ++i )
				{
					Vector2 size = buttons[ i ].GetAssembledSize();
					max_width = Math.Max( max_width, size.X );
				}

				for( int i = 0; i < buttons.Count(); ++i )
				{
					Vector2 new_size = new Vector2( max_width, buttons[ i ].GetAssembledSize().Y );
					buttons[ i ].ReassembleWidget( new_size );
				}

				AssembleVerticalStack( buttons, style );
			}

			public void ReassembleMenu( float button_width )
			{
				//Vector2 menu_size = GetAssembledSize();
				//menu_size.X = Math.Max( menu_size.X, button_width );
				//ReassembleWidget( menu_size );

				for( int i = 0; i < GetNumChildren(); ++i )
				{
					Vector2 size = GetChild( i ).GetAssembledSize();
					size.X = button_width;
					GetChild( i ).ReassembleWidget( size );
				}

				ReassembleVerticalStack();
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


		public class FullMenu : VerticalStack
		{
			Style mTitleStyle;
			Style mOptionsStyle;
			Style mControlsStyle;

			private enum eChild
			{
				Title,
				Options,
				Controls
			}

			public FullMenu()
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

				BasicMenu options_menu = new BasicMenu();
				options_menu.AssembleMenu( options_style, options );

				BasicMenu controls_menu = new BasicMenu();
				controls_menu.AssembleMenu( controls_style, controls );

				// make each menu the width of the max of each
				float[] width_arr = {	title_label.GetAssembledSize().X, 
										options_menu.GetButtonWidth(), 
										controls_menu.GetButtonWidth() };

				float max_width = XMath.MaxArr( width_arr );
				options_menu.ReassembleMenu( max_width );
				controls_menu.ReassembleMenu( max_width );

				// order matters here, must correspond to eChild layout
				Widget[] widgets = { title_label, options_menu, controls_menu };
				AssembleVerticalStack( widgets, style );
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
		}

	}
}
