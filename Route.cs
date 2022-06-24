namespace World
{
    public class Route
    {
        private static Random rng = new Random();
        private List<City> citiesVisited = new List<City>(16);

        public Route(City city)
        {
            citiesVisited.Add(city);
            for (int i = 1; i < 16; i++)
            {
                City[] possibilities = citiesVisited.Last().getConnections();
                citiesVisited.Add(possibilities[rng.Next(possibilities.Length)]);
            }
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
            var unique = new List<City>();
            foreach (var city in citiesVisited)
            {

            }
            return true;
        }
    }
}