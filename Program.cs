namespace WorldShipping
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var cities = World.City.citiesFromSSV(
                "information/buying.ssv",
                "information/selling.ssv",
                "information/links.ssv",
                "information/prices.ssv",
                "information/captials.ssv"
            );

            World.Route bestRoute = null!;
            double bestProfit = -1;

            Console.WriteLine("Permuting...");

            for (int i = 0; i < 100000; i++)
            {
                var route = new World.Route(cities.Values.ToArray());
                double profit = route.calculateOptimalProfit();
                if (bestProfit <= profit)
                {
                    bestRoute = route;
                    bestProfit = profit;
                    Console.WriteLine(profit);
                }
            }

            Console.WriteLine("Best Route:");
            Console.WriteLine(bestRoute);
            Console.WriteLine($"Profit: {bestProfit}");
            Console.WriteLine(bestRoute.purchasePathToString());
        }
    }
}