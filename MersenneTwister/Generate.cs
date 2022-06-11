using System.IO;
using System.Text;
using MersenneTwister;
namespace MersenneTwister
{
    class Program
    {
        static void Main(string[] args)
        {
            int seed = Guid.NewGuid().GetHashCode(); // bu her zaman tohum için iyi bir sayı üretmelidir
            RandomMersenne m = new RandomMersenne((uint)seed);
            if (File.Exists("random.txt"))
            {
                File.Delete("random.txt");
            }
            var file = new FileStream("random.txt", FileMode.Create);
            var writer = new StreamWriter(file,Encoding.UTF8);
            for (int i = 0; i < 1000000; i++)
            {
                writer.Write(m.IRandom(0,1) + " ");
            }
        }
    }
}