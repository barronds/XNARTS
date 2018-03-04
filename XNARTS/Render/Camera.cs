using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace XNARTS
{
	public interface ICamera
	{
		AABB2 GetAABB2();
	}


	public class Camera : ICamera
	{
		private AABB2 mBox;


		public Camera()
		{
			mBox = new AABB2();
		}


		AABB2 ICamera.GetAABB2()
		{
			return mBox;
		}
	}


	public class MainGameCam : Singleton< MainGameCam >, ICamera
	{
		private ICamera mCamera;


		private MainGameCam()
		{
		}


		public void Initialize()
		{
			mCamera = new Camera();
		}


		AABB2 ICamera.GetAABB2()
		{
			return mCamera.GetAABB2();
		}
	}
}
