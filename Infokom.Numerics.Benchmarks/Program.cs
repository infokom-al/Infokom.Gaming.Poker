#if DEBUG
using Infokom.Numerics;
using Infokom.Numerics.Atomics;
using Infokom.Numerics.Extensions;

using Spectre.Console;

using System.Threading.Channels;
namespace Infokom.Numerics.Benchmarks
{
	internal static class Program
	{
		public static void Main()
		{

			var π = Angle.Rad(Math.PI);

			Console.WriteLine($"π = {π}, {π:rad}, {π:deg}, {π:rot}");
			Console.WriteLine($"π/2 = {Angle.Rad(Math.PI / 2)}, {Angle.Rad(Math.PI / 2):rad}, {Angle.Rad(Math.PI / 2):deg}, {Angle.Rad(Math.PI / 2):rot}");
			Console.WriteLine($"π/4 = {Angle.Rad(Math.PI / 4)}, {Angle.Rad(Math.PI / 4):rad}, {Angle.Rad(Math.PI / 4):deg}, {Angle.Rad(Math.PI / 4):rot}");
			Console.WriteLine($"3π/4 = {Angle.Rad(3 * Math.PI / 4)}, {Angle.Rad(3 * Math.PI / 4):rad}, {Angle.Rad(3 * Math.PI / 4):deg}, {Angle.Rad(3 * Math.PI / 4):rot}");
		}
	}
}
#else
namespace Infokom.Numerics.Benchmarks
{
	internal static class Program
	{
		public static void Main()
		{
			_ = BenchmarkDotNet.Running.BenchmarkRunner.Run<Helpers.CombinatoricsBenckmarks>();
		}
	}
}
#endif