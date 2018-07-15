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


	public class XWorldCam : XICamera
	{
		private float mAspect;
		private xCoord mWorldSize;
		private xAABB2 mWorldView; 
		private Matrix mViewMatrix;
		private Matrix mProjectionMatrix;


		public XWorldCam( xCoord screen_dim )
		{
			mAspect = ((float)(screen_dim.y)) / screen_dim.x;
			mWorldSize = XWorld.Instance().GetMapSize();
			xAABB2 world_view = new xAABB2( new Vector2( 0, 0 ), new Vector2( mWorldSize.x, mWorldSize.y ) );
			mWorldView = ClampWorldView( world_view );
			NewWay( screen_dim );
		}


		private void NewWay( xCoord screen_dim )
		{
			Vector3 pos = new Vector3( 0, 0, 1f );
			Vector3 target = pos - 2f * Vector3.UnitZ;

			mViewMatrix = Matrix.CreateLookAt( pos, target, Vector3.UnitY );

			// left, right, bottom, top, near, far
			float left = mWorldView.GetMin().X;
			float right = mWorldView.GetMax().X;
			float top = mWorldView.GetMin().Y;
			float bottom = mAspect * (mWorldView.GetMax().X - mWorldView.GetMin().X);
			float near = 1;
			float far = -1;

			mProjectionMatrix = Matrix.CreateOrthographicOffCenter( left, right, bottom, top, near, far );
		}


		Matrix XICamera.GetViewMatrix()
		{
			return mViewMatrix;
		}


		Matrix XICamera.GetProjectionMatrix()
		{
			return mProjectionMatrix;
		}


		private xAABB2 ClampWorldView( xAABB2 world_view )
		{
			return world_view;
		}


		void XICamera.Update( GameTime game_time )
		{
			// move with controls
		}
	}


	public class XScreenCam : XICamera
	{
		private Matrix mView;
		private Matrix mProjection;


		public XScreenCam( xCoord screen_dim )
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
