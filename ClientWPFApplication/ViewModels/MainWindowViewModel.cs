using Common;
using Common.Interface;
using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
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
using System.Windows.Threading;

namespace ClientWPFApplication.ViewModels
{
    internal class MainWindowViewModel
    {

        private ChannelFactory<IModelService> factory;
        private IModelService proxy;

        // in MainWindowViewModel
        public SeriesCollection StateSeries { get; set; }

        private void UpdateStateSeries()
        {
            // Count models by state
            var counts = Models.GroupBy(m => m.StateName)
                               .Select(g => new { State = g.Key, Count = g.Count() })
                               .ToList();

            StateSeries = new SeriesCollection();
            foreach (var c in counts)
            {
                StateSeries.Add(new PieSeries
                {
                    Title = c.State,
                    Values = new ChartValues<int> { c.Count },
                    DataLabels = true
                });
            }

            OnPropertyChanged(nameof(StateSeries));
        }


        public NewModelWrapper NewModel { get; set; } = new NewModelWrapper();
        
        public List<BodyType> BodyTypes { get; } = Enum.GetValues(typeof(BodyType)).Cast<BodyType>().ToList();

        // --- Engine form properties ---
        private EngineConfiguration _newEngineConfiguration;
        public EngineConfiguration NewEngineConfiguration
        {
            get => _newEngineConfiguration;
            set { _newEngineConfiguration = value; OnPropertyChanged(nameof(NewEngineConfiguration)); }
        }

        private FuelType _newEngineFuel;
        public FuelType NewEngineFuel
        {
            get => _newEngineFuel;
            set { _newEngineFuel = value; OnPropertyChanged(nameof(NewEngineFuel)); }
        }

        private int _newEngineNumberOfCylinders;
        public int NewEngineNumberOfCylinders
        {
            get => _newEngineNumberOfCylinders;
            set { _newEngineNumberOfCylinders = value; OnPropertyChanged(nameof(NewEngineNumberOfCylinders)); }
        }

        private int _newEnginePower;
        public int NewEnginePower
        {
            get => _newEnginePower;
            set { _newEnginePower = value; OnPropertyChanged(nameof(NewEnginePower)); }
        }

        private int _newEngineTorque;
        public int NewEngineTorque
        {
            get => _newEngineTorque;
            set { _newEngineTorque = value; OnPropertyChanged(nameof(NewEngineTorque)); }
        }

        // --- Enum lists for ComboBoxes ---
        public List<EngineConfiguration> EngineConfigurations { get; } =
            Enum.GetValues(typeof(EngineConfiguration)).Cast<EngineConfiguration>().ToList();

        public List<FuelType> EngineFuelTypes { get; } =
            Enum.GetValues(typeof(FuelType)).Cast<FuelType>().ToList();

        private ObservableCollection<Engine> _displayedEngines;
        public ObservableCollection<Engine> DisplayedEngines
        {
            get => _displayedEngines;
            set { _displayedEngines = value; OnPropertyChanged(nameof(DisplayedEngines)); }
        }

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

                DisplayedEngines.Clear();
                foreach (var engine in SelectedModel.ViableEngines)
                    DisplayedEngines.Add(engine);
            }
            else
            {
                ClearForm();
                DisplayedEngines.Clear();
            }

            ((RelayCommand)UpdateModelCommand).RaiseCanExecuteChanged();
            ((RelayCommand)DeleteModelCommand).RaiseCanExecuteChanged();
            ((RelayCommand)AddEngineCommand).RaiseCanExecuteChanged();
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

        public ICommand AddEngineCommand { get; }


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

            DisplayedEngines = new ObservableCollection<Engine>();

            AddModelCommand = new RelayCommand(AddModel);
            UpdateModelCommand = new RelayCommand(UpdateModel, CanUpdateOrDelete);
            DeleteModelCommand = new RelayCommand(DeleteModel, CanUpdateOrDelete);

            UndoCommand = new RelayCommand(Undo, CanUndo);
            RedoCommand = new RelayCommand(Redo, CanRedo);

            SearchCommand = new RelayCommand(SearchModels);
            ResetSearchCommand = new RelayCommand(ResetSearch);

            AddEngineCommand = new RelayCommand(AddEngine, CanAddEngine);

            var allStates = new[] { "DesignState", "InProductionState", "OutOfProductionState"};

            StateSeries = new SeriesCollection();

            foreach (var state in allStates)
            {
                StateSeries.Add(new PieSeries
                {
                    Title = state,
                    Values = new ChartValues<int> { 0 },
                    DataLabels = true
                });
            }

            OnPropertyChanged(nameof(StateSeries));

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += async (s, e) =>
            {
                try
                {
                    // Fetch latest models from the server
                    var latestModels = await Task.Run(() => proxy.GetAllModels());

                    // Update the existing ObservableCollection without replacing it
                    Models.Clear();
                    foreach (var m in latestModels)
                        Models.Add(m);


                    // Count models by state
                    var counts = Models
                        .GroupBy(m => m.StateName)
                        .ToDictionary(g => g.Key, g => g.Count());

                    foreach (var series in StateSeries.Cast<PieSeries>())
                    {
                        if (counts.TryGetValue(series.Title, out int count))
                            series.Values[0] = count;
                        else
                            series.Values[0] = 0;
                    }

                }
                catch (Exception ex)
                {
                    // Handle exceptions (server down, network issues, etc.)
                    Console.WriteLine("Error refreshing models: " + ex.Message);
                }
            };
            timer.Start();
        }

        private bool CanAddEngine()
        {
            return SelectedModel != null;
        }

        private void AddEngine()
        {
            if (SelectedModel == null) return;

            var engine = new Engine
            {
                Configuration = NewEngineConfiguration,
                Fuel = NewEngineFuel,
                NumberOfCylinders = NewEngineNumberOfCylinders,
                Power = NewEnginePower,
                Torque = NewEngineTorque
            };

            // Add to DisplayedEngines (UI updates automatically)
            DisplayedEngines.Add(engine);

            // Also update the underlying model
            SelectedModel.ViableEngines.Add(engine);

            proxy.AddEngine(SelectedModel.Id, engine);

            // Clear the form
            NewEngineConfiguration = EngineConfigurations.First();
            NewEngineFuel = EngineFuelTypes.First();
            NewEngineNumberOfCylinders = 0;
            NewEnginePower = 0;
            NewEngineTorque = 0;

            // Notify UI to refresh DataGrid
            OnPropertyChanged(nameof(DisplayedEngines));
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
