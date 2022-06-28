namespace World
{
    public class City
    {
        private string name;

        public City? capital { get; private set; }
        private double fuelCost;

        private List<City> singleFuelConnections = new List<City>();
        private List<City> doubleFuelConnections = new List<City>();
        private Dictionary<string, double> buyingItems = new Dictionary<string, double>();
        private Dictionary<string, double> sellingItems = new Dictionary<string, double>();

        public City(string cityName)
        {
            name = cityName;
        }

        private static Dictionary<string, double> getPricesFromSSV(string path)
        {
            Dictionary<string, double> startPrices = new Dictionary<string, double>();

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(null, path);
            }

            StreamReader pricesReader = new StreamReader(File.OpenRead(path));

            while (!pricesReader.EndOfStream)
            {
                string[] priceValues = pricesReader.ReadLine()!.Split(' ');
                startPrices.Add(priceValues[0], Convert.ToDouble(priceValues[1]));
            }

            return startPrices;
        }

        public static Dictionary<string, City> citiesFromSSV(string buyingPath, string sellingPath, string linksPath, string pricesPath, string capitalsPath)
        {
            var startPrices = getPricesFromSSV(pricesPath);

            Dictionary<string, City> output = new Dictionary<string, City>();

            if (!File.Exists(buyingPath))
            {
                throw new FileNotFoundException(null, buyingPath);
            }
            if (!File.Exists(sellingPath))
            {
                throw new FileNotFoundException(null, sellingPath);
            }

            StreamReader buyReader = new StreamReader(File.OpenRead(buyingPath));
            StreamReader sellReader = new StreamReader(File.OpenRead(sellingPath));
            while (!buyReader.EndOfStream)
            {
                Dictionary<string, double> buyItemsDict;

                string line = buyReader.ReadLine()!;
                string[] items = line.Split(' ');
                string name = items[0];
                buyItemsDict = new Dictionary<string, double>();
                for (int i = 1; i < items.Length; i++)
                {
                    buyItemsDict.Add(items[i], startPrices[items[i]]);
                }

                City tempCity = new City(name);
                tempCity.buyingItems = buyItemsDict;
                output.Add(name, tempCity);
            }

            while (!sellReader.EndOfStream)
            {
                Dictionary<string, double> sellItemsDict;

                string line = sellReader.ReadLine()!;
                string[] items = line.Split(' ');
                string name = items[0];
                sellItemsDict = new Dictionary<string, double>();
                for (int i = 1; i < items.Length; i += 2)
                {
                    sellItemsDict.Add(items[i], startPrices[items[i]] * (1 + Convert.ToDouble(items[i + 1])));
                }

                output[name].sellingItems = sellItemsDict;
            }

            if (!File.Exists(linksPath))
            {
                throw new FileNotFoundException(null, linksPath);
            }

            StreamReader linksReader = new StreamReader(File.OpenRead(linksPath));
            while (!linksReader.EndOfStream)
            {
                string line = linksReader.ReadLine()!;
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

            if (!File.Exists(capitalsPath))
            {
                throw new FileNotFoundException(null, sellingPath);
            }

            StreamReader capitalsReader = new StreamReader(File.OpenRead(capitalsPath));
            while (!capitalsReader.EndOfStream)
            {
                string line = capitalsReader.ReadLine()!;
                string[] lineValues = line.Split(' ');
                output[lineValues[0]].capital = output[lineValues[1]];
            }

            foreach (City city in output.Values)
            {
                if (city.name == "karachi")
                {
                    city.fuelCost = 50;
                }
                else if (!(city.capital ?? new City("")).buyingItems.Keys.Contains("cruoil"))
                {
                    city.fuelCost = 200;
                }
                else if (city.capital == city)
                {
                    city.fuelCost = 75;
                }
                else
                {
                    city.fuelCost = 100;
                }
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

        public override string ToString()
        {
            return name;
        }

        public string getVerboseString()
        {
            string output =
                $"Name: {name}\n"
                + $"Connecting Cities: {string.Join<City>(", ", getConnections())}\n"
                + $"Buying: {string.Join(",", buyingItems)}\n"
                + $"Selling:{string.Join(", ", sellingItems)}\n"
                + $"Fuel Cost: {fuelCost}";
            return output;
        }

        public List<Tuple<string, double, double>> getSortedPurchasePairs(City other)
        {
            var buying = buyingItems;
            var selling = other.sellingItems;

            var output = new List<Tuple<string, double, double>>();

            foreach (var i in buying.Keys)
            {
                if (selling.ContainsKey(i))
                {
                    output.Add(new Tuple<string, double, double>(i, buying[i], selling[i]));
                }
            }

            output.Sort(CompareDealsByProfit);

            output.Reverse();

            return output;
        }

        public static List<Tuple<string, double, double>> getSortedPurchasePairs(City buying, City selling)
        {
            return selling.getSortedPurchasePairs(buying);
        }

        private static int CompareDealsByProfit(Tuple<string, double, double> a, Tuple<string, double, double> b)
        {
            var newVal = (a.Item3 - a.Item2) - (b.Item3 - b.Item2);
            if (newVal > 0)
            {
                return 1;
            }
            else if (newVal < 0)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}