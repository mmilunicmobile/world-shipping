namespace WorldShipping
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Employee a = new Employee(5);
            Employee b = new SpecialEmployee(5);
            Console.WriteLine("Employee a: {0}", a.getPay());
            Console.WriteLine("Employee b: {0}", b.getPay());
        }
    }

    class Employee
    {
        private double pay;
        public Employee(double pay)
        {
            this.pay = pay;
        }
        public double getPay()
        {
            return pay;
        }
    }

    class SpecialEmployee : Employee
    {
        public SpecialEmployee(double pay) : base(pay) { }

        public double getPay()
        {
            return 500;
        }
    }
}