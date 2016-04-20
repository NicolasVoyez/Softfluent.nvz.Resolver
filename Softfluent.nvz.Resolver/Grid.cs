using System;
using System.Collections.Generic;
using System.Linq;

namespace Softfluent.nvz.Resolver
{
    public class Grid
    {
        public Zone[,] Zones { get; set; }

        public bool IsCompleted => AllZones.All(x => x.Result.HasValue);

        public bool IsDead => AllZones.Any(x => x.Possibilities.Count == 0 && !x.Result.HasValue);

        public Grid(int?[,] initialGrid)
        {
            Zones = new Zone[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Zones[i, j] = new Zone(initialGrid[i, j], i, j);
                }
            }
        }

        private Grid()
        {
        }

        #region Clean

        private static bool Clean(Func<int, int, Zone> getZone)
        {
            var anyGlobalChange = false;
            for (int i = 0; i < 9; i++)
            {
                var existing = new List<int>();
                for (int j = 0; j < 9; j++)
                {
                    var curr = getZone(i, j);
                    if (curr.Result.HasValue)
                    {
                        existing.Add(curr.Result.Value);
                    }
                }
                bool hasChanges;
                do
                {
                    hasChanges = false;
                    var newlyFound = new List<int>();
                    for (var j = 0; j < 9; j++)
                    {
                        var curr = getZone(i, j);
                        if (!curr.RemovePossibilities(existing))
                        {
                            continue;
                        }

                        anyGlobalChange = true;
                        hasChanges = true;
                        newlyFound.Add(curr.Result.Value);
                    }
                    existing = newlyFound;

                } while (hasChanges);
            }
            return anyGlobalChange;
        }

        public bool CleanHorizontal()
        {
            return Clean((i, j) => Zones[i, j]);
        }

        public bool CleanVertical()
        {
            return Clean((i, j) => Zones[j, i]);
        }

        public bool Clean3X3()
        {
            var anyGlobalChange = false;
            for (int iG = 0; iG < 9; iG += 3)
            {
                for (int jG = 0; jG < 9; jG += 3)
                {
                    if (CleanMiniZone(iG, jG))
                    {
                        anyGlobalChange = true;
                    }
                }
            }
            return anyGlobalChange;
        }

        private bool CleanMiniZone(int mod3I, int mod3J)
        {
            bool anyGlobalChange = false;
            var existing = new List<int>();
            for (int i = mod3I; i < mod3I + 3; i++)
            {
                for (int j = mod3J; j < mod3J + 3; j++)
                {
                    var curr = Zones[i, j];
                    if (curr.Result.HasValue)
                    {
                        existing.Add(curr.Result.Value);
                    }
                }
            }

            bool hasChanges;
            do
            {
                hasChanges = false;
                var newlyFound = new List<int>();
                for (int i = mod3I; i < mod3I + 3; i++)
                {
                    for (int j = mod3J; j < mod3J + 3; j++)
                    {
                        var curr = Zones[i, j];
                        if (!curr.RemovePossibilities(existing))
                        {
                            continue;
                        }

                        anyGlobalChange = true;
                        hasChanges = true;
                        newlyFound.Add(curr.Result.Value);
                    }
                }
                existing = newlyFound;

            } while (hasChanges);

            return anyGlobalChange;
        }

        #endregion

        #region Error state

        public bool HasError => CheckHorizontalError() || CheckVerticalError() || Check3X3Error();

        private bool Check3X3Error()
        {
            for (int iG = 0; iG < 9; iG += 3)
            {
                for (int jG = 0; jG < 9; jG += 3)
                {
                    if (CheckMiniZone(iG, jG))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CheckMiniZone(int mod3I, int mod3J)
        {
            var existing = new HashSet<int>();
            for (int i = mod3I; i < mod3I + 3; i++)
            {
                for (int j = mod3J; j < mod3J + 3; j++)
                {
                    var curr = Zones[i, j];
                    if (curr.Result.HasValue)
                    {
                        if (!existing.Add(curr.Result.Value))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool CheckVerticalError()
        {
            return CheckLinearError((i, j) => Zones[j, i]);
        }

        private bool CheckHorizontalError()
        {
            return CheckLinearError((i, j) => Zones[i, j]);
        }

        private bool CheckLinearError(Func<int, int, Zone> getZone)
        {
            for (int i = 0; i < 9; i++)
            {
                var existing = new HashSet<int>();
                for (int j = 0; j < 9; j++)
                {
                    var curr = getZone(i, j);
                    if (curr.Result.HasValue)
                    {
                        if (!existing.Add(curr.Result.Value))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        #endregion

        public Grid Copy()
        {
            var newGrid = new Grid { Zones = new Zone[9, 9] };

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    newGrid.Zones[i, j] = Zones[i, j].Copy();
                }
            }
            return newGrid;
        }

        public Zone GetFirstNonEmptyZone()
        {
            return AllZones.FirstOrDefault(x => !x.Result.HasValue);
        }

        private IEnumerable<Zone> AllZones
        {
            get
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        yield return Zones[i, j];
                    }
                }
            }
        }
    }
}