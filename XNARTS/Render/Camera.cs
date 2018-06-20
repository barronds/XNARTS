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
			NewWay( screen_dim );
		}


		private void NewWay( xCoord screen_dim )
		{
			mAspect = ((float)(screen_dim.y)) / screen_dim.x;
			xCoord world_size = XWorld.Instance().GetMapSize();

			// new way should work
			//Vector3 pos = new Vector3( world_size.x * 0.5f, world_size.y * 0.5f, 1f );
			//Vector3 target = pos - 2f * Vector3.UnitZ;

			Vector3 pos = new Vector3( 0, 0, 1f );
			Vector3 target = pos - 2f * Vector3.UnitZ;

			mView = Matrix.CreateLookAt( pos, target, Vector3.UnitY );

			// left, right, bottom, top, near, far
			float left = 0;
			float right = world_size.x;
			float top = 0;
			float bottom = mAspect * world_size.x;
			float near = 1;
			float far = -1;
			float x_offset = 1;
			float y_offset = -1;
			mProjection = Matrix.CreateOrthographicOffCenter( left + x_offset, right + x_offset, bottom + y_offset, top + y_offset, near, far );
		}


		private void OldWay( xCoord screen_dim )
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
