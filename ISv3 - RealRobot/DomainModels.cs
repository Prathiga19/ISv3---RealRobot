using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace InventorySystem;

public class Item
{
    public uint InventoryLocation;   // 1/2/3 = a/b/c-kasser
    public string Name;
    public decimal PricePerUnit;
}

public class BulkItem : Item { public string MeasurementUnit; }
public class UnitItem : Item { public decimal Weight; }

public class OrderLine
{
    public Item Item { get; set; }
    public decimal Quantity { get; set; }
    public override string ToString() => $"{Item.Name} x {Quantity}";
}

public class Order
{
    public PrintableObservableCollection<OrderLine> OrderLines { get; set; }
    public DateTime Time { get; set; }
    public decimal TotalPrice()
    {
        decimal total = 0;
        foreach (var l in OrderLines) total += l.Item.PricePerUnit * l.Quantity;
        return total;
    }
    public override string ToString() => $"Order {Time:G}: {TotalPrice():0.##}$";
}

public class OrderBook
{
    public ObservableCollection<Order> ProcessedOrders { get; init; } = [];
    public ObservableCollection<Order> QueuedOrders { get; init; } = [];

    public void QueueOrder(Order order) => QueuedOrders.Add(order);

    public List<OrderLine> ProcessNextOrder()
    {
        List<OrderLine> lines = [];
        if (QueuedOrders.Count > 0)
        {
            var next = QueuedOrders[0];
            QueuedOrders.RemoveAt(0);
            ProcessedOrders.Add(next);
            foreach (var l in next.OrderLines) lines.Add(l);
        }
        return lines;
    }

    public decimal TotalRevenue()
    {
        decimal sum = 0;
        foreach (var o in ProcessedOrders) sum += o.TotalPrice();
        return sum;
    }
}

public class Customer
{
    public string Name;
    public List<Order> Orders = [];
    public void CreateOrder(OrderBook orderBook, Order order)
    {
        Orders.Add(order);
        orderBook.QueueOrder(order);
    }
}

public class PrintableObservableCollection<T> : ObservableCollection<T>
{
    public override string ToString() => string.Join(Environment.NewLine, Items);
}
