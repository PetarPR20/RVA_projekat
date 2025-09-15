using Common;
using ConsoleModelServer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleModelServer.Helper
{
	public class UpdateModelCommand : CRUDCommand, ICommand
	{
		Model newModel;

        public void Execute()
        {
            throw new NotImplementedException();
        }

        public void Unexecute()
        {
            throw new NotImplementedException();
        }
    }
}
