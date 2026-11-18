
using Cars.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Cars.WpfApp.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{

    private string selectedRepository = "Race Car";
    private Car? selectedCar;

    // Stores the currently selected car group.
    private CarGroup? selectedGroup;

    private string newCarName = "";
    private string newCarModel = "";


    private string newCarSpeed = "";
    private string newCarFuel = "";
    private string newCarCondition = "";
    private string newTurboLevel = "";
    private string newTowCapacity = "";
    private string newSirenOn = "False";
    private string newWaterLevel = "";
    private string newFarePerKm = "";


    private readonly SimulationRunner simulationRunner = new();
    private CancellationTokenSource? simulationCancellation;
    private string simulationReportText = "";
    private bool isSimulationRunning;


    private int simulationWorkload = 3;

    public int SimulationWorkload
    {
        get => simulationWorkload;
        set
        {
            if (simulationWorkload == value)
                return;

            simulationWorkload = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand RunSequentialCommand { get; }
    public RelayCommand RunParallelCommand { get; }
    public RelayCommand CancelSimulationCommand { get; }

    public string SimulationReportText
    {
        get => simulationReportText;
        set
        {
            simulationReportText = value;
            OnPropertyChanged();
        }
    }

    public bool IsSimulationRunning
    {
        get => isSimulationRunning;
        set
        {
            if (isSimulationRunning == value)
                return;

            isSimulationRunning = value;
            OnPropertyChanged();

            RunSequentialCommand?.RaiseCanExecuteChanged();
            RunParallelCommand?.RaiseCanExecuteChanged();
            CancelSimulationCommand?.RaiseCanExecuteChanged();
        }
    }

    public string NewCarName
    {
        get => newCarName;
        set
        {
            newCarName = value;
            OnPropertyChanged();
            AddCarCommand?.RaiseCanExecuteChanged();
            AddCarCommand?.RaiseCanExecuteChanged();
        }
    }

    public string NewCarModel
    {
        get => newCarModel;
        set
        {
            newCarModel = value;
            OnPropertyChanged();
            AddCarCommand?.RaiseCanExecuteChanged();
        }
    }

    public string NewCarSpeed
    {
        get => newCarSpeed;
        set
        {
            newCarSpeed = value;
            OnPropertyChanged();
            AddCarCommand?.RaiseCanExecuteChanged();
        }
    }

    public string NewCarFuel
    {
        get => newCarFuel;
        set
        {
            newCarFuel = value;
            OnPropertyChanged();
            AddCarCommand?.RaiseCanExecuteChanged();
        }
    }

    public string NewCarCondition
    {
        get => newCarCondition;
        set
        {
            newCarCondition = value;
            OnPropertyChanged();
            AddCarCommand?.RaiseCanExecuteChanged();
        }
    }

    public string NewTurboLevel
    {
        get => newTurboLevel;
        set
        {
            newTurboLevel = value;
            OnPropertyChanged();
            AddCarCommand?.RaiseCanExecuteChanged();
        }
    }

    public string NewTowCapacity
    {
        get => newTowCapacity;
        set
        {
            newTowCapacity = value;
            OnPropertyChanged();
            AddCarCommand?.RaiseCanExecuteChanged();
        }
    }

    public string NewSirenOn
    {
        get => newSirenOn;
        set
        {
            newSirenOn = value;
            OnPropertyChanged();
            AddCarCommand?.RaiseCanExecuteChanged();
        }
    }

    public string NewWaterLevel
    {
        get => newWaterLevel;
        set
        {
            newWaterLevel = value;
            OnPropertyChanged();
            AddCarCommand?.RaiseCanExecuteChanged();
        }
    }

    public string NewFarePerKm
    {
        get => newFarePerKm;
        set
        {
            newFarePerKm = value;
            OnPropertyChanged();
            AddCarCommand?.RaiseCanExecuteChanged();
        }
    }


    private readonly Repository<RaceCar> raceCars = new();
    private readonly Repository<TowTruck> towTrucks = new();
    private readonly Repository<PoliceCar> policeCars = new();
    private readonly Repository<Ambulance> ambulances = new();
    private readonly Repository<FireTruck> fireTrucks = new();
    private readonly Repository<Taxi> taxis = new();

    // Provides access to the selected world storage implementation.
    private readonly IWorldStorage worldStorage;


    public RelayCommand AddCarCommand { get; }
    public RelayCommand RemoveCarCommand { get; }
    public RelayCommand SortBySpeedCommand { get; }
    public RelayCommand GroupByTypeCommand { get; }

    // Command for creating a new car group.
    public RelayCommand AddGroupCommand { get; }

    // Commands for managing car group membership.
    public RelayCommand AddCarToGroupCommand { get; }
    public RelayCommand RemoveCarFromGroupCommand { get; }

    private bool showGroups;

    public bool ShowGroups
    {
        get => showGroups;
        set
        {
            showGroups = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CarsByType));
        }
    }

    // Receives the selected storage implementation through dependency injection.
    public MainViewModel(IWorldStorage worldStorage)
    {
        this.worldStorage = worldStorage
            ?? throw new ArgumentNullException(nameof(worldStorage));

        SortBySpeedCommand = new RelayCommand(
    _ => SortBySpeedDescending = !SortBySpeedDescending);

        GroupByTypeCommand = new RelayCommand(
            _ => ShowGroups = !ShowGroups);

        AddCarCommand = new RelayCommand(
            _ => AddCar());

        RemoveCarCommand = new RelayCommand(
            _ => RemoveCar(),
            _ => SelectedCar != null);


        RunSequentialCommand = new RelayCommand(
    _ => _ = RunSimulationAsync(false),
    _ => !IsSimulationRunning);

        RunParallelCommand = new RelayCommand(
            _ => _ = RunSimulationAsync(true),
            _ => !IsSimulationRunning);

        CancelSimulationCommand = new RelayCommand(
            _ => simulationCancellation?.Cancel(),
            _ => IsSimulationRunning);

        Log.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(LogText));
        };


        // Subscribes to events from all car repositories.
        SubscribeToRepository(raceCars);
        SubscribeToRepository(towTrucks);
        SubscribeToRepository(policeCars);

        // Connects commands to methods for managing car groups.
        AddGroupCommand = new RelayCommand(_ => AddGroup());

        AddCarToGroupCommand = new RelayCommand(
            _ => AddCarToGroup(),
            _ => SelectedCar != null && SelectedGroup != null);

        RemoveCarFromGroupCommand = new RelayCommand(
            _ => RemoveCarFromGroup(),
            _ => SelectedCar != null && SelectedGroup != null);
    }



    private void SubscribeToRepository<T>(Repository<T> repository)
        where T : Car
    {
        // Subscribes to events from existing cars.
        foreach (T car in repository.GetAll())
        {
            SubscribeToCar(car);
        }
    }

    private void SubscribeToCar(Car car)
    {
        car.Greeted += OnCarGreeted;
        car.Farewell += OnCarFarewell;
        car.Reacted += OnCarReacted;
        car.CrazyActionPerformed += OnCrazyActionPerformed;
    }

    // Removes ViewModel event subscriptions from a car.
    private void UnsubscribeFromCar(Car car)
    {
        car.Greeted -= OnCarGreeted;
        car.Farewell -= OnCarFarewell;
        car.Reacted -= OnCarReacted;
        car.CrazyActionPerformed -= OnCrazyActionPerformed;
    }


    private void OnCarGreeted(object? sender, CarEventArgs e)
    {
        Log.Add($"{((Car)sender!).Name} greeted {e.OtherCar.Name}.");
    }

    private void OnCarFarewell(object? sender, CarEventArgs e)
    {
        Log.Add($"{((Car)sender!).Name} said goodbye to {e.OtherCar.Name}.");
    }

    private void OnCarReacted(object? sender, CarEventArgs e)
    {
        Log.Add($"{((Car)sender!).Name} reacted to {e.OtherCar.Name}.");
    }

    private void OnCrazyActionPerformed(object? sender, CarEventArgs e)
    {
        Log.Add($"{((Car)sender!).Name} performed a crazy action!");
    }




    public ObservableCollection<Car> Cars { get; } = new();

    public ObservableCollection<Car> AllCars { get; } = new();

    // Stores cars selected for CPU simulation.
    public ObservableCollection<Car> SelectedCarsForSimulation { get; } = new();

    public ObservableCollection<IGrouping<string, Car>> CarsByType { get; } = new();

    public ObservableCollection<string> Log { get; } = new();

    public ObservableCollection<CarGroup> Groups { get; } = new();

    // Stores the cars that belong to the selected group.
    public ObservableCollection<Car> SelectedGroupCars { get; } = new();

    public string LogText => string.Join(
    Environment.NewLine,
    Log);


    // Stores the text used to search for cars.
    private string searchText = "";

    // Gets or sets the car search text.
    public string SearchText
    {
        get => searchText;
        set
        {
            searchText = value;
            OnPropertyChanged();
            RefreshCars();
        }
    }


    public string[] RepositoryTypes { get; } =
    {
        "Race Car",
        "Tow Truck",
        "Police Car",
        "Ambulance",
        "Fire Truck",
        "Taxi"
    };

    public string SelectedRepository
    {
        get => selectedRepository;
        set
        {
            if (selectedRepository == value)
                return;

            selectedRepository = value;
            OnPropertyChanged();
            RefreshCars();
        }
    }

    public Car? SelectedCar
    {
        get => selectedCar;
        set
        {
            selectedCar = value;
            OnPropertyChanged();
            RemoveCarCommand?.RaiseCanExecuteChanged();
            AddCarToGroupCommand?.RaiseCanExecuteChanged();
            RemoveCarFromGroupCommand?.RaiseCanExecuteChanged();
        }
    }

        // Gets or sets the currently selected car group.
    public CarGroup? SelectedGroup
        {
            get => selectedGroup;
            set
            {
                if (selectedGroup != value)
                {
                    selectedGroup = value;
                    OnPropertyChanged();
                    RefreshSelectedGroupCars();
                AddCarToGroupCommand?.RaiseCanExecuteChanged();
                    RemoveCarFromGroupCommand?.RaiseCanExecuteChanged();
                }
            }
        }

 

    private IEnumerable<Car> GetCarsFromSelectedRepository()
    {
        return SelectedRepository switch
        {
            "Race Car" => raceCars.GetAll(),
            "Tow Truck" => towTrucks.GetAll(),
            "Police Car" => policeCars.GetAll(),
            "Ambulance" => ambulances.GetAll(),
            "Fire Truck" => fireTrucks.GetAll(),
            "Taxi" => taxis.GetAll(),
            _ => Enumerable.Empty<Car>()
        };
    }



    // Refreshes the displayed cars using the selected repository and search text.

    private bool sortBySpeedDescending;

    public bool SortBySpeedDescending
    {
        get => sortBySpeedDescending;
        set
        {
            sortBySpeedDescending = value;
            OnPropertyChanged();
            RefreshCars();
        }
    }


    private void RefreshCars()
    {
        Cars.Clear();

        // Gets cars from the selected repository.


        IEnumerable<Car> filteredCars = GetCarsFromSelectedRepository();

        // Filters cars by name or model using LINQ.


        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filteredCars = filteredCars.Where(car =>
                car.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                car.Model.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        // Adds the filtered cars to the displayed collection.


        if (SortBySpeedDescending)
        {
            filteredCars = filteredCars
                .OrderByDescending(car => car.Speed);
        }
        else
        {
            filteredCars = filteredCars
                .OrderBy(car => car.Speed);
        }

        foreach (Car car in filteredCars)
            Cars.Add(car);


        // Clears the selection after refreshing the list.


        SelectedCar = null;
        RefreshGroups();
        RefreshAllCars();
    }

    private void RefreshAllCars()
    {
        AllCars.Clear();

        foreach (Car car in raceCars.GetAll()
                     .Cast<Car>()
                     .Concat(towTrucks.GetAll())
                     .Concat(policeCars.GetAll()))
        {
            AllCars.Add(car);
        }

        OnPropertyChanged(nameof(AllCars));
    }


    private void RefreshGroups()
    {
        CarsByType.Clear();

        IEnumerable<Car> allCars = raceCars.GetAll()
            .Cast<Car>()
            .Concat(towTrucks.GetAll())
            .Concat(policeCars.GetAll())
            .Concat(ambulances.GetAll())
            .Concat(fireTrucks.GetAll())
            .Concat(taxis.GetAll());

        var groups = allCars
            .GroupBy(car => car.GetType().Name)
            .OrderBy(group => group.Key);

        foreach (var group in groups)
        {
            CarsByType.Add(group);
        }

        OnPropertyChanged(nameof(CarsByType));
    }

    private void AddCar()
    {
        string name = NewCarName.Trim();
        string model = NewCarModel.Trim();


        // Validates the name and model.

        if (!CarValidator.IsValidName(name) ||
            string.IsNullOrWhiteSpace(model))
        {
            Log.Add("Enter a valid name and model.");
            return;
        }


        // Validates common car properties.

        if (!int.TryParse(NewCarSpeed, out int speed) ||
            !CarValidator.IsValidSpeed(speed))
        {
            Log.Add("Enter a valid speed.");
            return;
        }

        if (!int.TryParse(NewCarFuel, out int fuel) ||
            !CarValidator.IsValidFuel(fuel))
        {
            Log.Add("Enter a valid fuel value.");
            return;
        }

        if (!int.TryParse(NewCarCondition, out int condition) ||
            !CarValidator.IsValidCondition(condition))
        {
            Log.Add("Enter a valid condition value.");
            return;
        }


        // Creates the selected car type using the entered values.

        Car car;

        switch (SelectedRepository)
        {
            case "Race Car":
                if (!int.TryParse(NewTurboLevel, out int turboLevel) ||
                    turboLevel < 0 || turboLevel > 100)
                {
                    Log.Add("Turbo level must be between 0 and 100.");
                    return;
                }

                car = new RaceCar(
                    name, model, speed, fuel, condition, turboLevel);
                break;

            case "Tow Truck":
                if (!int.TryParse(NewTowCapacity, out int towCapacity) ||
                    towCapacity <= 0)
                {
                    Log.Add("Tow capacity must be greater than 0.");
                    return;
                }

                car = new TowTruck(
                    name, model, speed, fuel, condition, towCapacity);
                break;

            case "Police Car":
                if (!bool.TryParse(NewSirenOn, out bool sirenOn))
                {
                    Log.Add("Choose True or False for Siren On.");
                    return;
                }

                car = new PoliceCar(
                    name, model, speed, fuel, condition, sirenOn);
                break;

            case "Ambulance":
                car = new Ambulance(
                    name, model, speed, fuel, condition);
                break;

            case "Fire Truck":
                if (!int.TryParse(NewWaterLevel, out int waterLevel) ||
                    waterLevel < 0 || waterLevel > 100)
                {
                    Log.Add("Water level must be between 0 and 100.");
                    return;
                }

                car = new FireTruck(
                    name, model, speed, fuel, condition, waterLevel);
                break;

            case "Taxi":
                if (!int.TryParse(NewFarePerKm, out int farePerKm) ||
                    farePerKm <= 0)
                {
                    Log.Add("Fare per km must be greater than 0.");
                    return;
                }

                car = new Taxi(
                    name, model, speed, fuel, condition, farePerKm);
                break;

            default:
                Log.Add("Select a valid car type.");
                return;
        }


        // Adds the car to the matching repository.

        switch (car)
        {
            case RaceCar raceCar:
                raceCars.Add(raceCar);
                break;

            case TowTruck towTruck:
                towTrucks.Add(towTruck);
                break;

            case PoliceCar policeCar:
                policeCars.Add(policeCar);
                break;

            case Ambulance ambulance:
                ambulances.Add(ambulance);
                break;

            case FireTruck fireTruck:
                fireTrucks.Add(fireTruck);
                break;

            case Taxi taxi:
                taxis.Add(taxi);
                break;
        }


        // Subscribes to events from the new car.

        SubscribeToCar(car);


        // Updates the displayed list and selection.

        RefreshCars();
        SelectedCar = car;

        Log.Add($"{car.Name} ({car.Model}) was added to the garage.");


        // Clears the common input fields.

        NewCarName = "";
        NewCarModel = "";
        NewCarSpeed = "";
        NewCarFuel = "";
        NewCarCondition = "";


        // Clears only the special field used by this car type.

        switch (SelectedRepository)
        {
            case "Race Car":
                NewTurboLevel = "";
                break;

            case "Tow Truck":
                NewTowCapacity = "";
                break;

            case "Police Car":
                NewSirenOn = "False";
                break;

            case "Fire Truck":
                NewWaterLevel = "";
                break;

            case "Taxi":
                NewFarePerKm = "";
                break;
        }
    }


    // Creates a new car group.
    private void AddGroup()
    {
        // Sets the default name for a new group.
        string groupName = "New Group";
        int number = 2;

        // Generates a unique name if the default name already exists.
        while (Groups.Any(g =>
            g.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase)))
        {
            groupName = $"New Group {number}";
            number++;
        }

        // Creates a new group with a unique ID.
        var group = new CarGroup(groupName);

        // Adds the group to the collection.
        Groups.Add(group);

        // Selects the newly created group.
        SelectedGroup = group;

        // Records the action in the application log.
        Log.Add($"Group '{group.Name}' was created.");
    }


    // Adds the selected car to the selected group.
    private void AddCarToGroup()
    {
        // Checks whether a car and a group are selected.
        if (SelectedCar == null || SelectedGroup == null)
            return;

        // Prevents adding the same car more than once.
        if (SelectedGroup.CarIds.Contains(SelectedCar.Id))
        {
            Log.Add($"Car '{SelectedCar.Name}' is already in group '{SelectedGroup.Name}'.");
            return;
        }

        // Adds the selected car to the group.
        SelectedGroup.AddCar(SelectedCar.Id);

        RefreshSelectedGroupCars();

        // Records the action in the application log.
        Log.Add($"Car '{SelectedCar.Name}' was added to group '{SelectedGroup.Name}'.");
    }

    // Removes the selected car from the selected group.
    private void RemoveCarFromGroup()
    {
        // Checks whether a car and a group are selected.
        if (SelectedCar == null || SelectedGroup == null)
            return;

        // Checks whether the car belongs to the selected group.
        if (!SelectedGroup.CarIds.Contains(SelectedCar.Id))
        {
            Log.Add($"Car '{SelectedCar.Name}' is not in group '{SelectedGroup.Name}'.");
            return;
        }

        // Removes the selected car from the group.
        SelectedGroup.RemoveCar(SelectedCar.Id);

        RefreshSelectedGroupCars();

        // Records the action in the application log.
        Log.Add($"Car '{SelectedCar.Name}' was removed from group '{SelectedGroup.Name}'.");
    }

    // Updates the list of cars belonging to the selected group.
    private void RefreshSelectedGroupCars()
    {
        // Clears the previous list.
        SelectedGroupCars.Clear();

        // Stops if no group is selected.
        if (SelectedGroup == null)
            return;

        // Finds the cars that belong to the selected group.
        foreach (Guid carId in SelectedGroup.CarIds)
        {
            Car? car = AllCars.FirstOrDefault(c => c.Id == carId);

            if (car != null)
            {
                SelectedGroupCars.Add(car);
            }
        }
    }


    // Saves the current cars and groups to the selected storage.
    public async Task SaveWorldAsync(string path)
    {
        try
        {
            // Collects all cars from the three repositories.
            List<Car> allCars = raceCars.GetAll()
                .Cast<Car>()
                .Concat(towTrucks.GetAll())
                .Concat(policeCars.GetAll())
                .ToList();

            // Converts the current world into a storage DTO.
            WorldDto world = WorldMapper.ToWorldDto(allCars, Groups);

            // Saves the world using the injected storage implementation.
            await worldStorage.SaveAsync(world, path);

            Log.Add("World was saved successfully.");
        }
        catch (Exception ex)
        {
            // Reports the error without changing the current world.
            Log.Add($"Failed to save world: {ex.Message}");
        }
    }

    // Loads a saved world and replaces the current data after successful validation.
    public async Task LoadWorldAsync(string path)
    {
        try
        {
            // Reads the saved world using the selected storage implementation.
            WorldDto world = await worldStorage.LoadAsync(path);

            // Recreates all cars and groups before changing the current world.
            var restoredWorld = WorldMapper.FromWorldDto(world);

            List<Car> restoredCars = restoredWorld.Cars;
            List<CarGroup> restoredGroups = restoredWorld.Groups;

            // Separates the restored cars by their concrete types.
            List<RaceCar> restoredRaceCars = restoredCars
                .OfType<RaceCar>()
                .ToList();

            List<TowTruck> restoredTowTrucks = restoredCars
                .OfType<TowTruck>()
                .ToList();

            List<PoliceCar> restoredPoliceCars = restoredCars
                .OfType<PoliceCar>()
                .ToList();

            // Stops the current simulation before replacing the world.
            simulationCancellation?.Cancel();

            // Removes ViewModel event subscriptions from the old cars.
            foreach (Car car in raceCars.GetAll()
                         .Cast<Car>()
                         .Concat(towTrucks.GetAll())
                         .Concat(policeCars.GetAll()))
            {
                UnsubscribeFromCar(car);
            }

            // Clears the old repositories and their event subscriptions.
            raceCars.Clear();
            towTrucks.Clear();
            policeCars.Clear();

            // Restores cars without triggering greeting or reaction events.
            foreach (RaceCar car in restoredRaceCars)
            {
                raceCars.Restore(car);
                SubscribeToCar(car);
            }

            foreach (TowTruck car in restoredTowTrucks)
            {
                towTrucks.Restore(car);
                SubscribeToCar(car);
            }

            foreach (PoliceCar car in restoredPoliceCars)
            {
                policeCars.Restore(car);
                SubscribeToCar(car);
            }

            // Replaces the current groups with the restored groups.
            Groups.Clear();

            foreach (CarGroup group in restoredGroups)
            {
                Groups.Add(group);
            }

            // Clears the previously selected group.
            SelectedGroup = null;

            // Clears the list of cars from the previously selected group.
            SelectedGroupCars.Clear();

            // Clears selections that may reference cars from the old world.
            SelectedCarsForSimulation.Clear();
            SelectedCar = null;

            // Refreshes the collections displayed by the interface.
            RefreshCars();

            Log.Add("World was loaded successfully.");
        }
        catch (Exception ex)
        {
            // Keeps the current world unchanged if reading or validation fails.
            Log.Add($"Failed to load world: {ex.Message}");
        }
    }


    // Removes the selected car from its repository.


    private void RemoveCar()
    {
        // Checks whether a car is selected.
        if (SelectedCar == null)
            return;

        Car car = SelectedCar;
        bool removed = false;

        // Removes the car from the repository matching its type.
        switch (car)
        {
            case RaceCar raceCar:
                removed = raceCars.Remove(raceCar);
                break;

            case TowTruck towTruck:
                removed = towTrucks.Remove(towTruck);
                break;

            case PoliceCar policeCar:
                removed = policeCars.Remove(policeCar);
                break;

            case Ambulance ambulance:
                removed = ambulances.Remove(ambulance);
                break;

            case FireTruck fireTruck:
                removed = fireTrucks.Remove(fireTruck);
                break;

            case Taxi taxi:
                removed = taxis.Remove(taxi);
                break;
        }

        // Stops if the car was not removed.
        if (!removed)
            return;

        // Removes the deleted car from every group.
        foreach (CarGroup group in Groups)
        {
            group.RemoveCar(car.Id);
        }

        // Updates the displayed group.
        RefreshSelectedGroupCars();

        // Updates the displayed car list and clears the selection.
        RefreshCars();

        // Records the removal in the activity log.
        Log.Add($"{car.Name} was removed from the garage.");
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }

    private async Task RunSimulationAsync(bool parallel)
    {
        if (IsSimulationRunning)
            return;

        if (SelectedCarsForSimulation.Count < 3)
        {
            SimulationReportText = "Please select at least three cars.";
            return;
        }

        IsSimulationRunning = true;
        simulationCancellation = new CancellationTokenSource();

        try
        {
            var snapshots = SelectedCarsForSimulation
                .Select(CarSnapshot.FromCar)
                .ToArray();

            SimulationReport report;

            if (parallel)
            {
                report = await simulationRunner.RunParallelAsync(
                    snapshots,
                    SimulationWorkload,
                    simulationCancellation.Token);
            }
            else
            {
                report = await simulationRunner.RunSequentialAsync(
                    snapshots,
                    SimulationWorkload,
                    simulationCancellation.Token);
            }

            SimulationReportText =
                $"{(parallel ? "Parallel" : "Sequential")} simulation" +
                Environment.NewLine +
                string.Join(
                    Environment.NewLine,
                    report.Results.Select(result =>
                        $"{result.CarName}: {result.Status} | " +
                        $"Result: {result.Result?.ToString() ?? "-"} | " +
                        $"Error: {result.ErrorMessage ?? "-"} | " +
                        $"Duration: {result.Duration.TotalMilliseconds:F2} ms")) +
                Environment.NewLine +
                $"Total duration: {report.TotalDuration.TotalMilliseconds:F2} ms";
        }
        catch (Exception ex)
        {
            SimulationReportText = $"Simulation failed: {ex.Message}";
        }
        finally
        {
            simulationCancellation.Dispose();
            simulationCancellation = null;
            IsSimulationRunning = false;
        }
    }
}
