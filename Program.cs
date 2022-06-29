namespace WorldShipping
{
    internal class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            var cities = World.City.citiesFromSSV(
                "information/buying.ssv",
                "information/selling.ssv",
                "information/links.ssv",
                "information/prices.ssv",
                "information/captials.ssv"
            );

            World.Route bestRoute = null!;
            double bestProfit = -1;
            var route = new World.Route(cities.Values.ToArray());
            Console.WriteLine("Randomizing...");

            while (sw.Elapsed.TotalHours < 6)
            {
                route = new World.Route(cities.Values.ToArray());
                double profit = route.calculateOptimalProfit();
                if (bestProfit < profit)
                {
                    bestRoute = new World.Route(route);
                    bestProfit = profit;
                    Console.WriteLine(profit);
                }
            }

            Console.WriteLine("Best Route:");
            Console.WriteLine(bestRoute);
            Console.WriteLine($"Profit: {bestProfit}");
            Console.WriteLine(bestRoute.purchasePathToString());
            Console.WriteLine(bestRoute.calculateOptimalProfit());
        }
    }
}