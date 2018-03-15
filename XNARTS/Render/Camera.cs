using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace XNARTS
{
	public interface ICamera
	{
		Matrix	GetViewMatrix		();
		Matrix	GetProjectionMatrix	();
		void	Update				( GameTime dt );
	}


	public class WorldCam : ICamera
	{
		private float mAspect;
		private Matrix mView;
		private Matrix mProjection;


		public WorldCam( tCoord screen_dim )
		{
			mAspect = ((float)(screen_dim.y)) / screen_dim.x;

			// temporary
			float viewport_scale = 10f;
			mView = Matrix.CreateLookAt( new Vector3( 6, 4, 1f ), new Vector3( 6, 4, 0f ), new Vector3( 0f, 1f, 0f ) );
			mProjection = Matrix.CreateOrthographicOffCenter( -viewport_scale, viewport_scale, -viewport_scale * mAspect, viewport_scale * mAspect, 0f, 2f );
		}


		Matrix ICamera.GetViewMatrix()
		{
			return mView;
		}


		Matrix ICamera.GetProjectionMatrix()
		{
			return mProjection;
		}


		void ICamera.Update( GameTime dt )
		{
			// move with controls
		}
	}


	public class ScreenCam : ICamera
	{
		private Matrix mView;
		private Matrix mProjection;


		public ScreenCam( tCoord screen_dim )
		{
			mView = Matrix.CreateLookAt( new Vector3( 0f, 0f, 1f ), new Vector3( 0f, 0f, 0f ), new Vector3( 0f, 1f, 0f ) );
			mProjection = Matrix.CreateOrthographicOffCenter( 0, screen_dim.x, screen_dim.y, 0, 0f, 2f );
		}


		Matrix ICamera.GetViewMatrix()
		{
			return mView;
		}


		Matrix ICamera.GetProjectionMatrix()
		{
			return mProjection;
		}


		void ICamera.Update( GameTime dt ) {}
	}
}
