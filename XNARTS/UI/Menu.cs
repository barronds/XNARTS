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

			public void AssembleMenu( Style style, String[] texts )
			{
				XUtils.Assert( texts.Count() > 0 );
				mButtonStyle = style;
				String[] padded_texts = PadButtonTexts( texts );

				Button[] buttons = new Button[ padded_texts.Count() ];

				for( int i = 0; i < texts.Count(); ++i )
				{
					XUtils.Assert( padded_texts.Length > 0 );
					buttons[ i ] = new Button();
					buttons[ i ].AssembleButton( style, padded_texts[ i ] );
					mUIDMap.Add( buttons[ i ].GetUID(), i );
				}

				AssembleVerticalStack( buttons, style );
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

			private int GetLongestString( String[] strings )
			{
				int longest = 0;

				for ( int i = 0; i < strings.Length; ++i )
				{
					longest = Math.Max( longest, strings[ i ].Length );
				}

				return longest;
			}

			private String PadButtonText( String text, int longest )
			{
				int length = text.Length;
				int shortfall = longest - length;
				int even_floor_half_shortfall = shortfall / 2;
				String padding = XUtils.GetNSpaces( even_floor_half_shortfall );
				return padding + text + padding;
			}

			private String[] PadButtonTexts( String[] input )
			{
				int longest_text_length = GetLongestString( input );
				String[] output = new string[ input.Length ];

				for ( int i = 0; i < input.Length; ++i )
				{
					output[ i ] = PadButtonText( input[ i ], longest_text_length );
				}

				return output;
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
