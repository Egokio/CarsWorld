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


namespace Cars.WpfApp;

public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainViewModel();


        // Updates the UI when the user switches between the car table and grouped view.

        var viewModel = (MainViewModel)DataContext;
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

        CarTypeBox.ItemsSource = new[]
        {
            "Race Car",
            "Tow Truck",
            "Police Car"
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


