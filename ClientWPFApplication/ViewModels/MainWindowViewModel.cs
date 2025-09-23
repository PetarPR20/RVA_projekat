using Common;
using Common.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ClientWPFApplication.ViewModels
{
    internal class MainWindowViewModel
    {

        private ChannelFactory<IModelService> factory;
        private IModelService proxy;


        public NewModelWrapper NewModel { get; set; } = new NewModelWrapper();
        
        public List<BodyType> BodyTypes { get; } = Enum.GetValues(typeof(BodyType)).Cast<BodyType>().ToList();

        public ObservableCollection<Model> Models { get; set; }

        private Model _selectedModel;
        public Model SelectedModel
        {
            get => _selectedModel;
            set
            {
                _selectedModel = value;
                OnPropertyChanged(nameof(SelectedModel));
                
                ((RelayCommand)UpdateModelCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteModelCommand).RaiseCanExecuteChanged();
                ((RelayCommand)UndoCommand).RaiseCanExecuteChanged();
                ((RelayCommand)RedoCommand).RaiseCanExecuteChanged();
                SelectedModelChanged();
            }
        }

        private void SelectedModelChanged()
        {
            if (SelectedModel != null)
            {
                NewModel.Id = SelectedModel.Id;
                NewModel.ModelName = SelectedModel.ModelName;
                NewModel.BrandName = SelectedModel.BrandName;
                NewModel.BodyType = SelectedModel.BodyType;
                NewModel.NumberOfDoors = SelectedModel.NumberOfDoors;
            }
            else
            {
                ClearForm();
            }

            ((RelayCommand)UpdateModelCommand).RaiseCanExecuteChanged();
            ((RelayCommand)DeleteModelCommand).RaiseCanExecuteChanged();
        }

        private void ClearForm()
        {
            NewModel.Id = 0;
            NewModel.ModelName = string.Empty;
            NewModel.BrandName = string.Empty;
            NewModel.BodyType = BodyType.HATCHBACK; // default
            NewModel.NumberOfDoors = 0;
        }

        public ICommand AddModelCommand { get; }
        public ICommand UpdateModelCommand { get; }
        public ICommand DeleteModelCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }

        public ICommand SearchCommand { get; }
        public ICommand ResetSearchCommand { get; }

        public MainWindowViewModel()
        {

            var binding = new NetTcpBinding(SecurityMode.None)
            {
                MaxReceivedMessageSize = 10_000_000,
                MaxBufferSize = 10_000_000,
                SendTimeout = TimeSpan.FromMinutes(5),
                ReceiveTimeout = TimeSpan.FromMinutes(5)
            };
            var endpoint = new EndpointAddress("net.tcp://localhost:9000/MyService");

            factory = new ChannelFactory<IModelService>(binding, endpoint);
            proxy = factory.CreateChannel();


            List<Model> result = proxy.GetAllModels();

            Models = new ObservableCollection<Model>(result);

            AddModelCommand = new RelayCommand(AddModel);
            UpdateModelCommand = new RelayCommand(UpdateModel, CanUpdateOrDelete);
            DeleteModelCommand = new RelayCommand(DeleteModel, CanUpdateOrDelete);

            UndoCommand = new RelayCommand(Undo, CanUndo);
            RedoCommand = new RelayCommand(Redo, CanRedo);

            SearchCommand = new RelayCommand(SearchModels);
            ResetSearchCommand = new RelayCommand(ResetSearch);
        }

        private void SearchModels()
        {

            var filteredModels = proxy.SearchModels(NewModel.ModelName,
                NewModel.BrandName,
                NewModel.BodyType,
                NewModel.NumberOfDoors);


            Models.Clear();
            foreach (var m in filteredModels)
                Models.Add(m);
        }

        private void ResetSearch()
        {
            RefreshModels();
        }

        private void RefreshModels()
        {
            ((RelayCommand)UndoCommand).RaiseCanExecuteChanged();
            ((RelayCommand)RedoCommand).RaiseCanExecuteChanged();
            List<Model> result = proxy.GetAllModels();
            Models.Clear();
            foreach (var model in result) Models.Add(model);
        }

        private void Undo()
        {
            proxy.Undo(); // call server-side undo
            RefreshModels();
        }

        private bool CanUndo() => proxy.CanUndo(); // returns true if undo is possible

        private void Redo()
        {
            proxy.Redo(); // call server-side redo
            RefreshModels();
        }

        private bool CanRedo() => proxy.CanRedo(); // returns true if redo is possible

        private bool CanUpdateOrDelete()
        {
            return SelectedModel != null;        
        }

        private void DeleteModel()
        {
            proxy.RemoveModel(SelectedModel.Id);
            RefreshModels();
        }

        private void UpdateModel()
        {
            if (SelectedModel == null) return;
            int id = NewModel.Id.Value;
            // Copy values from NewModel to the SelectedModel
            SelectedModel.Id = NewModel.Id.Value;
            SelectedModel.ModelName = NewModel.ModelName;
            SelectedModel.BrandName = NewModel.BrandName;
            SelectedModel.BodyType = NewModel.BodyType;
            SelectedModel.NumberOfDoors = NewModel.NumberOfDoors.Value;

            // Call server
            proxy.UpdateModel(SelectedModel);

            // Refresh list
            RefreshModels();

            // Optional: reselect the updated model
            SelectedModel = Models.FirstOrDefault(m => m.Id == id);
        }

        private void AddModel()
        {
            if (!NewModel.Id.HasValue || !NewModel.NumberOfDoors.HasValue)
                return; // optionally show a validation message

            var model = new Model(
                NewModel.Id.Value,
                NewModel.ModelName,
                NewModel.BrandName,
                NewModel.BodyType,
                NewModel.NumberOfDoors.Value
            );

            proxy.AddModel(model);
            RefreshModels();
            SelectedModel = model;

            // clear form safely
            NewModel.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
