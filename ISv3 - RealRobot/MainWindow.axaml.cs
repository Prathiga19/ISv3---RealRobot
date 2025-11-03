using System;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace InventorySystem;

public partial class MainWindow : Window
{
    public ItemSorterRobot robot;
    public OrderBook OrderBook { get; } = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        // Testdata (som før)
        var item1 = new UnitItem { Name = "M3 screw", PricePerUnit = 1,   InventoryLocation = 1 };
        var item2 = new UnitItem { Name = "M3 nut",   PricePerUnit = 1.5m, InventoryLocation = 2 };
        var item3 = new UnitItem { Name = "pen",      PricePerUnit = 1,   InventoryLocation = 3 };

        var order1 = new Order { OrderLines = new() { new() { Item=item1, Quantity=1 }, new() { Item=item2, Quantity=2 }, new() { Item=item3, Quantity=1 } }, Time = DateTime.Now.AddDays(-2) };
        var order2 = new Order { OrderLines = new() { new() { Item=item2, Quantity=1 } }, Time = DateTime.Now };

        var c1 = new Customer { Name = "Ramanda" };
        var c2 = new Customer { Name = "Totoro"  };
        c1.CreateOrder(OrderBook, order1);
        c2.CreateOrder(OrderBook, order2);
    }

    // KUN knappen forbinder – som i lærerens løsning
    public async void ConnectButton_OnClick(object? s, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await ConnectAndInitAsync(IpAddress?.Text);
    }

    private async Task ConnectAndInitAsync(string? ip)
    {
        try
        {
            var host = string.IsNullOrWhiteSpace(ip) ? "localhost" : ip;
            robot = new ItemSorterRobot(host);

            StatusMessages.Text += $"Connecting to {host}…{Environment.NewLine}";
            robot.Connect();
            StatusMessages.Text += $"Connected: {robot.Connected}{Environment.NewLine}";

            robot.PowerOn();
            await Task.Delay(500);
            StatusMessages.Text += $"{robot.RobotMode}{Environment.NewLine}";

            robot.BrakeRelease();
            await Task.Delay(500);
            StatusMessages.Text += $"{robot.RobotMode}{Environment.NewLine}";
        }
        catch (Exception ex)
        {
            StatusMessages.Text += $"Connection error: {ex.Message}{Environment.NewLine}";
        }
    }

    public async void ProcessButton_OnClick(object? s, Avalonia.Interactivity.RoutedEventArgs e)
    {
        StatusMessages.Text += $"Processing order{Environment.NewLine}";
        foreach (var line in OrderBook.ProcessNextOrder())
            for (var i = 0; i < line.Quantity; i++)
            {
                StatusMessages.Text += $"Picking up {line.Item.Name}{Environment.NewLine}";
                await robot.PickUp(line.Item.InventoryLocation);
            }
    }
}
