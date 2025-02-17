using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        string filePath = @"D:\Prince\Test\IceCreamSalesAnalysis\IceCreamSalesAnalysis\sales-data.txt";
        var salesData = File.ReadAllLines(filePath).Skip(1).Select(ParseSale).ToList();

        // 1. Total Sales of the Store
        decimal totalSales = salesData.Sum(s => s.TotalPrice);
        Console.WriteLine($"Total Sales: {totalSales:C}");

        // Group data by Month
        var salesByMonth = salesData.GroupBy(s => new { s.Date.Year, s.Date.Month });

        foreach (var monthGroup in salesByMonth)
        {
            var month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthGroup.Key.Month);
            Console.WriteLine($"\nMonth: {month} {monthGroup.Key.Year}");

            // 2. Month-wise Sales Totals
            decimal monthTotal = monthGroup.Sum(s => s.TotalPrice);
            Console.WriteLine($"Total Sales: {monthTotal:C}");

            // 3. Most Popular Item (Most Quantity Sold)
            var mostPopularItem = monthGroup.GroupBy(s => s.Item)
                                            .OrderByDescending(g => g.Sum(s => s.Quantity))
                                            .First();
            Console.WriteLine($"Most Popular Item: {mostPopularItem.Key} ({mostPopularItem.Sum(s => s.Quantity)} sold)");

            // 4. Item Generating Most Revenue
            var topRevenueItem = monthGroup.GroupBy(s => s.Item)
                                           .OrderByDescending(g => g.Sum(s => s.TotalPrice))
                                           .First();
            Console.WriteLine($"Top Revenue Item: {topRevenueItem.Key} ({topRevenueItem.Sum(s => s.TotalPrice):C})");

            // 5. Min, Max, and Average Orders of Most Popular Item
            var itemOrders = mostPopularItem.Select(s => s.Quantity).ToList();
            Console.WriteLine($"Min Orders: {itemOrders.Min()}, Max Orders: {itemOrders.Max()}, Avg Orders: {itemOrders.Average():F2}");
        }
    }

    static Sale ParseSale(string line)
    {
        var parts = line.Split(',');
        return new Sale
        {
            Date = DateTime.ParseExact(parts[0], "yyyy-MM-dd", CultureInfo.InvariantCulture),
            Item = parts[1],
            Quantity = int.Parse(parts[2]),
            UnitPrice = decimal.Parse(parts[3]),
            TotalPrice = int.Parse(parts[2]) * decimal.Parse(parts[3])
        };
    }
}

class Sale
{
    public DateTime Date { get; set; }
    public string Item { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
