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
		public Button CreateButton( Style button_style, String text, Widget parent, Style placement_style, UIPosSpec spec )
		{
			Button b = new Button();
			b.AssembleButton( button_style, text );
			spec.Complete( b.GetAssembledSize() );
			b.PlaceButton( parent, placement_style, spec );
			AddActiveButton( b );
			AddRootWidget( b );
			return b;
		}

		public void DestroyButton( Button b )
		{
			RemoveActiveButton( b );
			RemoveRootWidget( b );
		}

		public BasicMenu CreateBasicMenu( eStyle menu_style, String[] texts, Widget parent, eStyle placement_style, ePlacement placement )
		{
			XUI ui = XUI.Instance();
			Style menu_style_actual = ui.GetStyle( menu_style );
			Style placement_style_actual = ui.GetStyle( placement_style );
			BasicMenu m = new BasicMenu();

			m.AssembleMenu( menu_style_actual, texts );
			m.PlaceMenu( parent, placement_style_actual, new UIPosSpec( placement, m.GetAssembledSize() ) );

			ui.AddRootWidget( m );
			return m;
		}

		public void DestroyBasicMenu( BasicMenu m )
		{
			m.Destroy();
			RemoveRootWidget( m );
		}

		public void DestroyFullMenu( FullMenu m )
		{
			m.Destroy();
			RemoveRootWidget( m );
		}
	}
}
