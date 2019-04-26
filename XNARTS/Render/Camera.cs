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
		private xCoord mScreenDim;
		private xCoord mWorldSize;
		private xAABB2 mWorldView; 
		private Matrix mViewMatrix;
		private Matrix mProjectionMatrix;

		private XListener< XTouch.MultiDragData >		mListener_MultiDrag;
		private XListener< XWorld.WorldRegenerated >    mListener_WorldRegenerated;

		private XTouch.MultiDragData    mMultiDragPrev;
		float                           mDampedMaxScreenSeparation;

		public XWorldCam( xCoord screen_dim )
		{
			mScreenDim = screen_dim;
			mAspect = ((float)(screen_dim.y)) / screen_dim.x;
			InitFromWorld();

			mListener_MultiDrag = new XListener<XTouch.MultiDragData>(	1, eEventQueueFullBehaviour.IgnoreOldest, 
																		"worldcammultidrag" );
			((XIBroadcaster<XTouch.MultiDragData>)XTouch.Instance()).GetBroadcaster().Subscribe( mListener_MultiDrag );

			mListener_WorldRegenerated = new XListener<XWorld.WorldRegenerated>(	1, eEventQueueFullBehaviour.IgnoreOldest, 
																					"worldcamworldregenerated" );
			((XIBroadcaster<XWorld.WorldRegenerated>)XWorld.Instance()).GetBroadcaster().Subscribe( mListener_WorldRegenerated );
		}

		private void InitFromWorld()
		{
			mWorldSize = XWorld.Instance().GetMapSize();

			// initial view of map
			xAABB2 world_view = new xAABB2( new Vector2( 0, 0 ), new Vector2( mWorldSize.x, mWorldSize.y ) );
			mWorldView = ClampWorldView( world_view );

			// view matrix is unchanging
			Vector3 pos = new Vector3( 0, 0, 1f );
			Vector3 target = pos - 2f * Vector3.UnitZ;
			mViewMatrix = Matrix.CreateLookAt( pos, target, Vector3.UnitY );
			CalcProjectionMatrix();

			mMultiDragPrev = new XTouch.MultiDragData( Vector2.Zero, 1f, 0 );
			mDampedMaxScreenSeparation = -1f;
		}
		private void CalcProjectionMatrix()
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
		private Vector2 CalcWorldPos( Vector2 screen_pos )
		{
			// refactor this out to central util somewhere
			float xf = screen_pos.X / mScreenDim.x;
			float yf = screen_pos.Y / mScreenDim.y;

			Vector2 min = mWorldView.GetMin();
			Vector2 max = mWorldView.GetMax();

			float wx = min.X + xf * (max.X - min.X);
			float wy = min.Y + yf * (max.Y - min.Y);

			return new Vector2( wx, wy );
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
			
			if( mListener_WorldRegenerated.GetMaxOne() != null )
			{
				InitFromWorld();
			}

			XTouch.MultiDragData multi_drag_msg = mListener_MultiDrag.GetMaxOne();

			if( multi_drag_msg != null )
			{
				if( multi_drag_msg.mFrameCount == 0 )
				{
					mMultiDragPrev = multi_drag_msg;
				}

				// figure out zoom.  damp so that human powered drag zoom is not jittery.
				// it's not that there is anything wrong with the measurement or the calculation, it's
				// that people don't seem to keep their fingers at constant separation when they mean to.
				const float k_zoom_damping = 0.8f;
				float ideal_zoom_ratio =(float)(mMultiDragPrev.mMaxScreenSeparation / multi_drag_msg.mMaxScreenSeparation);

				float zoom_ratio =  (mDampedMaxScreenSeparation > -1f)															?
									(k_zoom_damping * mDampedMaxScreenSeparation + (1f - k_zoom_damping) * ideal_zoom_ratio)	:
									ideal_zoom_ratio																			;

				mDampedMaxScreenSeparation = zoom_ratio;

				// place new world view so that the zoom point in world space is at the same place on the screen.
				Vector2 size_0 = mWorldView.GetSize();
				Vector2 size_1 = zoom_ratio * size_0;
				Vector2 avg_world_pos = CalcWorldPos( multi_drag_msg.mAvgScreenPos );
				float pos_x_fraction = multi_drag_msg.mAvgScreenPos.X / mScreenDim.x;
				float pos_y_fraction = multi_drag_msg.mAvgScreenPos.Y / mScreenDim.y;
				float dx_world = pos_x_fraction * size_1.X;
				float dy_world = pos_y_fraction * size_1.Y;
				Vector2 p0 = new Vector2( avg_world_pos.X - dx_world, avg_world_pos.Y - dy_world );
				Vector2 p1 = p0 + size_1;
				xAABB2 world_view = new xAABB2( p0, p1 );

				// figure out translation
				// this calculation assumes fullscreen, viewport not taken into consideration			
				Vector2 pixel_move = multi_drag_msg.mAvgScreenPos - mMultiDragPrev.mAvgScreenPos;
				Vector2 screen_fraction = new Vector2( pixel_move.X / mScreenDim.x, pixel_move.Y / mScreenDim.y );
				Vector2 world_view_size = world_view.GetSize();
				Vector2 world_move = new Vector2( screen_fraction.X * world_view_size.X, screen_fraction.Y * world_view_size.Y );
				world_view.Translate( -world_move );
				mMultiDragPrev = multi_drag_msg;

				// clamp and calc projection matrix
				mWorldView = ClampWorldView( world_view );
				CalcProjectionMatrix();
			}
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
