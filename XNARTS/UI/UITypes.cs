using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace XNARTS
{
	public partial class XUI
	{
		public class _Position
		{
			private bool	mCentered;
			private Vector2 mPosition;

			// one constructor for centered and one for absolute position.  could add one for relative.
			public _Position()
			{
				mCentered = true;
				mPosition = Vector2.Zero;
			}
			public _Position( Vector2 pos )
			{
				mCentered = false;
				mPosition = pos;
			}

			public bool IsCentered()
			{
				return mCentered;
			}
			public Vector2 GetPosition()
			{
				return mPosition;
			}
		}
	}
}
