using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna;


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

			// initial view of map
			xAABB2 world_view = new xAABB2( new Vector2( 0, 0 ), new Vector2( mWorldSize.x, mWorldSize.y ) );
			mWorldView = ClampWorldView( world_view );

			// view matrix is unchanging
			Vector3 pos = new Vector3( 0, 0, 1f );
			Vector3 target = pos - 2f * Vector3.UnitZ;
			mViewMatrix = Matrix.CreateLookAt( pos, target, Vector3.UnitY );

			CalcProjectionMatrix( screen_dim );
		}


		private void CalcProjectionMatrix( xCoord screen_dim )
		{
			Vector2 min = mWorldView.GetMin();
			Vector2 max = mWorldView.GetMax();

			// left, right, bottom, top, near, far
			float left = min.X;
			float right = max.X;
			float top = min.Y;
			float bottom = max.Y;
			const float near = 1;
			const float far = -1;

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
			// make it true to aspect ratio and can't exceed width or height of map.
			Vector2 min = world_view.GetMin();
			Vector2 max = world_view.GetMax();

			// clamp edges
			min.X = Math.Max( 0, min.X );
			min.Y = Math.Max( 0, min.Y );
			max.X = Math.Min( mWorldSize.x, max.X );
			max.Y = Math.Min( mWorldSize.y, max.Y );

			// restore aspect ratio by bringing in x or y
			Vector2 span = max - min;
			float ideal_dy = span.X * mAspect;
			
			if( span.Y > ideal_dy )
			{
				max.Y = min.Y + span.X * mAspect;
			}		
			else
			{
				max.X = min.X + span.Y / mAspect;
			}

			return new xAABB2( min, max );
		}


		void XICamera.Update( GameTime game_time )
		{
			// move with touch controls
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
