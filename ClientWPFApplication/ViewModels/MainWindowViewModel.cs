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

   


        public NewModelWrapper NewModel { get; set; } = new NewModelWrapper();

        
        
        public List<BodyType> BodyTypes { get; } = Enum.GetValues(typeof(BodyType)).Cast<BodyType>().ToList();

        // --- Engine form properties ---
        private EngineConfiguration _newEngineConfiguration;
        public EngineConfiguration NewEngineConfiguration
        {
            get => _newEngineConfiguration;
            set { _newEngineConfiguration = value; OnPropertyChanged(nameof(NewEngineConfiguration)); 
                    ((RelayCommand)AddEngineCommand).RaiseCanExecuteChanged();
            }
        }

        private FuelType _newEngineFuel;
        public FuelType NewEngineFuel
        {
            get => _newEngineFuel;
            set { _newEngineFuel = value; OnPropertyChanged(nameof(NewEngineFuel)); 
                    ((RelayCommand)AddEngineCommand).RaiseCanExecuteChanged();
            }
        }

        private int _newEngineNumberOfCylinders;
        public int NewEngineNumberOfCylinders
        {
            get => _newEngineNumberOfCylinders;
            set { _newEngineNumberOfCylinders = value; OnPropertyChanged(nameof(NewEngineNumberOfCylinders)); 
                    ((RelayCommand)AddEngineCommand).RaiseCanExecuteChanged();
            }
        }

        private int _newEnginePower;
        public int NewEnginePower
        {
            get => _newEnginePower;
            set { _newEnginePower = value; OnPropertyChanged(nameof(NewEnginePower)); 
                    ((RelayCommand)AddEngineCommand).RaiseCanExecuteChanged();
            }
        }

        private int _newEngineTorque;
        public int NewEngineTorque
        {
            get => _newEngineTorque;
            set { _newEngineTorque = value; OnPropertyChanged(nameof(NewEngineTorque)); 
                    ((RelayCommand)AddEngineCommand).RaiseCanExecuteChanged();
            }
        }

        private double _newEVVoltage;
        public double NewEVVoltage
        {
            get => _newEVVoltage;
            set { _newEVVoltage = value; OnPropertyChanged(nameof(NewEVVoltage)); 
                    ((RelayCommand)AddEngineCommand).RaiseCanExecuteChanged();
            }
        }

        private double _newEVBatteryCapacity;
        public double NewEVBatteryCapacity
        {
            get => _newEVBatteryCapacity;
            set { _newEVBatteryCapacity = value; OnPropertyChanged(nameof(NewEVBatteryCapacity)); 
                    ((RelayCommand)AddEngineCommand).RaiseCanExecuteChanged();
            }
        }

        private string _newEVCooling;
        public string NewEVCooling
        {
            get => _newEVCooling;
            set { _newEVCooling = value; OnPropertyChanged(nameof(NewEVCooling)); 
                    ((RelayCommand)AddEngineCommand).RaiseCanExecuteChanged();
            }
        }

        private PowerSource? _newEVSource;
        public PowerSource? NewEVSource
        {
            get => _newEVSource;
            set
            {
                if (_newEVSource != value)
                {
                    _newEVSource = value;
                    OnPropertyChanged(nameof(NewEVSource));
                    ((RelayCommand)AddEngineCommand).RaiseCanExecuteChanged();
                }
            }
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

        public List<PowerSource> PowerSources { get; } =
            Enum.GetValues(typeof(PowerSource)).Cast<PowerSource>().ToList();


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

            AddModelCommand = new RelayCommand(AddModel, CanAddModel);
            UpdateModelCommand = new RelayCommand(UpdateModel, CanUpdateModel);
            DeleteModelCommand = new RelayCommand(DeleteModel, CanDeleteModel);

            UndoCommand = new RelayCommand(Undo, CanUndo);
            RedoCommand = new RelayCommand(Redo, CanRedo);

            SearchCommand = new RelayCommand(SearchModels);
            ResetSearchCommand = new RelayCommand(ResetSearch);

            AddEngineCommand = new RelayCommand(AddEngine, CanAddEngine);

            NewModel.PropertyChanged += (s, e) =>
            {
                // Raise CanExecuteChanged for the commands whenever any relevant property changes
                ((RelayCommand)AddModelCommand).RaiseCanExecuteChanged();
                ((RelayCommand)UpdateModelCommand).RaiseCanExecuteChanged();
            };

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

                    // Remember currently selected model's ID (if any)
                    int? selectedModelId = SelectedModel?.Id;

                    // Synchronize Models without replacing the collection
                    // Remove models that no longer exist
                    for (int i = Models.Count - 1; i >= 0; i--)
                    {
                        if (!latestModels.Any(m => m.Id == Models[i].Id))
                            Models.RemoveAt(i);
                    }

                    // Add or update models from latestModels
                    foreach (var latest in latestModels)
                    {
                        var existing = Models.FirstOrDefault(m => m.Id == latest.Id);
                        if (existing == null)
                        {
                            // New model, add it
                            Models.Add(latest);
                        }
                        else
                        {
                            // Update existing model's properties (so binding updates)
                            existing.ModelName = latest.ModelName;
                            existing.BrandName = latest.BrandName;
                            existing.BodyType = latest.BodyType;
                            existing.NumberOfDoors = latest.NumberOfDoors;
                            existing.StateName = latest.StateName;
                            existing.ViableEngines = latest.ViableEngines;
                        }
                    }

                    // Restore selection if possible
                    if (selectedModelId.HasValue)
                    {
                        SelectedModel = Models.FirstOrDefault(m => m.Id == selectedModelId.Value);
                    }

                    // --- Update chart ---
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
                    Console.WriteLine("Error refreshing models: " + ex.Message);
                }
            };
            timer.Start();
        }

        private bool CommonFieldsValid =>
    NewEnginePower > 0 &&
    NewEngineTorque > 0 &&
    NewEngineFuel != null &&
    NewEngineConfiguration != null &&
    NewEngineNumberOfCylinders > 0;

        private bool EVFieldsValid =>
            NewEVVoltage > 0 &&
            NewEVBatteryCapacity > 0 &&
            !string.IsNullOrWhiteSpace(NewEVCooling) &&
            NewEVSource.HasValue;

        private bool EVFieldsEmpty =>
            NewEVVoltage == 0 &&
            NewEVBatteryCapacity == 0 &&
            string.IsNullOrWhiteSpace(NewEVCooling) &&
            !NewEVSource.HasValue;

        public bool CanAddEngine() =>
            CommonFieldsValid && (EVFieldsEmpty || EVFieldsValid);


        private void AddEngine()
        {
            if (SelectedModel == null) return;

            bool commonFieldsFilled =
                NewEnginePower > 0 &&
                NewEngineTorque > 0 &&
                NewEngineFuel != null &&
                NewEngineConfiguration != null &&
                NewEngineNumberOfCylinders > 0;

            bool evFieldsFilled =
                NewEVVoltage > 0 &&
                NewEVBatteryCapacity > 0 &&
                !string.IsNullOrWhiteSpace(NewEVCooling) &&
                NewEVSource.HasValue;

            // Only add if valid: either regular engine or EV
            if (!CanAddEngine())
                return; // invalid state, do nothing

            if (EVFieldsValid)
            {
                // --- Create EV ---
                ElectricMotor evMotor = new ElectricMotor(
                    voltage: NewEVVoltage,
                    source: NewEVSource.Value,
                    power: NewEnginePower,
                    torque: NewEngineTorque,
                    cooling: NewEVCooling,
                    batteryCapacity: NewEVBatteryCapacity
                );

                ElectricMotorAdapter adapter = new ElectricMotorAdapter(evMotor);
                adapter.Voltage = NewEVVoltage;
                adapter.Source = NewEVSource.Value;
                adapter.Cooling = NewEVCooling;
                adapter.BatteryCapacity = NewEVBatteryCapacity;

                DisplayedEngines.Add(adapter);
                SelectedModel.ViableEngines.Add(adapter);
                proxy.AddEngine(SelectedModel.Id, adapter);
            }
            else
            {
                // --- Create Regular Engine ---
                var engine = new Engine
                {
                    Configuration = NewEngineConfiguration,
                    Fuel = NewEngineFuel,
                    NumberOfCylinders = NewEngineNumberOfCylinders,
                    Power = NewEnginePower,
                    Torque = NewEngineTorque
                };

                DisplayedEngines.Add(engine);
                SelectedModel.ViableEngines.Add(engine);
                proxy.AddEngine(SelectedModel.Id, engine);
            }

            // --- Clear the form ---
            NewEngineConfiguration = EngineConfigurations.First();
            NewEngineFuel = EngineFuelTypes.First();
            NewEngineNumberOfCylinders = 0;
            NewEnginePower = 0;
            NewEngineTorque = 0;

            NewEVVoltage = 0;
            NewEVBatteryCapacity = 0;
            NewEVCooling = string.Empty;
            NewEVSource = null;

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

        private bool CanDeleteModel()
        {
            return SelectedModel != null;        
        }

        // Validation for AddModel
        private bool CanAddModel()
        {
            if (!NewModel.Id.HasValue || NewModel.Id <= 0)
                return false;

            // Make sure Id is unique
            if (Models.Any(m => m.Id == NewModel.Id.Value))
                return false;

            if (string.IsNullOrWhiteSpace(NewModel.ModelName))
                return false;

            if (string.IsNullOrWhiteSpace(NewModel.BrandName))
                return false;

            if (!NewModel.NumberOfDoors.HasValue || NewModel.NumberOfDoors <= 0)
                return false;

            return true;
        }

        // Validation for UpdateModel
        private bool CanUpdateModel()
        {
            if (SelectedModel == null)
                return false;

            if (string.IsNullOrWhiteSpace(NewModel.ModelName))
                return false;

            if (string.IsNullOrWhiteSpace(NewModel.BrandName))
                return false;

            if (!NewModel.NumberOfDoors.HasValue || NewModel.NumberOfDoors <= 0)
                return false;

            return true;
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
