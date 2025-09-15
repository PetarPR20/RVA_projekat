using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleModelServer.Interface
{
	public interface IStartable
	{
		void StartProduction(Model model);
	}
}
