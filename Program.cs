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
            var route = new World.Route(cities.Values.ToArray());
            Console.WriteLine("Randomizing...");

            for (int i = 0; i < 50000; i++)
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

            route = new World.Route(bestRoute);
            Console.WriteLine("Permuting...");
            for (int i = 0; i < 50000; i++)
            {
                route.permuteRouteMonteCarlo(0.3);
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