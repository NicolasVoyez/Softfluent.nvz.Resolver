using System;
using System.Diagnostics;

namespace Softfluent.nvz.Resolver
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            var solver = new Solver(new Grid(StringToSudoku(EasyGrid)));

            sw.Start();
            solver.OnBasicSolveStepPassed += Solver_OnBasicSolveStepPassed;

            solver.Solve();
            sw.Stop();

            Console.WriteLine("{0} ms for one hardcore grid", sw.ElapsedMilliseconds);
            Console.WriteLine(); 

            Show(solver.Grid.Zones, false);

            Console.WriteLine();
            Console.WriteLine(solver.Grid.IsCompleted ? "Done !" : "Impossible to finish");
            Console.ReadLine();
        }

        private const string EasyGrid = "1   6 2  \n" +
                                        " 2 4 875 \n" +
                                        " 84 2  39\n" +
                                        "2  8 6 47\n" +
                                        "  9   5  \n" +
                                        "76 3 4  2\n" +
                                        "39  4 87 \n" +
                                        " 126 7 9 \n" +
                                        "  7 3   5";

        private const string HardcoreGrid2 ="     5 4 \n" +
                                            " 8  43 17\n" +
                                            "2  1  5 6\n" +
                                            " 59      \n" +
                                            "6       9\n" +
                                            "      12 \n" +
                                            "4 2  1  8\n" +
                                            "59 76  3 \n" +
                                            " 6 8     \n";

        private const string HardcoreGrid = "   1 98  \n" +
                                            " 41  2 9 \n" +
                                            "2      1 \n" +
                                            "37  2 5  \n" +
                                            "   8 6   \n" +
                                            "  6 1  82\n" +
                                            " 1      6\n" +
                                            " 6 4  13 \n" +
                                            "  75 1   \n";

        private const string EmptyGrid = "1   6 2  \n" +
                                         "         \n" +
                                         "         \n" +
                                         "         \n" +
                                         "    4    \n" +
                                         "         \n" +
                                         "         \n" +
                                         "         \n" +
                                         "  7 3   5";

        private const string BadGrid =   "11  6 2  \n" +
                                         "         \n" +
                                         "         \n" +
                                         "         \n" +
                                         "    4    \n" +
                                         "         \n" +
                                         "         \n" +
                                         "         \n" +
                                         "  7 3   5";
        private static void Solver_OnBasicSolveStepPassed(object sender, EventArgs e)
        {
            var zones = ((Grid)sender).Zones;
            Show(zones);
        }

        private static void Show(Zone[,] zones, bool clearBefore = true)
        {
            if (clearBefore)
            {
                Console.Clear();
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Console.Write(zones[i, j]);
                    if (j%3 == 2)
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
                if (i%3 == 2)
                {
                    Console.WriteLine();
                }
            }
            Console.WriteLine("Press one key to continue");
            Console.ReadKey();
        }

        private static int?[,] StringToSudoku(string gridStr)
        {
            var grid = new int?[9,9];
            var splitted = gridStr.Split('\n');

            for (int i = 0; i < 9; i++)
            {
                var curr = splitted[i];
                for (int j = 0; j < 9; j++)
                {
                    int num;
                    grid[i, j] = int.TryParse(curr[j].ToString(), out num) ? num : (int?)null;
                }
            }
            return grid;
        }
    }
}
