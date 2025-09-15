using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleModelServer.Interface
{
	public interface IStopable
	{
		void StopProduction(Model model);
	}
}
