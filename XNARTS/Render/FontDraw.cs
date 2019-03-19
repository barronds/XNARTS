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
	public enum eFont
	{
		Invalid = -1,

		Consolas16,

		Num
	}

	class XFontDraw : XSingleton< XFontDraw >
	{
		GraphicsDevice					mGraphicsDevice;
		ContentManager					mContentManager;
		SpriteBatch						mSpriteBatch;
		List< DrawRequest >				mRequests;
		Dictionary< int, SpriteFont >	mFontSpriteMap;

		private class DrawRequest
		{
			public DrawRequest( eFont font, Vector2 screen_pos, Color color, String s )
			{
				mFont = font;
				mScreenPos = screen_pos;
				mColor = color;
				mString = s;
			}

			public eFont	mFont;
			public Vector2	mScreenPos;
			public Color	mColor;
			public String   mString;
		}

		public void DrawString( eFont font, Vector2 screen_pos, Color color, String s )
		{
			mRequests.Add( new DrawRequest( font, screen_pos, color, s ) );
		}

		private XFontDraw()
		{
			mRequests = new List<DrawRequest>();
			mFontSpriteMap = new Dictionary<int, SpriteFont>();
		}

		public void Init( GraphicsDevice device, ContentManager content_manager )
		{
			mGraphicsDevice = device;
			mContentManager = content_manager;
		}

		public void LoadContent()
		{
			mSpriteBatch = new SpriteBatch( mGraphicsDevice );

			for( int i = 0; i < (int)eFont.Num; ++i )
			{
				mFontSpriteMap.Add( i, mContentManager.Load<SpriteFont>( ((eFont)i).ToString() ) );
			}
		}

		public void Draw()
		{
			mSpriteBatch.Begin();

			for( int i = 0; i < mRequests.Count; ++i )
			{
				SpriteFont sprite_font = mFontSpriteMap[ (int)(mRequests[ i ].mFont) ];
				String s = mRequests[ i ].mString;
				Vector2 pos = mRequests[ i ].mScreenPos;
				Color color = mRequests[ i ].mColor;

				mSpriteBatch.DrawString( sprite_font, s, pos, color );
			}

			mSpriteBatch.End();
			mRequests.Clear();
		}
	}
}
