namespace World
{
    public class Purchase
    {
        public double costInitial
        {
            get; private set;
        }
        public double sellingPrice
        {
            get; private set;
        }
        public int? length
        {
            get; private set;
        }

        private City startingCity;
        private City endingCity;
        private string good;

        public double profit
        {
            get
            {
                return sellingPrice - costInitial;
            }
        }

        public static Purchase getOptimizedPurchase(City a, City b, List<Purchase> exclusions, double budget, int length)
        {
            var possibilities = a.getSortedPurchasePairs(b);
            var budgetedPossibilities =
                from i in possibilities
                where i.costInitial <= budget
                select i;
            var unusedPossibilites =
                from i in possibilities
                where !exclusions.Any(temp => temp.good == i.good)
                select i;
            if (unusedPossibilites.Count() <= 0)
            {
                return new Purchase(a, b, 0, 0, "nothing", length);
            }
            var finalPurchase = unusedPossibilites.First();
            finalPurchase.length = length;
            return finalPurchase;
        }

        public static Purchase getOptimizedPurchasesFromCities(
            List<City> cities,
            List<List<Purchase>> exclusions,
            List<double> budget,
            int startingPos,
            int numberOfCrates
        )
        {
            Purchase? a1 = null, b1 = null, c1 = null, a2 = null, b2 = null, a3 = null;

            var purchaseHelper = (int start, int end) =>
            {
                return getOptimizedPurchase(
                    cities[startingPos + start],
                    cities[startingPos + end],
                    exclusions[startingPos + start],
                    budget.GetRange(startingPos + start, end - start).Min() / numberOfCrates,
                    end - start
                );
            };

            switch (cities.Count - startingPos - 1)
            {
                case >= 3:
                    a3 = purchaseHelper(0, 3);
                    b2 = purchaseHelper(1, 3);
                    c1 = purchaseHelper(2, 3);
                    goto case 2;
                case 2:
                    a2 = purchaseHelper(0, 2);
                    b1 = purchaseHelper(1, 2);
                    goto case 1;
                case 1:
                    a1 = purchaseHelper(0, 1);
                    break;
                default:
                    break;
            }

            switch (cities.Count - startingPos - 1)
            {
                case >= 3:
                    var choices = new List<Tuple<Purchase, double>>();
                    choices.Add(new Tuple<Purchase, double>(a1!, a1!.profit + b1!.profit + c1!.profit));
                    choices.Add(new Tuple<Purchase, double>(a1!, a1!.profit + b2!.profit));
                    choices.Add(new Tuple<Purchase, double>(a2!, a2!.profit + c1!.profit));
                    choices.Add(new Tuple<Purchase, double>(a3!, a3!.profit));
                    return choices.MaxBy(temp => temp.Item2)!.Item1;
                case 2:
                    if (a1!.profit + b1!.profit >= a2!.profit)
                    {
                        return a1;
                    }
                    else
                    {
                        return a2;
                    }
                case 1:
                    return a1!;
                default:
                    throw new ArgumentOutOfRangeException("cannot have lookforward of below one");
            }
        }

        public Purchase(City a, City b, double costInitial, double sellingPrice, string good, int? length)
        {
            startingCity = a;
            endingCity = b;
            this.costInitial = costInitial;
            this.sellingPrice = sellingPrice;
            this.good = good;
            this.length = length;
        }

        public static int CompareByProfit(Purchase a, Purchase b)
        {
            var newVal = (a.profit) - (b.profit);
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