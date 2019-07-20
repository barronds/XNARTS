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
		private GraphicsDeviceManager			mGraphicsDeviceManager;
		private XListener< ExitGameEvent >      mListener_ExitGameEvent;

		public Game1()
        {
			mGraphicsDeviceManager = new GraphicsDeviceManager( this );
			Content.RootDirectory = "Content";

			mListener_ExitGameEvent = new XListener<ExitGameEvent>( 1, eEventQueueFullBehaviour.Ignore, "ExitGame" );

			BulletinBoard.CreateInstance();
			XFontDraw.CreateInstance();
			XRenderManager.CreateInstance();
			XTouch.CreateInstance();
			XKeyInput.CreateInstance();
			XWorld.CreateInstance();
			XMouse.CreateInstance();
			XUI.CreateInstance();
			XDebugMenu.CreateInstance();
		}


		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
        {
			BulletinBoard.Instance().Init();
			XTouch.Instance().Init();
			XKeyInput.Instance().Init();
			XWorld.Instance().Init();
			XMouse.Instance().Init();
			XFontDraw.Instance().Init( GraphicsDevice, Content );
			XRenderManager.Instance().Initialize( GraphicsDevice, mGraphicsDeviceManager, Content );
			XUI.Instance().Init();
			XDebugMenu.Instance().Init();

			base.Initialize();

			BulletinBoard.Instance().mBroadcaster_ExitGameEvent.Subscribe( mListener_ExitGameEvent );
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
			// TODO: get this into exit event framework below.  
			if ( GamePad.GetState( PlayerIndex.One ).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown( Keys.Escape ) )
            {
                Exit();
            }

			ExitGameEvent e = mListener_ExitGameEvent.GetMaxOne();

			if( e != null )
			{
				// elaborate shutdown code goes here.  
				Exit();
			}

			// TODO: Add your update logic here
			XTouch.Instance().Update( game_time );
			XKeyInput.Instance().Update();
			XMouse.Instance().Update( game_time );
			XUI.Instance().Update( game_time );
			XDebugMenu.Instance().Update();
			XWorld.Instance().Update();

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