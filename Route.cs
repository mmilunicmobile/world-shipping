namespace World
{
    public class Route
    {
        private static Random rng = new Random();
        private List<City> citiesVisited;

        private const int numberOfCities = 16;

        public Route(City city, bool isValid = true) :
        this(new List<City>(new[] { city }), isValid)
        { }

        public Route(IList<City> cities, bool isValid = true)
        {
            do
            {
                citiesVisited = new List<City>(numberOfCities);
                citiesVisited.Add(cities[rng.Next(cities.Count)]);
                for (int i = 1; i < numberOfCities; i++)
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

        public void main()
        {
            var a = citiesVisited[0].getSortedPurchasePairs(citiesVisited[1]);
            foreach (var i in a)
            {
                Console.WriteLine(i);
            }
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
                int editValue = rng.Next(numberOfCities);
                switch (editValue)
                {
                    case 0:
                        {
                            var possibilities = citiesVisited[1].getConnections();
                            citiesVisited[0] = possibilities[rng.Next(possibilities.Count())];
                        }
                        break;
                    case numberOfCities - 1:
                        {
                            var possibilities = citiesVisited[numberOfCities - 2].getConnections();
                            citiesVisited[numberOfCities - 1] = possibilities[rng.Next(possibilities.Count())];
                        }
                        break;
                    default:
                        {
                            var possibilities = citiesVisited[editValue + 1].getSharedConnections(citiesVisited[editValue - 1]);
                            if (possibilities.Length < 1)
                            {
                                continue;
                            }
                            citiesVisited[editValue] = possibilities[rng.Next(possibilities.Count())];
                        }
                        break;
                }

            } while (!isValid() && validityCheck);
        }
    }
}