using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace XNARTS
{
	public partial class XUI : XSingleton< XUI >
	{
		private static long sPrevUID = 0;

		private XUI()
		{
			// private constructor as per singleton
			Constructor_Style();
			Constructor_Widget();
			Constructor_WidgetManager();
			_Constructor_Buttons();
			Constructor_Buttons();
			Constructor_Selector();
			_Constructor_Input();
			Constructor_Input();
			Constructor_TestBed();
		}

		public void Init()
		{
			Init_Widget();
			Init_Selector();
			_Init_Input();
			Init_Render();
			Init_TestBed();
		}

		public void Update( GameTime t )
		{
			_Update_Input();
			Update_TestBed( t );
		}

		public long NextUID()
		{
			++sPrevUID;
			return sPrevUID;
		}
	}
}
