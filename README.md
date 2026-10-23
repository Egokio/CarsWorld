# Cars World

## Purpose

Cars World is a humorous WPF application inspired by the Disney Cars universe.

The application allows the user to create different cars, select them, view their details, remove them and perform special actions.

The project demonstrates object-oriented programming principles using C#.

## Solution Structure

The solution contains two projects:

- `Cars.Core` — contains the domain logic, classes, inheritance, interfaces, validation and car behavior.
- `Cars.WpfApp` — contains the WPF user interface and handles user interaction.

`Cars.WpfApp` references `Cars.Core`.

`Cars.Core` does not depend on WPF.

## Class Hierarchy

The main abstract base class is:

- `Car`

Derived classes:

- `RaceCar`
- `TowTruck`
- `PoliceCar`

The `Car` class contains common properties such as:

- Name
- Model
- Speed
- Fuel
- Condition

The properties use protected state changes through methods, so the WPF application cannot directly modify the internal state.

## Interfaces

The application uses two interfaces:

- `IRefuelable` — allows a car to be refueled.
- `IRepairable` — allows a car to be repaired.

`PoliceCar` implements both interfaces.

The application checks capabilities using the `is` operator before performing interface actions.

## Polymorphism and Crazy Action

`Car` contains the abstract method:

`CrazyAction()`

Each derived class overrides this method with different behavior.

### RaceCar

Activates the turbo and increases the turbo level.

### TowTruck

Performs a rescue action using its tow cable.

### PoliceCar

Turns the police siren on or off.

The WPF application calls `CrazyAction()` through the base `Car` reference, so the correct implementation is selected polymorphically.

## Validation and State Rules

Input validation is performed before creating a car.

The application validates:

- Car name
- Car model
- Speed
- Fuel
- Condition
- Turbo level
- Tow capacity
- Siren selection

Invalid input does not create or modify a car.

Refuel and repair actions also validate their amounts.

The internal state of a car cannot be changed directly from the WPF project.

## User Interface

The application uses a `TabControl` with three tabs:

### Cars

Displays all created cars in a `DataGrid`.

The user can:

- Add a car
- Select a car
- Remove a car

### Selected Car

Displays detailed information about the selected car.

The user can perform:

- Drive
- Crazy Action
- Refuel
- Repair

### Log

Displays successful actions and important application messages.

## Collection

Cars are stored in:

`ObservableCollection<Car>`

The collection uses the abstract base type, allowing different car types to exist in the same collection.

## Tested Use Cases

The following scenarios were tested:

1. Creating a valid Race Car.
2. Creating a valid Tow Truck.
3. Creating a valid Police Car.
4. Rejecting invalid speed, fuel and condition values.
5. Rejecting invalid action amounts.
6. Removing a selected car.
7. Using Crazy Action for all three car types.
8. Refueling Race Car and Police Car.
9. Repairing Tow Truck and Police Car.
10. Using Drive and receiving different messages for different car types.
11. Selecting a car and displaying its current state.
12. Checking that invalid actions do not change valid car state.

## Why TabControl Was Used

`TabControl` was used to separate the main application functions into clear sections.

The Cars tab is used for managing the collection, the Selected Car tab is used for interacting with one selected object, and the Log tab is used for viewing application history.

This makes the interface easier to understand and keeps different responsibilities visually separated.

## OOP Concepts Demonstrated

The project demonstrates:

- Abstraction
- Inheritance
- Encapsulation
- Interfaces
- Polymorphism
- Abstract methods
- Method overriding
- Collections
- Capability checks using `is`
- Validation
- Separation of Core and UI responsibilities


## Simulation (Vol.3)

The application includes CPU-intensive simulations for selected cars.

### Simulation Features

- Select at least three cars for a simulation.
- Run calculations sequentially or in parallel.
- Use immutable `CarSnapshot` records as simulation inputs.
- Configure the simulation workload.
- Measure the duration of each calculation and the total execution time.
- Cancel running calculations using `CancellationToken`.
- Handle errors independently for each car.
- Display each car's status, result or error message, and execution duration.

### Sequential and Parallel Execution

**Sequential mode** processes one car at a time. Each calculation finishes before the next one begins.

**Parallel mode** starts separate tasks for the selected cars and waits for all calculations to finish using `Task.WhenAll`.

Both modes use `Stopwatch` to measure the total execution time.

### Cancellation and Error Handling

The simulation checks the cancellation token regularly during CPU-intensive calculations.

Each car's calculation is handled independently. If one calculation fails, its error is included in the report while other calculations can continue.

The report identifies each calculation as `Success`, `Error`, or `Cancelled`, where applicable.

### Simulation Report

The report includes:

- Execution mode
- Car name and status
- Calculation result, when successful
- Error or cancellation message, when applicable
- Individual calculation duration
- Total simulation duration

### Running a Simulation

1. Select at least three cars in the simulation section.
2. Choose a workload.
3. Click **Run Sequential** or **Run Parallel**.
4. Review the simulation report.
5. Click **Cancel** to request cancellation during a running simulation.


## AI Usage

AI assistance was used during development to help with:

- Understanding WPF and C# concepts.
- Planning the class hierarchy and interfaces.
- Debugging compilation and event-handler problems.
- Improving validation and resource usage.
- Reviewing whether the implementation followed the assignment requirements.

All generated code was checked manually, tested in Visual Studio and adjusted when necessary.

## How to Run

1. Open the solution in Visual Studio.
2. Make sure `Cars.WpfApp` is set as the startup project.
3. Build the solution.
4. Run the application.
5. Add cars, test their actions, and run simulations using the simulation section.