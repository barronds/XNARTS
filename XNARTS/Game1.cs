using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using XNARTS.Controls;
using XNARTS.RTSMath;
using XNARTS.Render;

namespace XNARTS
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager mGraphics;

        BasicEffect basicEffect;
		BasicEffect basicEffect2;
		BasicEffect mBasicEffect_World;
		BasicEffect mBasicEffect_Screen;
		SimpleDraw  mSimpleDraw_World;
        SimpleDraw  mSimpleDraw_Screen;

        Matrix world = Matrix.CreateTranslation(0, 0, 0);
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 3), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.01f, 100f);
        double angle = 0;
		float mCamDrift, mCamStartX, mCamStartY;

		XNARTSMouse mMouse;

        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

			mCamDrift = 0.1f;
			mCamStartX = 6.0f;
			mCamStartY = 4.0f;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
        {
            base.Initialize();

            var current_display_mode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            tCoord native_dim = new tCoord( current_display_mode.Width, current_display_mode.Height );

            mGraphics.IsFullScreen = false;
            mGraphics.PreferredBackBufferWidth = native_dim.x;
            mGraphics.PreferredBackBufferHeight = native_dim.y;
            mGraphics.ApplyChanges();

			mSimpleDraw_World = new SimpleDraw( GraphicsDevice );
			mSimpleDraw_Screen = new SimpleDraw( GraphicsDevice );

			mMouse = new XNARTSMouse( native_dim, mSimpleDraw_World, mSimpleDraw_Screen );
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
			// world space rendering setup
			mBasicEffect_World = new BasicEffect( GraphicsDevice );
			mBasicEffect_World.World = Matrix.Identity;
			mBasicEffect_World.View = Matrix.CreateLookAt( new Vector3( mCamStartX, mCamStartY, 1f ), new Vector3( mCamStartX, mCamStartY, 0f ), new Vector3( 0f, 1f, 0f ) );
			float aspect = (float)(mGraphics.PreferredBackBufferHeight) / mGraphics.PreferredBackBufferWidth;
			float viewport_scale = 10f;
			mBasicEffect_World.Projection = Matrix.CreateOrthographicOffCenter( -viewport_scale, viewport_scale, -viewport_scale * aspect, viewport_scale * aspect, 0f, 2f );

			// screen space rendering setup
			mBasicEffect_Screen = new BasicEffect( GraphicsDevice );
			mBasicEffect_Screen.World = Matrix.Identity;
			mBasicEffect_Screen.View = Matrix.CreateLookAt( new Vector3( 0f, 0f, 1f ), new Vector3( 0f, 0f, 0f ), new Vector3( 0f, 1f, 0f ) );
			mBasicEffect_Screen.Projection = Matrix.CreateOrthographicOffCenter( 0, mGraphics.PreferredBackBufferWidth, mGraphics.PreferredBackBufferHeight, 0, 0f, 2f );

			basicEffect = new BasicEffect(GraphicsDevice);
			basicEffect2 = new BasicEffect( GraphicsDevice );
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="game_time">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime game_time)
        {
			float dx = mCamDrift * (game_time.TotalGameTime.Seconds + (game_time.TotalGameTime.Milliseconds) / 1000f);
			mBasicEffect_World.View = Matrix.CreateLookAt( new Vector3( mCamStartX + dx, mCamStartY, 1f ), new Vector3( mCamStartX + dx, mCamStartY, 0f ), new Vector3( 0f, 1f, 0f ) );

			// user controller app exit
			if ( GamePad.GetState( PlayerIndex.One ).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown( Keys.Escape ) )
            {
                Exit();
            }

            // TODO: Add your update logic here
            mMouse.Update( game_time );

            angle += 0.01f;
            view = Matrix.CreateLookAt(
                new Vector3(5 * (float)Math.Sin(angle), -2, 5 * (float)Math.Cos(angle)),
                new Vector3(0, 0, 0),
                Vector3.UnitY);

            base.Update(game_time);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime game_time )
        {
            // A temporary array, with 12 items in it, because
            // the icosahedron has 12 distinct vertices
            VertexPositionColor[] vertices = new VertexPositionColor[2];
			VertexPositionColor[] vertices2 = new VertexPositionColor[2];

			// vertex position and color information for icosahedron
			vertices[ 0] = new VertexPositionColor(new Vector3(-0.26286500f, 0.0000000f, 0.42532500f), Color.Red);
            vertices[1] = new VertexPositionColor(new Vector3(0.26286500f, 0.0000000f, 0.42532500f), Color.Orange);

			vertices2[ 0 ] = new VertexPositionColor( new Vector3( -0.3f, 0.0000000f, 0.4f ), Color.Blue );
			vertices2[ 1 ] = new VertexPositionColor( new Vector3( 0.23f, 0.0000000f, 0.41f ), Color.Green );

			var vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 2, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);

			var vertexBuffer2 = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 2, BufferUsage.WriteOnly);
			vertexBuffer2.SetData<VertexPositionColor>( vertices2 );

			short[] indices = new short[4];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 3;

            var indexBuffer = new IndexBuffer(mGraphics.GraphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);


            GraphicsDevice.Clear(Color.CornflowerBlue);

            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.Projection = projection;
            basicEffect.VertexColorEnabled = true;

			basicEffect2.World = world;
			basicEffect2.View = view;
			basicEffect2.Projection = projection;
			basicEffect2.VertexColorEnabled = true;

			GraphicsDevice.SetVertexBuffer(vertexBuffer);
            GraphicsDevice.Indices = indexBuffer;

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                // GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 20);
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, 20);
            }

			GraphicsDevice.SetVertexBuffer( vertexBuffer2 );

			foreach( EffectPass pass in basicEffect2.CurrentTechnique.Passes )
			{
				pass.Apply();
				// GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 20);
				GraphicsDevice.DrawIndexedPrimitives( PrimitiveType.LineList, 0, 0, 20 );
			}

			// simple draw only clients
			mMouse.RenderWorld( game_time );
			mMouse.RenderScreen( game_time );

			mBasicEffect_World.VertexColorEnabled = true;
			EffectTechnique effectTechnique = mBasicEffect_World.Techniques[0];
			EffectPassCollection effectPassCollection = effectTechnique.Passes;


			foreach ( EffectPass pass in effectPassCollection )
			{
				// hopefully only one pass
				pass.Apply();

				// actually render simple draw stuff.  possible layers needed.
				mSimpleDraw_World.DrawAllPrimitives();

				// render clients who do their own rendering.  they should probably have pre-renders like simple draw, especially if there is more than one pass.
			}


			// simple draw screen
			mBasicEffect_Screen.VertexColorEnabled = true;
			effectTechnique = mBasicEffect_Screen.Techniques[ 0 ];
			effectPassCollection = effectTechnique.Passes;

			foreach ( EffectPass pass in effectPassCollection )
			{
				// hopefully only one pass
				pass.Apply();

				// actually render simple draw stuff.  possible layers needed.
				mSimpleDraw_Screen.DrawAllPrimitives();

				// render clients who do their own rendering.  they should probably have pre-renders like simple draw, especially if there is more than one pass.
			}

			base.Draw( game_time );
        }
    }
}