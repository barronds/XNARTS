using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using XNARTS.RTSMath;
using XNARTS.Controls;


namespace XNARTS.Render
{
	public class RenderManager
	{
		GraphicsDeviceManager	mGraphics;
		Game1					mOwner;
		BasicEffect				mBasicEffect_World;
		BasicEffect				mBasicEffect_Screen;

		public SimpleDraw		mSimpleDraw_World;
		public SimpleDraw		mSimpleDraw_Screen;
		public tCoord			mScreenDim;


		public RenderManager( Game1 owner )
		{
			mOwner = owner;
			mGraphics = new GraphicsDeviceManager( owner );
			owner.Content.RootDirectory = "Content";
		}


		public void Initialize()
		{
			var current_display_mode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
			mScreenDim = new tCoord( current_display_mode.Width, current_display_mode.Height );

			mGraphics.IsFullScreen = false;
			mGraphics.PreferredBackBufferWidth = mScreenDim.x;
			mGraphics.PreferredBackBufferHeight = mScreenDim.y;
			mGraphics.ApplyChanges();

			mSimpleDraw_World = new SimpleDraw( mOwner.GraphicsDevice );
			mSimpleDraw_Screen = new SimpleDraw( mOwner.GraphicsDevice );
		}


		public void LoadContent()
		{
			// world space rendering setup
			mBasicEffect_World = new BasicEffect( mOwner.GraphicsDevice );
			mBasicEffect_World.World = Matrix.Identity;
			mBasicEffect_World.View = Matrix.CreateLookAt( new Vector3( 6, 4, 1f ), new Vector3( 6, 4, 0f ), new Vector3( 0f, 1f, 0f ) );
			float aspect = (float)(mGraphics.PreferredBackBufferHeight) / mGraphics.PreferredBackBufferWidth;
			float viewport_scale = 10f;
			mBasicEffect_World.Projection = Matrix.CreateOrthographicOffCenter( -viewport_scale, viewport_scale, -viewport_scale * aspect, viewport_scale * aspect, 0f, 2f );

			// screen space rendering setup
			mBasicEffect_Screen = new BasicEffect( mOwner.GraphicsDevice );
			mBasicEffect_Screen.World = Matrix.Identity;
			mBasicEffect_Screen.View = Matrix.CreateLookAt( new Vector3( 0f, 0f, 1f ), new Vector3( 0f, 0f, 0f ), new Vector3( 0f, 1f, 0f ) );
			mBasicEffect_Screen.Projection = Matrix.CreateOrthographicOffCenter( 0, mGraphics.PreferredBackBufferWidth, mGraphics.PreferredBackBufferHeight, 0, 0f, 2f );
		}


		public void Draw( GameTime game_time )
		{
			mOwner.GraphicsDevice.Clear( Color.CornflowerBlue );

			RasterizerState rasterizerState = new RasterizerState();
			rasterizerState.CullMode = CullMode.None;
			mOwner.GraphicsDevice.RasterizerState = rasterizerState;

			// simple draw only clients
			XNARTSMouse mouse = XNARTSMouse.Instance();
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
