using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleModelServer.Interface
{
	public interface IDataPersistance
	{
		List<Model> Load(string filePath);

		void Save(string filePath, List<Model> models);
	}
}
