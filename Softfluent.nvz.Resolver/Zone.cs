using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softfluent.nvz.Resolver
{
    public class Zone
    {
        public int X { get; private set; }

        public int Y { get; private set; }

        public int? Result { get; set; }

        private static readonly List<int> AllPossibilities = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        public List<int> Possibilities { get; private set; }

        public Zone(int? result, int x, int y)
        {
            X = x;
            Y = y;
            Result = result;
            Possibilities = result.HasValue ? new List<int>() : AllPossibilities.ToList();
        }

        private Zone()
        {
        }

        public bool RemovePossibility(int possibility)
        {
            if (Result.HasValue)
            {
                return false;
            }
            if (!Possibilities.Remove(possibility) || Possibilities.Count != 1)
            {
                return false;
            }

            ForceResult();
            return true;
        }

        public bool ForceResult()
        {
            if (Possibilities.Count == 0)
            {
                return false;
            }
            Result = Possibilities[0];
            Possibilities = new List<int>();
            return true;
        }

        public bool RemovePossibilities(IList<int> possibilities)
        {
            if (possibilities.Count == 1)
            {
                return RemovePossibility(possibilities[0]);
            }
            if (Result.HasValue)
            {
                return false;
            }
            if (Possibilities.RemoveAll(possibilities.Contains) == 0 || Possibilities.Count != 1)
            {
                return false;
            }
            ForceResult();
            return true;
        }

        public override string ToString()
        {
            return Result?.ToString() ?? ".";
        }

        public Zone Copy()
        {
            return new Zone
            {
                X = X,
                Y = Y,
                Result = Result,
                Possibilities = new List<int>(Possibilities)
            };
        }
    }
}
