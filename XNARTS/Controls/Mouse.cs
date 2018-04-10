using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using XNARTS;


namespace XNARTS
{
    public class XMouse : XSingleton< XMouse >
    {
        xCoord mScreenDim;


		private XMouse()
		{
		}

		
		public void Init()
		{
			XRenderManager render_manager = XRenderManager.Instance();
			mScreenDim = render_manager.mScreenDim;
		}


		public void Update( GameTime game_time )
        {
            MouseState state = Mouse.GetState();

            ButtonState button_state = state.LeftButton;
            string button_value = button_state.ToString();

            Point pos = state.Position;
            string pos_value = pos.ToString();

            //Console.WriteLine( pos_value + " " + button_value);
        }

		public void RenderWorld( GameTime game_time )
		{
			//Vector3 start = new Vector3( -1f, -1f, -1f ) * 2f;
			//Vector3 end = -start;
			//Color color = Color.White;
			//XSimpleDraw.Instance( xeSimpleDrawType.World_Transient ).DrawLine( start, end, color, color );
		}

		public void RenderScreen( GameTime game_time )
		{
			Vector3 start = new Vector3( 10f, 10f, 0f );
			Vector3 end = new Vector3( 1910f, 1070f, 0f );
			XSimpleDraw screen = XSimpleDraw.Instance( xeSimpleDrawType.Screen_Transient );
			screen.DrawLine( start, end, Color.Black, Color.White );
		}
	}
}
