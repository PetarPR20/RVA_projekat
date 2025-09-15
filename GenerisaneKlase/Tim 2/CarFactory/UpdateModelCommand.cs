using System;
using System.Collections.Generic;
using System.Text;

namespace Tim 2.CarFactory
{
	public class UpdateModelCommand : CRUDCommand, ICommand
	{
		Model newModel;
	}
}
