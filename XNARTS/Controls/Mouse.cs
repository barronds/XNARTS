using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using XNARTS;
using XNARTS.Render;


namespace XNARTS
{
    public class XMouse : Singleton< XMouse >
    {
        tCoord mScreenDim;
		SimpleDraw mSimpleDrawWorld;
		SimpleDraw mSimpleDrawScreen;


		private XMouse()
		{
		}

		
		public void Init()
		{
			RenderManager render_manager = RenderManager.Instance();
			mScreenDim = render_manager.mScreenDim;
			mSimpleDrawWorld = render_manager.mSimpleDraw_World;
			mSimpleDrawScreen = render_manager.mSimpleDraw_Screen;
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
			Vector3 start = new Vector3( -1f, -1f, -1f ) * 2f;
			Vector3 end = -start;
			Color color = Color.White;
			mSimpleDrawWorld.DrawLine( start, end, color, color );
		}

		public void RenderScreen( GameTime game_time )
		{
			Vector3 start = new Vector3( 0f, 0f, 0f );
			Vector3 end = new Vector3( 1920f, 1080f, 0f );
			mSimpleDrawScreen.DrawLine( start, end, Color.Black, Color.White );

			mSimpleDrawScreen.DrawLine( new Vector3( 10f, 10f, 0f ), new Vector3( 100f, 10f, 0f ), Color.Black , Color.White );
		}
	}
}
