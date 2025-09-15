using System;
using System.Collections.Generic;
using System.Text;

namespace Tim 2.CarFactory
{
	public interface IFailable
	{
		void ProductionFailed(Model model);
	}
}
