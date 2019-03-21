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
		private XSimpleDraw mSimpleDraw;

		private void Init_Render()
		{
			mSimpleDraw = XSimpleDraw.GetInstance( xeSimpleDrawType.ScreenSpace_Transient );
		}

		public void Draw()
		{
			Draw_Buttons();
		}
	}
}
