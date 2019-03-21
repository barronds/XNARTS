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
		public interface ISelector
		{
			long GetID();
			void Draw();
		}

		public ISelector CreateSelector( String title, eFont title_font, Vector2 location)
		{
			return null;
		}

		public class Selector
		{
		}
	}

}
