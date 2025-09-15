using System;
using System.Collections.Generic;
using System.Text;

namespace Tim 2.CarFactory
{
	public interface IModelService
	{
		void AddModel(Model model);

		void RemoveModel(int id);

		void UpdateModel(Model model);

		void Undo();

		void Redo();

		void GetAllModels();
	}
}
