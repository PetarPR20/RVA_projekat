using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Interface
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
