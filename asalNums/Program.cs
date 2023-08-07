namespace asalNums
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ust siniri girin");
            int bound = Convert.ToInt32(Console.ReadLine());
            bool isAsal = true;
            for (int i = 2; i < bound; i++)
            {
                for (int j = 2; j <= i / 2; j++)
                {
                    if (i % j == 0)
                    {
                        isAsal = false;
                        break;
                    }
                }
                if (i % 10 == 0)
                {
                    Console.WriteLine();
                }
                if (isAsal)
                {
                    Console.Write(i + " ");
                }
                isAsal = true;
            }
            Console.ReadLine();
        }
    }
}