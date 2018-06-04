using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using XNARTS;


namespace XNARTS
{
	public class XRenderManager : XSingleton< XRenderManager >
	{
		GraphicsDeviceManager	mGraphicsDeviceManager;
		GraphicsDevice			mGraphicsDevice;

		BasicEffect				mBasicEffect_World;
		BasicEffect				mBasicEffect_Screen;

		public xCoord			mScreenDim;
		private XICamera		mMainWorldCam;
		private XICamera		mScreenCam;


		// private constructor for XSingleton
		private XRenderManager()
		{}


		public void Initialize( GraphicsDevice graphics_device, GraphicsDeviceManager graphics_device_manager )
		{
			mGraphicsDeviceManager = graphics_device_manager;
			mGraphicsDevice = graphics_device;

			var current_display_mode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
			mScreenDim = new xCoord( current_display_mode.Width, current_display_mode.Height );

			mGraphicsDeviceManager.IsFullScreen = true;
			mGraphicsDeviceManager.PreferredBackBufferWidth = mScreenDim.x;
			mGraphicsDeviceManager.PreferredBackBufferHeight = mScreenDim.y;
			mGraphicsDeviceManager.ApplyChanges();

			XSimpleDraw.Initialize();
			XSimpleDraw.CreateInstance( xeSimpleDrawType.World_Transient ).Init( graphics_device, persistent: false );
			XSimpleDraw.CreateInstance( xeSimpleDrawType.World_Persistent ).Init( graphics_device, persistent: true );
			XSimpleDraw.CreateInstance( xeSimpleDrawType.Screen_Transient ).Init( graphics_device, persistent: false );
			XSimpleDraw.CreateInstance( xeSimpleDrawType.Screen_Persistent ).Init( graphics_device, persistent: true );

			mMainWorldCam = new WorldCam( mScreenDim );
			mScreenCam = new ScreenCam( mScreenDim );
		}


		public void LoadContent()
		{
			// world space rendering setup
			mBasicEffect_World = new BasicEffect( mGraphicsDevice );
			mBasicEffect_World.World = Matrix.Identity;
			mBasicEffect_World.View = mMainWorldCam.GetViewMatrix();
			mBasicEffect_World.Projection = mMainWorldCam.GetProjectionMatrix();

			// screen space rendering setup
			mBasicEffect_Screen = new BasicEffect( mGraphicsDevice );
			mBasicEffect_Screen.World = Matrix.Identity;
			mBasicEffect_Screen.View = mScreenCam.GetViewMatrix();
			mBasicEffect_Screen.Projection = mScreenCam.GetProjectionMatrix();
		}


		public void Draw( GameTime game_time )
		{
			mGraphicsDevice.Clear( Color.CornflowerBlue );

			RasterizerState rasterizerState = new RasterizerState();
			rasterizerState.CullMode = CullMode.None;
			mGraphicsDevice.RasterizerState = rasterizerState;

			// simple draw only clients
			XWorld.Instance().RenderWorld( game_time );
			XMouse mouse = XMouse.Instance();
			mouse.RenderWorld( game_time );
			mouse.RenderScreen( game_time );

			mBasicEffect_World.VertexColorEnabled = true;

			XSimpleDraw simple_draw_world_transient = XSimpleDraw.Instance( xeSimpleDrawType.World_Transient );
			XSimpleDraw simple_draw_world_persistent = XSimpleDraw.Instance( xeSimpleDrawType.World_Persistent );
			XSimpleDraw simple_draw_screen_transient = XSimpleDraw.Instance( xeSimpleDrawType.Screen_Transient );
			XSimpleDraw simple_draw_screen_persistent = XSimpleDraw.Instance( xeSimpleDrawType.Screen_Persistent );

			foreach( EffectPass pass in mBasicEffect_World.CurrentTechnique.Passes )
			{
				pass.Apply();

				// actually render simple draw stuff.  possible layers needed.
				simple_draw_world_persistent.DrawAllPrimitives();
				simple_draw_world_transient.DrawAllPrimitives();

				// render clients who do their own rendering.  they should probably have pre-renders like simple draw, especially if there is more than one pass.
			}

			// simple draw screen
			mBasicEffect_Screen.VertexColorEnabled = true;

			//foreach ( EffectPass pass in effectPassCollection )
			foreach( EffectPass pass in mBasicEffect_Screen.CurrentTechnique.Passes )
			{
				pass.Apply();

				// actually render simple draw stuff.  possible layers needed.
				simple_draw_screen_persistent.DrawAllPrimitives();
				simple_draw_screen_transient.DrawAllPrimitives();

				// render clients who do their own rendering.  they should probably have pre-renders like simple draw, especially if there is more than one pass.
			}
		}
	}
}
