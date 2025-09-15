using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
	public class Model
	{
		private string modelName;
		private string brandName;
		private BodyType bodyType;
		private int numberOfDoors;
		public List<Engine> ViableEngines;
		int id;
		ConcreteState state;

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
