using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace XNARTS
{
	public interface XICamera
	{
		Matrix	GetViewMatrix		();
		Matrix	GetProjectionMatrix	();
		void	Update				( GameTime dt );
	}


	public class WorldCam : XICamera
	{
		private float mAspect;
		private Matrix mView;
		private Matrix mProjection;


		public WorldCam( xCoord screen_dim )
		{
			mAspect = ((float)(screen_dim.y)) / screen_dim.x;
			//xCoord world_size = XWorld.Instance().GetMapSize();

			// temporary
			float viewport_scale = 10f;
			// old hack
            mView = Matrix.CreateLookAt( new Vector3( 6, 4, 1f ), new Vector3( 6, 4, 0f ), new Vector3( 0f, 1f, 0f ) );

			// new way should work
			//Vector3 pos = new Vector3( screen_dim.x * 0.5f, screen_dim.y * 0.5f, 1f );
			//Vector3 target = pos - 2f * Vector3.UnitZ;

			//mView = Matrix.CreateLookAt( pos, target, Vector3.UnitY );
			mProjection = Matrix.CreateOrthographicOffCenter( -viewport_scale, viewport_scale, -viewport_scale * mAspect, viewport_scale * mAspect, 0f, 2f );
		}


		Matrix XICamera.GetViewMatrix()
		{
			return mView;
		}


		Matrix XICamera.GetProjectionMatrix()
		{
			return mProjection;
		}


		void XICamera.Update( GameTime game_time )
		{
			// move with controls
		}
	}


	public class ScreenCam : XICamera
	{
		private Matrix mView;
		private Matrix mProjection;


		public ScreenCam( xCoord screen_dim )
		{
			mView = Matrix.CreateLookAt( new Vector3( 0f, 0f, 1f ), new Vector3( 0f, 0f, 0f ), new Vector3( 0f, 1f, 0f ) );
			mProjection = Matrix.CreateOrthographicOffCenter( 0, screen_dim.x, screen_dim.y, 0, -1f, 1f );
		}


		Matrix XICamera.GetViewMatrix()
		{
			return mView;
		}


		Matrix XICamera.GetProjectionMatrix()
		{
			return mProjection;
		}


		void XICamera.Update( GameTime dt ) {}
	}
}
