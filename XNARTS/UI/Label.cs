using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNARTS
{
	public partial class XUI
	{
		public class Label : Widget
		{
			public Label( Widget parent, xAABB2 aabb ) : base( parent, aabb )
			{ }

			public Label( Widget parent, ePlacement placement ) : base( parent, placement )
			{ }
		}

	}
}
