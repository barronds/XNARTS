using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNARTS
{
	public class TestBed : XSingleton< TestBed >
	{
		public void Init()
		{

		}

		private TestBed()
		{
			// private constructor as per XSingleton
		}
	}
}
