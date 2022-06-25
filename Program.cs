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

            var route = new World.Route(cities.Values.ToArray());

            Console.WriteLine(route.isValid());
        }
    }
}