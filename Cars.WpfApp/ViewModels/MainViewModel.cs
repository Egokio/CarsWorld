
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Cars.Core;
using System.Linq;

namespace Cars.WpfApp.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{

    private string selectedRepository = "Race Car";
    private Car? selectedCar;


    private string newCarName = "";
    private string newCarModel = "";


    private string newCarSpeed = "";
    private string newCarFuel = "";
    private string newCarCondition = "";
    private string newTurboLevel = "";
    private string newTowCapacity = "";
    private string newSirenOn = "False";


    public string NewCarName
    {
        get => newCarName;
        set
        {
            newCarName = value;
            OnPropertyChanged();
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


    private readonly Repository<RaceCar> raceCars = new();
    private readonly Repository<TowTruck> towTrucks = new();
    private readonly Repository<PoliceCar> policeCars = new();

    public RelayCommand AddCarCommand { get; }
    public RelayCommand RemoveCarCommand { get; }
    public RelayCommand SortBySpeedCommand { get; }
    public RelayCommand GroupByTypeCommand { get; }

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

    public MainViewModel()
    {

        SortBySpeedCommand = new RelayCommand(
    _ => SortBySpeedDescending = !SortBySpeedDescending);

        GroupByTypeCommand = new RelayCommand(
            _ => ShowGroups = !ShowGroups);

        AddCarCommand = new RelayCommand(
            _ => AddCar());

        RemoveCarCommand = new RelayCommand(
            _ => RemoveCar(),
            _ => SelectedCar != null);


        Log.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(LogText));
        };


        // Subscribes to events from all car repositories.
        SubscribeToRepository(raceCars);
        SubscribeToRepository(towTrucks);
        SubscribeToRepository(policeCars);
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

    public ObservableCollection<IGrouping<string, Car>> CarsByType { get; } = new();

    public ObservableCollection<string> Log { get; } = new();

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
        "Police Car"
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
        }
    }

    private IEnumerable<Car> GetCarsFromSelectedRepository()
    {
        return SelectedRepository switch
        {
            "Race Car" => raceCars.GetAll(),
            "Tow Truck" => towTrucks.GetAll(),
            "Police Car" => policeCars.GetAll(),
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
    }

    private void RefreshGroups()
    {
        CarsByType.Clear();

        IEnumerable<Car> allCars = raceCars.GetAll()
            .Cast<Car>()
            .Concat(towTrucks.GetAll())
            .Concat(policeCars.GetAll());

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
        }


        // Stops if the car was not removed.

        if (!removed)
            return;


        // Updates the displayed list and clears the selection.

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
}
