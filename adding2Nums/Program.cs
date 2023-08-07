namespace adding2Nums
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int num1,num2;
            num1 = Convert.ToInt32(Console.ReadLine());
            num2 = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine(add(num1, num2));
            Console.Write("selam");
            Console.Write("selam2");
        }
        static int add(int num1,int num2)
        {
            return num1+num2;
        }
    }
}