namespace World
{
    public class Route
    {
        private static Random rng = new Random();
        private List<City> citiesVisited;

        public Route(City city, bool isValid = true) :
        this(new List<City>(new[] { city }), isValid)
        { }

        public Route(IList<City> cities, bool isValid = true)
        {
            do
            {
                citiesVisited = new List<City>(16);
                citiesVisited.Add(cities[rng.Next(cities.Count)]);
                for (int i = 1; i < 16; i++)
                {
                    City[] possibilities = citiesVisited.Last().getConnections();
                    citiesVisited.Add(possibilities[rng.Next(possibilities.Length)]);
                }
            } while (!this.isValid() && isValid);
        }

        public override string ToString()
        {
            string output = "";
            for (int i = 0; i < citiesVisited.Count - 1; i++)
            {
                output += citiesVisited[i].ToString();
                output += " -> ";
            }
            output += citiesVisited.Last().ToString();
            return output;
        }

        public bool isValid()
        {
            var uniqueCapitals = new List<City>();
            foreach (var city in citiesVisited)
            {
                if (!uniqueCapitals.Contains(city.capital ?? new City("defaultCapital")))
                {
                    uniqueCapitals.Add(city.capital ?? new City("defaultCapital"));
                }
            }
            return uniqueCapitals.Count >= 7;
        }

        public void permuteRoute(bool validityCheck = true)
        {
            do
            {
                rng.Next(citiesVisited.Count);

            } while (!isValid() && validityCheck);
        }
    }
}