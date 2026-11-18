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
using Cars.WpfApp.ViewModels;
using System.Linq;
using Cars.Core;
using Cars.WpfApp.ViewModels;
using Microsoft.Win32;


namespace Cars.WpfApp;

public partial class MainWindow : Window
{



    // Receives the ViewModel created by the application startup.
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();

        // Uses the ViewModel provided by the application.
        DataContext = viewModel;


        // Updates the UI when the user switches between the car table and grouped view.

        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.ShowGroups))
            {
                CarsGrid.Visibility = viewModel.ShowGroups
                    ? Visibility.Collapsed
                    : Visibility.Visible;

                CarsGroupsList.Visibility = viewModel.ShowGroups
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        };

        CarsGrid.ItemsSource = ((MainViewModel)DataContext).Cars;


        SimulationCarsList.ItemsSource = ((MainViewModel)DataContext).AllCars;

        CarTypeBox.ItemsSource = new[]
        {
            "Race Car",
            "Tow Truck",
            "Police Car",
            "Ambulance",
            "Fire Truck",
            "Taxi"
        };

        // Sets the initial repository selection.
        CarTypeBox.SelectedItem =
            ((MainViewModel)DataContext).SelectedRepository;

        SirenOnBox.ItemsSource = new[]
        {
            "True",
            "False"
        };


        // Connects the action log to the ViewModel.
        ((MainViewModel)DataContext).Log.CollectionChanged += (s, e) =>
        {
            LogTextBox.Text = string.Join(
                Environment.NewLine,
                ((MainViewModel)DataContext).Log);
        

        };

    }

    private void SimulationCarsList_SelectionChanged(
object sender,
SelectionChangedEventArgs e)
    {
        if (DataContext is not MainViewModel viewModel)
            return;

        viewModel.SelectedCarsForSimulation.Clear();

        foreach (Car car in SimulationCarsList.SelectedItems.Cast<Car>())
        {
            viewModel.SelectedCarsForSimulation.Add(car);
        }
    }


        private void CarTypeBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
            {
                if (DataContext is not MainViewModel viewModel)
                    return;

                if (CarTypeBox.SelectedItem is not string selectedType)
                    return;

                // Updates the selected repository in the ViewModel.
                viewModel.SelectedRepository = selectedType;
            }

            private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {

    }



    private void TabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
    {

    }

    private void CarsGrid_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (DataContext is not MainViewModel viewModel)
            return;

        if (CarsGrid.SelectedItem is not Car selectedCar)
        {
            viewModel.SelectedCar = null;
            return;
        }

        viewModel.SelectedCar = selectedCar;
        UpdateSelectedCarDetails(selectedCar);
    }


    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {

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

            "AmbulanceEmergencyRun" => string.Format(
                Properties.Resources.AmbulanceEmergencyRun,
                result.Parameters),

            "FireTruckSpray" => string.Format(
                Properties.Resources.FireTruckSpray,
                result.Parameters),

            "FireTruckRefill" => string.Format(
                Properties.Resources.FireTruckRefill,
                result.Parameters),

            "TaxiWildRide" => string.Format(
                Properties.Resources.TaxiWildRide,
                result.Parameters),

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

        SelectedPatientsValue.Text = selectedCar is Ambulance ambulance
            ? ambulance.PatientsTransported.ToString()
            : "N/A";

        SelectedWaterLevelValue.Text = selectedCar is FireTruck fireTruck
            ? $"{fireTruck.WaterLevel}%"
            : "N/A";

        SelectedFareValue.Text = selectedCar is Taxi taxi
            ? $"{taxi.FarePerKm} per km"
            : "N/A";

        SelectedRidesValue.Text = selectedCar is Taxi taxiRides
            ? $"{taxiRides.RidesCompleted} rides, {taxiRides.Earnings} earned"
            : "N/A";
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

    private void SimulationWorkloadBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    // Opens a dialog and saves the current world to the selected file.
    private async void SaveWorldButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel viewModel)
            return;

        // Lets the user choose where to save the JSON file.
        SaveFileDialog dialog = new()
        {
            Filter = "World files (*.json;*.xml)|*.json;*.xml|JSON files (*.json)|*.json|XML files (*.xml)|*.xml",
            DefaultExt = ".xml",
            AddExtension = true,
            FileName = "CarsWorld"
        };

        if (dialog.ShowDialog() != true)
            return;

        await viewModel.SaveWorldAsync(dialog.FileName);
    }

    // Opens a dialog and loads the selected world file.
    private async void LoadWorldButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel viewModel)
            return;

        // Lets the user select a previously saved JSON file.
        OpenFileDialog dialog = new()
        {
            Filter = "World files (*.json;*.xml)|*.json;*.xml|JSON files (*.json)|*.json|XML files (*.xml)|*.xml",
            DefaultExt = ".xml",
            CheckFileExists = true
        };

        if (dialog.ShowDialog() != true)
            return;

        await viewModel.LoadWorldAsync(dialog.FileName);
    }

}


