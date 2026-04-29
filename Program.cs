using System.Text;


namespace GenerationPathsFromSquares
{
  internal class Program
  {
    static void Main()
    {
      Console.OutputEncoding = Encoding.UTF8;

      const int FIELD_LENGTH_X = 50 * 1;
      const int FIELD_LENGTH_Y = 50 * 1;
      Random rand = new();

      while (true)
      {
        //Console.ReadKey();
        Thread.Sleep(100);
        Console.Clear();
        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.White;
        GeneratorPathNew generatorPath = new(rand.Next(1, FIELD_LENGTH_X), rand.Next(1, FIELD_LENGTH_Y), rand.Next(2, FIELD_LENGTH_X), rand.Next(2, FIELD_LENGTH_Y), FIELD_LENGTH_X + 1, FIELD_LENGTH_Y + 1);
        generatorPath.GenerateRoad();
      }
    }
  }
}
