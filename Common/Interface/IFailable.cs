using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Interface
{
	public interface IFailable
	{
		void ProductionFailed(Model model);
	}
}
