﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace XNARTS
{
	public partial class XUI : XSingleton< XUI >
	{
		private static long sPrevID = 0;

		private XUI()
		{
			// private constructor as per singleton
			Constructor_Style();
			Constructor_Buttons();
			Constructor_Selector();
			Constructor_Input();
		}

		public void Init()
		{
			Init_Selector();
			Init_Input();
			Init_Render();

			TestBed.CreateInstance().Init();
		}

		public void Update( GameTime t )
		{
			Update_Input();
			TestBed.Instance().Update( t );
		}

		private long NextID()
		{
			++sPrevID;
			return sPrevID;
		}
	}
}
