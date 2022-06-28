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

        private Tuple<List<List<Purchase>>, List<double>> calculateOptimalRoutePayments()
        {
            var purchases = new List<List<Purchase>>(new List<Purchase>[numberOfCities]);
            for (int i = 0; i < purchases.Count; i++)
            {
                purchases[i] = new List<Purchase>();
            }

            List<double> budgetList = new List<double>(new double[numberOfCities]);

            for (int i = 0; i < budgetList.Count; i++)
            {
                budgetList[i] = 3000;
            }

            for (int slotSection = 0; slotSection < 3; slotSection++)
            {
                int slotsFilled = 0;
                int numberOfCrates = slotSection == 2 ? 2 : 4;
                while (slotsFilled < numberOfCities - 1)
                {
                    var optimalPurchase = Purchase.getOptimizedPurchasesFromCities(citiesVisited, purchases, budgetList, slotsFilled, numberOfCrates, slotSection);
                    purchases[slotsFilled].Add(optimalPurchase);
                    for (int i = slotsFilled; i < budgetList.Count; i++)
                    {
                        budgetList[i] -= optimalPurchase.costInitial * numberOfCrates;
                    }
                    for (int i = slotsFilled + optimalPurchase.length!.Value; i < budgetList.Count; i++)
                    {
                        budgetList[i] += optimalPurchase.sellingPrice * numberOfCrates;
                    }
                    slotsFilled += optimalPurchase.length!.Value;
                }
            }
            return new Tuple<List<List<Purchase>>, List<double>>(purchases, budgetList);
        }

        public double calculateOptimalProfit()
        {
            return calculateOptimalRoutePayments().Item2.Last();
        }

        public List<List<Purchase>> getBestPurchasePath()
        {
            return calculateOptimalRoutePayments().Item1;
        }

        public string purchasePathToString()
        {
            var optimalRoutePurchases = calculateOptimalRoutePayments();
            var money = optimalRoutePurchases.Item2;
            var purchases = optimalRoutePurchases.Item1;
            string[] strings = new string[numberOfCities];

            for (int i = 0; i < purchases.Count; i++)
            {
                for (int j = 0; j < purchases[i].Count; i++)
                {
                    var purch = purchases[i][j];
                    strings[i] += purch.getBuyString() + ", ";
                    strings[i + purch.length!.Value] += purch.getSellString() + ", ";
                }
            }

            string output = "";

            for (var i = 0; i < money.Count; i++)
            {
                output += $"Day {i}, ${money[i]}, {strings[i]}\n";
            }

            return output;

        }
    }
}