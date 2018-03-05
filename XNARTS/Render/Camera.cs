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
		Matrix CalcViewMatrix();
		Matrix CalcProjectionMatrix();
	}


	public class Camera : ICamera
	{
		private tCoord mScreenDim;


		public Camera( tCoord screen_dim )
		{
			mScreenDim = screen_dim;
		}


		Matrix ICamera.CalcViewMatrix()
		{
			return Matrix.CreateLookAt( new Vector3( 6, 4, 1f ), new Vector3( 6, 4, 0f ), new Vector3( 0f, 1f, 0f ) );
		}


		Matrix ICamera.CalcProjectionMatrix()
		{
			float viewport_scale = 10f;
			float aspect = ((float)(mScreenDim.y)) / mScreenDim.x;
			return Matrix.CreateOrthographicOffCenter( -viewport_scale, viewport_scale, -viewport_scale * aspect, viewport_scale * aspect, 0f, 2f );
		}
	}
}
