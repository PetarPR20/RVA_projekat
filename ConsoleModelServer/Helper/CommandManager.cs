using ConsoleModelServer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleModelServer.Helper
{
	public class CommandManager
	{
		Stack<ICommand> undoStack;
		Stack<ICommand> redoStack;

		public void ExecuteCommand(ICommand command)
		{
			throw new NotImplementedException();
		}

		public void Undo()
		{
			throw new NotImplementedException();
		}

		public void Redo()
		{
			throw new NotImplementedException();
		}
	}
}
