namespace World
{
    public class City
    {
        private string name;

        private bool isCapital;
        private double fuelCost;

        private List<City> singleFuelConnections;
        private List<City> doubleFuelConnections;
        private Dictionary<string, double> available = new Dictionary<string, double>();
        private Dictionary<string, double> saleMarkup = new Dictionary<string, double>();

        public City(string cityName, Dictionary<string, double> buying, Dictionary<string, double> selling)
        {
            name = cityName;
            isCapital = true;
            singleFuelConnections = new List<City>();
            doubleFuelConnections = new List<City>();
            available = buying;
            saleMarkup = selling;
            fuelCost = available.ContainsKey("cruoil") ? 75 : 200;
            if (name == "Karachi")
            {
                fuelCost = 50;
            }
        }

        public City(string cityName, bool sellsFuel)
        {
            name = cityName;
            isCapital = false;
            available = new Dictionary<string, double>();
            saleMarkup = new Dictionary<string, double>();
            singleFuelConnections = new List<City>();
            doubleFuelConnections = new List<City>();
            fuelCost = sellsFuel ? 100 : 200;
        }

        public static Dictionary<string, City> citiesFromSSV(string buyingPath, string sellingPath, string linksPath, string pricesPath)
        {
            Dictionary<string, double> startPrices = new Dictionary<string, double>();

            if (!File.Exists(pricesPath))
            {
                throw new FileNotFoundException(null, pricesPath);
            }

            StreamReader pricesReader = new StreamReader(File.OpenRead(buyingPath));
            string[] priceValues = pricesReader.ReadLine().Split(' ');
            for (int i = 0; i < priceValues.Length; i += 2)
            {
                startPrices.Add(priceValues[i], Convert.ToDouble(priceValues[i + 1]));
            }

            Dictionary<string, City> output = new Dictionary<string, City>();

            if (!File.Exists(buyingPath))
            {
                throw new FileNotFoundException(null, buyingPath);
            }
            if (!File.Exists(sellingPath))
            {
                throw new FileNotFoundException(null, sellingPath);
            }
            if (!File.Exists(linksPath))
            {
                throw new FileNotFoundException(null, sellingPath);
            }

            StreamReader buyReader = new StreamReader(File.OpenRead(buyingPath));
            StreamReader sellReader = new StreamReader(File.OpenRead(sellingPath));
            while (!buyReader.EndOfStream)
            {
                string name;

                Dictionary<string, double> buyItemsDict;
                {
                    string line = buyReader.ReadLine();
                    string[] items = line.Split(' ');
                    name = items[0];
                    buyItemsDict = new Dictionary<string, double>();
                    for (int i = 1; i < items.Length; i++)
                    {
                        buyItemsDict.Add(items[i], startPrices[items[i]]);
                    }
                }
                Dictionary<string, double> sellItemsDict;
                {
                    string line = sellReader.ReadLine();
                    string[] items = line.Split(' ');
                    sellItemsDict = new Dictionary<string, double>();
                    for (int i = 1; i < items.Length; i += 2)
                    {
                        sellItemsDict.Add(items[i], startPrices[items[i]] * Convert.ToDouble(items[i + 1]));
                    }
                }

                City tempCity;

                if (sellItemsDict.Count == 0)
                {
                    tempCity = new City(name, buyItemsDict["fuel"] == 1);
                }
                else
                {
                    tempCity = new City(name, buyItemsDict, sellItemsDict);
                }
            }

            StreamReader linksReader = new StreamReader(File.OpenRead(linksPath));
            while (!linksReader.EndOfStream)
            {
                string line = linksReader.ReadLine();
                List<string> lineValues = new List<string>(line.Split(' '));
                string name = lineValues[0];
                lineValues.RemoveAt(0);
                List<City> lineValuesCity = new List<City>(lineValues.Select(x => { return output[x]; }));
                List<City> singleFuel = new List<City>();
                List<City> doubleFuel = new List<City>();
                foreach (City value in lineValuesCity)
                {
                    if (singleFuel.Contains(value))
                    {
                        singleFuel.Remove(value);
                        doubleFuel.Add(value);
                    }
                    else
                    {
                        singleFuel.Add(value);
                    }
                }
                output[name].singleFuelConnections = singleFuel;
                output[name].doubleFuelConnections = doubleFuel;
            }

            return output;
        }
        public City[] getSharedConnections(City other)
        {
            List<City> output = new List<City>();
            foreach (
                City city in
                    other.singleFuelConnections.Concat(
                    other.doubleFuelConnections
                ))
            {
                if (singleFuelConnections.Contains(city) || doubleFuelConnections.Contains(city))
                {
                    output.Add(city);
                }
            }
            return output.ToArray();
        }

        public City[] getConnections()
        {
            return singleFuelConnections.Concat(doubleFuelConnections).ToArray();
        }

        public bool isConnected(City other)
        {
            return singleFuelConnections.Contains(other) || doubleFuelConnections.Contains(other);
        }

        public int connectionFuelCost(City other)
        {
            if (singleFuelConnections.Contains(other))
            {
                return 1;
            }
            else if (doubleFuelConnections.Contains(other))
            {
                return 2;
            }
            else
            {
                throw new Exception("Cannot get the fuel cost between two unconnected cities.");
            }
        }

        public double getFuelCost()
        {
            return fuelCost;
        }

        public string ToString()
        {
            return name;
        }
    }
}