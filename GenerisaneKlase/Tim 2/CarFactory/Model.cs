using System;
using System.Collections.Generic;
using System.Text;

namespace Tim 2.CarFactory
{
	public class Model
	{
		private string modelName;
		private string brandName;
		private BodyType bodyType;
		private int numberOfDoors;
		public List<Engine> ViableEngines;
		int id;
		ConcreteStatet state;

		public void StartProduction()
		{
			throw new NotImplementedException();
		}

		public Model()
		{
			throw new NotImplementedException();
		}

		public void ModelRollOut()
		{
			throw new NotImplementedException();
		}

		public void StopProduction()
		{
			throw new NotImplementedException();
		}

		public void Redesign()
		{
			throw new NotImplementedException();
		}

		public void ProductionFailed()
		{
			throw new NotImplementedException();
		}
	}
}
