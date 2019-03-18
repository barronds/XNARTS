using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XNARTS
{
	class XFontDraw : XSingleton< XFontDraw >
	{
		GraphicsDevice	mGraphicsDevice;
		ContentManager	mContentManager;
		SpriteFont		mSpriteFont_DebugText;
		SpriteBatch		mSpriteBatch;

		private XFontDraw()
		{
		}

		public void Init( GraphicsDevice device, ContentManager content_manager )
		{
			mGraphicsDevice = device;
			mContentManager = content_manager;
		}

		public void LoadContent()
		{
			mSpriteBatch = new SpriteBatch( mGraphicsDevice );
			mSpriteFont_DebugText = mContentManager.Load<SpriteFont>( "DebugText" );
		}

		public void Draw()
		{
			mSpriteBatch.Begin();
			mSpriteBatch.DrawString( mSpriteFont_DebugText, "Hello", new Vector2( 100, 100 ), Color.Black );
			mSpriteBatch.End();
		}
	}
}
