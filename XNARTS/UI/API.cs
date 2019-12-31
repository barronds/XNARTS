﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace XNARTS
{
	public partial class XUI
	{
		public Button CreateButton( Style button_style, String text, Widget parent, Style placement_style, Vector2 pos )
		{
			Button b = new Button();
			b.AssembleButton( button_style, text );
			b.PlaceButton( parent, placement_style, pos );
			AddActiveButton( b );
			return b;
		}

		public Button CreateButton( Style button_style, String text, Widget parent, Style placement_style, ePlacement placement )
		{
			Button b = new Button();
			b.AssembleButton( button_style, text );
			b.PlaceButton( parent, placement_style, placement );
			AddActiveButton( b );
			return b;
		}

	}
}
