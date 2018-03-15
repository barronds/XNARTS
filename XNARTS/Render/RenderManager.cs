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
	public class XRenderManager : Singleton< XRenderManager >
	{
		GraphicsDeviceManager	mGraphicsDeviceManager;
		GraphicsDevice			mGraphicsDevice;

		BasicEffect				mBasicEffect_World;
		BasicEffect				mBasicEffect_Screen;

		public SimpleDraw		mSimpleDraw_World;
		public SimpleDraw		mSimpleDraw_Screen;
		public tCoord			mScreenDim;
		private ICamera			mMainWorldCam;
		private ICamera			mScreenCam;


		private XRenderManager()
		{
		}


		public void Initialize( GraphicsDevice graphics_device, GraphicsDeviceManager graphics_device_manager )
		{
			mGraphicsDeviceManager = graphics_device_manager;
			mGraphicsDevice = graphics_device;

			var current_display_mode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
			mScreenDim = new tCoord( current_display_mode.Width, current_display_mode.Height );

			mGraphicsDeviceManager.IsFullScreen = false;
			mGraphicsDeviceManager.PreferredBackBufferWidth = mScreenDim.x;
			mGraphicsDeviceManager.PreferredBackBufferHeight = mScreenDim.y;
			mGraphicsDeviceManager.ApplyChanges();

			mSimpleDraw_World = new SimpleDraw( mGraphicsDevice );
			mSimpleDraw_Screen = new SimpleDraw( mGraphicsDevice );

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
			XMouse mouse = XMouse.Instance();
			mouse.RenderWorld( game_time );
			mouse.RenderScreen( game_time );

			mBasicEffect_World.VertexColorEnabled = true;

			foreach( EffectPass pass in mBasicEffect_World.CurrentTechnique.Passes )
			{
				pass.Apply();

				// actually render simple draw stuff.  possible layers needed.
				mSimpleDraw_World.DrawAllPrimitives();

				// render clients who do their own rendering.  they should probably have pre-renders like simple draw, especially if there is more than one pass.
			}

			// simple draw screen
			mBasicEffect_Screen.VertexColorEnabled = true;

			//foreach ( EffectPass pass in effectPassCollection )
			foreach( EffectPass pass in mBasicEffect_Screen.CurrentTechnique.Passes )
			{
				pass.Apply();

				// actually render simple draw stuff.  possible layers needed.
				mSimpleDraw_Screen.DrawAllPrimitives();

				// render clients who do their own rendering.  they should probably have pre-renders like simple draw, especially if there is more than one pass.
			}
		}
	}
}
