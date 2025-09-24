using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace Common.Interface
{
	[ServiceContract]
	public interface IModelService
	{
		[OperationContract]
		List<Model> SearchModels(
		   string modelName = null,
		   string brandName = null,
		   BodyType? bodyType = null,
		   int? numberOfDoors = null);


        [OperationContract]
        void AddEngine(int id, Engine engine);


        [OperationContract]
		bool CanUndo();

        [OperationContract]
        bool CanRedo();

        [OperationContract]
		void AddModel(Model model);

		[OperationContract]
		void RemoveModel(int id);

		[OperationContract]
		void UpdateModel(Model model);

		[OperationContract]
		void Undo();

		[OperationContract]
		void Redo();

		[OperationContract]
		List<Model> GetAllModels();

		[OperationContract]
		Model Alive();
	}
}
