namespace World
{
    public class Route
    {
        private List<City> citiesVisited = new List<City>(16);

        public Route(City city)
        {
            citiesVisited[0] = city;
            for (int i = 1; i < citiesVisited.Count; i++)
            {
                citiesVisited[i] = citiesVisited[i - 1].getConnections()[0];
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
            int numberOfCountries = 0;
            var unique = new List<City>();
            foreach (var city in citiesVisited)
            {

            }
            return true;
        }
    }
}