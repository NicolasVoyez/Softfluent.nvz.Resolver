using System;

namespace Softfluent.nvz.Resolver
{
    public class Solver
    {
        public Grid Grid { get; private set; }

        private bool _solved = false;

        public Solver(Grid grid)
        {
            Grid = grid;
        }

        public void Solve()
        {
            BasicSolve(Grid);
            while (!Grid.IsCompleted && !Grid.IsDead)
            {
                TryMultipleValues(Grid);
            }
        }

        private void TryMultipleValues(Grid grid)
        {
            var newGrid = grid.Copy();
            Zone oldTriedZone = null;
            while (true)
            {
                var triedZone = newGrid.GetFirstNonEmptyZone();
                if (!triedZone.ForceResult())
                {
                    if (oldTriedZone != null)
                    {
                        grid.Zones[oldTriedZone.X, oldTriedZone.Y].Possibilities.Remove(oldTriedZone.Result.Value);
                    }
                    return;
                }
                oldTriedZone = triedZone;

                if (!BasicSolve(newGrid, true))
                {
                    grid.Zones[triedZone.X, triedZone.Y].Possibilities.Remove(triedZone.Result.Value);
                    return;
                }
                if (newGrid.IsCompleted)
                {
                    Grid = newGrid;
                    _solved = true;
                    return;
                }
                TryMultipleValues(newGrid);
            }
        }

        private bool BasicSolve(Grid grid, bool checkErrors = false)
        {
            bool hasAnyChange;
            do
            {
                hasAnyChange = grid.CleanHorizontal() | grid.CleanVertical() | grid.Clean3X3();
                OnBasicSolveStepPassed?.Invoke(grid, EventArgs.Empty);
                if (checkErrors && grid.HasError)
                {
                    return false;
                }
            } while (hasAnyChange);
            return true;
        }

        public event EventHandler OnBasicSolveStepPassed;
    }
}
