using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleModelServer.Interface
{
	public interface ICommand
	{
		void Execute();

		void Unexecute();
	}
}
