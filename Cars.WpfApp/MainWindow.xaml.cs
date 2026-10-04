using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Cars.Core;

namespace Cars.WpfApp;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<Car> cars = new();

    public MainWindow()
    {
        InitializeComponent();

        CarsGrid.ItemsSource = cars;

        CarTypeBox.ItemsSource = new[]
{
    "Race Car",
    "Tow Truck",
    "Police Car"
};

        SirenOnBox.ItemsSource = new[]
{
    "True",
    "False"
};

    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (CarsGrid.SelectedItem is not Car selectedCar)
        {
            MessageBox.Show(Properties.Resources.NoCarSelected);
            return;
        }

        cars.Remove(selectedCar);

        LogTextBox.AppendText(
            $"{selectedCar.Name}: {Properties.Resources.CarRemoved}" +
            Environment.NewLine);

        LogTextBox.ScrollToEnd();
    }


    private void TabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
    {

    }

    private void CarsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CarsGrid.SelectedItem is not Car selectedCar)
        {
            return;
        }

        UpdateSelectedCarDetails(selectedCar);

    }
    

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void AddCarButton_Click(object sender, RoutedEventArgs e)
    {
        string name = NameBox.Text.Trim();
        string model = ModelBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(model))
        {
            MessageBox.Show(Properties.Resources.InvalidModel);
            return;
        }

        if (!CarValidator.IsValidName(name))
        {
            MessageBox.Show(Properties.Resources.InvalidName);
            return;
        }

        if (!int.TryParse(SpeedBox.Text, out int speed) ||
            !CarValidator.IsValidSpeed(speed))
        {
            MessageBox.Show(Properties.Resources.InvalidSpeed);
            return;
        }

        if (!int.TryParse(FuelBox.Text, out int fuel) ||
            !CarValidator.IsValidFuel(fuel))
        {
            MessageBox.Show(Properties.Resources.InvalidFuel);
            return;
        }

        if (!int.TryParse(ConditionBox.Text, out int condition) ||
            !CarValidator.IsValidCondition(condition))
        {
            MessageBox.Show(Properties.Resources.InvalidCondition);
            return;
        }
        if (CarTypeBox.SelectedItem is null)
        {
            MessageBox.Show(Properties.Resources.CarTypeNotSelected);
            return;
        }

        string carType = CarTypeBox.SelectedItem.ToString()!;


        int turboLevel = 0;

        if (carType == "Race Car")
        {
            if (!int.TryParse(TurboLevelBox.Text, out turboLevel) ||
                turboLevel < 0 || turboLevel > 100)
            {
                MessageBox.Show(Properties.Resources.InvalidTurboLevel);
                return;
            }
        }

        int towCapacity = 0;


        if (carType == "Tow Truck")
        {
            if (!int.TryParse(TowCapacityBox.Text, out towCapacity) ||
                towCapacity <= 0)
            {
                MessageBox.Show(Properties.Resources.InvalidTowCapacity);
                return;
            }
        }


        bool sirenOn = false;

        if (carType == "Police Car")
        {
            if (SirenOnBox.SelectedItem is null)
            {
                MessageBox.Show(Properties.Resources.SirenNotSelected);
                return;
            }

            sirenOn = bool.Parse(SirenOnBox.SelectedItem.ToString()!);
        }


        var car = CreateCar(
            name,
            model,
            speed,
            fuel,
            condition,
            turboLevel,
            towCapacity,
            sirenOn);

        cars.Add(car);

        LogTextBox.AppendText(
    $"{car.Name}: {Properties.Resources.CarAdded}" +
    Environment.NewLine);

        LogTextBox.ScrollToEnd();

    }
    private Car CreateCar(
        string name,
        string model,
        int speed,
        int fuel,
        int condition,
        int turboLevel,
        int towCapacity,
        bool sirenOn)
    {
        return CarTypeBox.SelectedItem?.ToString() switch
        {
            "Race Car" => new RaceCar(
                name,
                model,
                speed,
                fuel,
                condition,
                turboLevel),

            "Tow Truck" => new TowTruck(
                name,
                model,
                speed,
                fuel,
                condition,
                towCapacity),

            "Police Car" => new PoliceCar(
                name,
                model,
                speed,
                fuel,
                condition,
                sirenOn),

            _ => throw new InvalidOperationException()
        };
    }

    private void Button_Click_2(object sender, RoutedEventArgs e)
    {
        if (CarsGrid.SelectedItem is not Car selectedCar)
        {
            MessageBox.Show(Properties.Resources.NoCarSelected);
            return;
        }

        ActionResult result = selectedCar.CrazyAction();

        UpdateSelectedCarDetails(selectedCar);

        string message = result.MessageKey switch
        {
            "RaceCarTurbo" => string.Format(
                Properties.Resources.RaceCarTurbo,
                result.Parameters),

            "TowTruckRescue" => $"{selectedCar.Name} {Properties.Resources.TowTruckRescue}",

            "PoliceSirenOn" => $"{selectedCar.Name} {Properties.Resources.PoliceSirenOn}",

            "PoliceSirenOff" => $"{selectedCar.Name} {Properties.Resources.PoliceSirenOff}",

            _ => result.MessageKey
        };

        LogTextBox.AppendText($"{selectedCar.Name} {message}" + Environment.NewLine);
        LogTextBox.ScrollToEnd();
    }
    private void UpdateSelectedCarDetails(Car selectedCar)
    {
        SelectedNameValue.Text = selectedCar.Name;
        SelectedModelValue.Text = selectedCar.Model;
        SelectedSpeedValue.Text = $"{selectedCar.Speed} km/h";
        SelectedFuelValue.Text = $"{selectedCar.Fuel}%";
        SelectedConditionValue.Text = $"{selectedCar.Condition}%";

        if (selectedCar is RaceCar raceCar)
        {
            SelectedTurboValue.Text = $"{raceCar.TurboLevel}%";
        }
        else
        {
            SelectedTurboValue.Text = "N/A";
        }

        if (selectedCar is TowTruck towTruck)
        {
            SelectedTowCapacityValue.Text = towTruck.TowCapacity.ToString();
        }
        else
        {
            SelectedTowCapacityValue.Text = "N/A";
        }

        if (selectedCar is PoliceCar policeCar)
        {
            SelectedSirenValue.Text = policeCar.SirenOn.ToString();
        }
        else
        {
            SelectedSirenValue.Text = "N/A";
        }
    }

    private void RefuelButton_Click(object sender, RoutedEventArgs e)
    {
        {
            if (CarsGrid.SelectedItem is not Car selectedCar)
            {
                MessageBox.Show(Properties.Resources.NoCarSelected);
                return;
            }

            if (selectedCar is not IRefuelable refuelable)
            {
                MessageBox.Show(Properties.Resources.CannotRefuel);
                return;
            }

            if (!int.TryParse(ActionAmountBox.Text, out int amount) || amount <= 0)
            {
                MessageBox.Show(Properties.Resources.InvalidFuelAmount);
                return;
            }

            ActionResult result = refuelable.Refuel(amount);

            if (!result.Success)
            {
                MessageBox.Show(
                    result.MessageKey == "FuelFull"
                        ? Properties.Resources.FuelFull
                        : Properties.Resources.InvalidFuelAmount);

                return;
            }

            UpdateSelectedCarDetails(selectedCar);

            string message = string.Format(
                Properties.Resources.Refueled,
                amount);

            LogTextBox.AppendText(
                $"{selectedCar.Name}: {message}" +
                Environment.NewLine);

            LogTextBox.ScrollToEnd();
        }
    }

    private void RepairButton_Click(object sender, RoutedEventArgs e)
    {
        if (CarsGrid.SelectedItem is not Car selectedCar)
        {
            MessageBox.Show(Properties.Resources.NoCarSelected);
            return;
        }

        if (selectedCar is not IRepairable repairable)
        {
            MessageBox.Show(Properties.Resources.CannotRepair);
            return;
        }

        if (!int.TryParse(ActionAmountBox.Text, out int amount) || amount <= 0)
        {
            MessageBox.Show(Properties.Resources.InvalidRepairAmount);
            return;
        }

        ActionResult result = repairable.Repair(amount);

        if (!result.Success)
        {
            MessageBox.Show(
                result.MessageKey == "ConditionFull"
                    ? Properties.Resources.ConditionFull
                    : Properties.Resources.InvalidRepairAmount);

            return;
        }

        UpdateSelectedCarDetails(selectedCar);

        string message = string.Format(
            Properties.Resources.Repaired,
            amount);

        LogTextBox.AppendText(
            $"{selectedCar.Name}: {message}" +
            Environment.NewLine);

        LogTextBox.ScrollToEnd();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        if (CarsGrid.SelectedItem is not Car selectedCar)
        {
            MessageBox.Show(Properties.Resources.NoCarSelected);
            return;
        }

        string message = selectedCar.Drive();

        LogTextBox.AppendText(
            $"{selectedCar.Name} {message}" +
            Environment.NewLine);

        LogTextBox.ScrollToEnd();
    }
}


