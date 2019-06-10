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
		ContentManager          mContentManager;

		BasicEffect				mBasicEffect_World;
		BasicEffect				mBasicEffect_Screen;

		SpriteBatch				mSpriteBatch;

		public xCoord			mScreenDim;
		private XICamera		mMainWorldCam;
		private XICamera		mScreenCam;


		// private constructor for XSingleton
		private XRenderManager()
		{}


		public void Initialize( GraphicsDevice graphics_device, 
								GraphicsDeviceManager graphics_device_manager, 
								ContentManager content_manager )
		{
			mGraphicsDeviceManager = graphics_device_manager;
			mGraphicsDevice = graphics_device;
			mContentManager = content_manager;

			var current_display_mode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
			mScreenDim = new xCoord( current_display_mode.Width, current_display_mode.Height );

			mGraphicsDeviceManager.IsFullScreen = false; // true;
			mGraphicsDeviceManager.PreferredBackBufferWidth = mScreenDim.x;
			mGraphicsDeviceManager.PreferredBackBufferHeight = mScreenDim.y;
			mGraphicsDeviceManager.ApplyChanges();

			XSimpleDraw.Initialize();
			XSimpleDraw.CreateInstance( xeSimpleDrawType.WorldSpace_Transient ).Init(	graphics_device, 
																						persistent: false,
																						max_lines: 2000,
																						max_triangles: 50 );

			XSimpleDraw.CreateInstance( xeSimpleDrawType.WorldSpace_Persistent ).Init(	graphics_device, 
																						persistent: true,
																						max_lines: 50,
																						max_triangles: 50 );

			XSimpleDraw.CreateInstance( xeSimpleDrawType.ScreenSpace_Transient ).Init(	graphics_device, 
																						persistent: false,
																						max_lines: 200,
																						max_triangles: 50 );

			XSimpleDraw.CreateInstance( xeSimpleDrawType.ScreenSpace_Persistent ).Init( graphics_device, 
																						persistent: true,
																						max_lines: 50,
																						max_triangles: 50 );

			XSimpleDraw.CreateInstance( xeSimpleDrawType.WorldSpace_Persistent_Map ).Init(	graphics_device,
																							persistent: true,
																							max_lines: 50,
																							max_triangles: 1600000 );
			mMainWorldCam = new XWorldCam( mScreenDim );
			mScreenCam = new XScreenCam( mScreenDim );
		}


		public void LoadContent()
		{
			// world space rendering setup
			mBasicEffect_World = new BasicEffect( mGraphicsDevice );
			mBasicEffect_World.World = Matrix.Identity;

			// screen space rendering setup
			mBasicEffect_Screen = new BasicEffect( mGraphicsDevice );
			mBasicEffect_Screen.World = Matrix.Identity;

			// sprite batch, not used yet
			mSpriteBatch = new SpriteBatch( mGraphicsDevice );

			XFontDraw.Instance().LoadContent();

			UpdateCameras();
		}


		private void UpdateCameras()
		{
			mBasicEffect_World.View = mMainWorldCam.GetViewMatrix();
			mBasicEffect_World.Projection = mMainWorldCam.GetProjectionMatrix();
			mBasicEffect_Screen.View = mScreenCam.GetViewMatrix();
			mBasicEffect_Screen.Projection = mScreenCam.GetProjectionMatrix();
		}


		public void Draw( GameTime game_time )
		{
			mGraphicsDevice.Clear( Color.CornflowerBlue );

			RasterizerState rasterizerState = new RasterizerState();
			rasterizerState.CullMode = CullMode.None;
			mGraphicsDevice.RasterizerState = rasterizerState;

			// maybe not the best place for this
			// also, screen cam not updating anywhere
			mMainWorldCam.Update( game_time );
			UpdateCameras();

			// simple draw only clients
			XWorld.Instance().RenderWorld( game_time, mMainWorldCam.GetViewAABB() );
			XMouse mouse = XMouse.Instance();
			mouse.RenderWorld( game_time );
			mouse.RenderScreen( game_time );
			XUI.Instance().Draw();

			mBasicEffect_World.VertexColorEnabled = true;

			XSimpleDraw simple_draw_world_transient = XSimpleDraw.Instance( xeSimpleDrawType.WorldSpace_Transient );
			XSimpleDraw simple_draw_world_persistent = XSimpleDraw.Instance( xeSimpleDrawType.WorldSpace_Persistent );
			XSimpleDraw simple_draw_screen_transient = XSimpleDraw.Instance( xeSimpleDrawType.ScreenSpace_Transient );
			XSimpleDraw simple_draw_screen_persistent = XSimpleDraw.Instance( xeSimpleDrawType.ScreenSpace_Persistent );
			XSimpleDraw simple_draw_world_map_persistent = XSimpleDraw.Instance( xeSimpleDrawType.WorldSpace_Persistent_Map );

			foreach ( EffectPass pass in mBasicEffect_World.CurrentTechnique.Passes )
			{
				pass.Apply();

				// actually render simple draw stuff.  possible layers needed.
				simple_draw_world_map_persistent.DrawAllPrimitives();
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

			//mSpriteBatch.Begin();
			// do sprite batch rendering here
			//mSpriteBatch.End();

			XFontDraw.Instance().Draw();
		}
	}
}
