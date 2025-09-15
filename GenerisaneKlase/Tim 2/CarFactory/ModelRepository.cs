using System;
using System.Collections.Generic;
using System.Text;

namespace Tim 2.CarFactory
{
	public class ModelRepository
	{
		List<Model> models;
		IDataPersistance persistance;
		ILogger logger;

		public void AddModel(Model model)
		{
			throw new NotImplementedException();
		}

		public void RemoveModel(int id)
		{
			throw new NotImplementedException();
		}

		public void UpdateModel(Model model)
		{
			throw new NotImplementedException();
		}

		public List<Model> Load(string filePath)
		{
			throw new NotImplementedException();
		}

		public void Save(string filePath, List<Model> models)
		{
			throw new NotImplementedException();
		}
	}
}
