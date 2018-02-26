using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using XNARTS;


namespace XNARTS
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
		GraphicsDeviceManager mGraphicsDeviceManager;


		public Game1()
        {
			mGraphicsDeviceManager = new GraphicsDeviceManager( this );
			Content.RootDirectory = "Content";
			XRenderManager.InstantiateSingleton();
		}
		

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
        {
			XRenderManager.Instance().Initialize( GraphicsDevice, mGraphicsDeviceManager );
			XMouse.InstantiateSingleton().Init();
			base.Initialize();
        }

		
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
			XRenderManager.Instance().LoadContent();
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
			// user controller app exit
			if ( GamePad.GetState( PlayerIndex.One ).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown( Keys.Escape ) )
            {
                Exit();
            }

			// TODO: Add your update logic here
			XMouse.Instance().Update( game_time );

            base.Update(game_time);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime game_time )
        {
			XRenderManager.Instance().Draw( game_time );
			base.Draw( game_time );
		}
    }
}