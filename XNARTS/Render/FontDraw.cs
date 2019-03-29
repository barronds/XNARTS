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
		Not_Available = -2,
		Invalid = -1,

		Consolas13,
		Consolas16,
		Consolas24,
		Consolas36,
		Consolas48,

		LucidaConsole8,
		LucidaConsole10,
		LucidaConsole12,
		LucidaConsole16,
		LucidaConsole24,
		LucidaConsole36,

		Num
	}

	class XFontDraw : XSingleton< XFontDraw >
	{
		private GraphicsDevice					mGraphicsDevice;
		private ContentManager					mContentManager;
		private SpriteBatch						mSpriteBatch;
		private List< DrawRequest >				mRequests;
		private Dictionary< int, SpriteFont >	mFontSpriteMap;
		private Dictionary< eFont, FontInfo >	mFontInfos;

		public class FontInfo
		{
			public FontInfo( Vector2 size, Vector2 offset )
			{
				mSize = size;
				mOffset = offset;
			}

			public Vector2 mSize;
			public Vector2 mOffset;
		}
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
		public FontInfo GetFontInfo( eFont font )
		{
			return mFontInfos[ font ];
		}

		private XFontDraw()
		{
			mRequests = new List<DrawRequest>();
			mFontSpriteMap = new Dictionary<int, SpriteFont>();

			// manually stock the font sizes dictionary with emperically determined sizes
			mFontInfos = new Dictionary<eFont, FontInfo>();
			mFontInfos.Add( eFont.Consolas16, new FontInfo( new Vector2( 12.0f, 19.0f ), Vector2.Zero ) );
			mFontInfos.Add( eFont.Consolas24, new FontInfo( new Vector2( 18, 29 ), Vector2.Zero ) );
			mFontInfos.Add( eFont.Consolas36, new FontInfo( new Vector2( 26, 45 ), Vector2.Zero ) );
			mFontInfos.Add( eFont.Consolas48, new FontInfo( new Vector2( 35, 60 ), Vector2.Zero ) );
			mFontInfos.Add( eFont.LucidaConsole16, new FontInfo( new Vector2( 13, 19 ), Vector2.Zero ) );
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
				FontInfo info = mFontInfos[ mRequests[ i ].mFont ];
				String s = mRequests[ i ].mString;
				Vector2 pos = mRequests[ i ].mScreenPos + info.mOffset;
				Color color = mRequests[ i ].mColor;

				mSpriteBatch.DrawString( sprite_font, s, pos, color );
			}

			mSpriteBatch.End();
			mRequests.Clear();
		}
	}
}
