using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace HexEngine
{
    /// <summary>
    ///     Cellular automatic hexagonal state grid.
    ///     Creates a grid shaped like:
    ///           0 0 0 0 0 0
    ///          0 0 0 0 0 0
    ///         0 0 0 0 0 0
    ///     where x = col & y = row, increasing to the right and upwards, resp.
    /// </summary>
    public class HexWorld
    {
        HexGridCellContainer[,] _currentCells, _nextCells;
        List<Faction> _factions;

        #region initialization

        /// <summary>
        ///     Convenience constructor for a hex-rectangle environment.
        ///     More complex, arbitrary patterns/shapes possible, but indexing by row and column still works because it's a (hex) grid.
        ///     When that happens, this becomes RectangularHexWorld, and appropriate methods become part of parent class.
        /// </summary>
        public HexWorld(int rows, int cols)
        {
            RowCount = rows;
            ColCount = cols;

            _currentCells = new HexGridCellContainer[rows, cols];
            _nextCells = new HexGridCellContainer[rows, cols];

            // initialize cell containers
            ForEachPosition((row, col) =>
            {
                _currentCells[row, col] = new HexGridCellContainer(row, col);
                _nextCells[row, col] = new HexGridCellContainer(row, col);
            });

            // add neighbors to each container
            ForEachCell((current, next, coord) =>
            {
                IEnumerable<Coord> neighborPositions = ComputeNeighborPositions(coord.Row, coord.Col);

                current.AddNeighbors(neighborPositions.Select((nbrPos) => GetCurrentCell(nbrPos)));
                next.AddNeighbors(neighborPositions.Select((nbrPos) => GetNextCell(nbrPos)));
            });

            _factions = new List<Faction>();
        }

        #endregion

        #region properties

        public int RowCount { get; private set; }
        public int ColCount { get; private set; }

        #endregion

        #region methods

        public void AddCreatureCells(Faction faction, Coord position, int density)
        {
            _currentCells[position.Row, position.Col].AddCreatureCells(faction, density);
        }

        public void AddMoveHormone(Faction faction, Coord position, int density)
        {
            _currentCells[position.Row, position.Col].AddMoveHormone(faction, density);
        }

        public void Evolve()
        {
            ForEachCell((current, next) => next.MutateToNextGen(current));

            var tmp = _currentCells;
            _currentCells = _nextCells;
            _nextCells = tmp;
        }

        public void AddFaction(Faction newFaction)
        {
            _factions.Add(newFaction);

            ForEachCell((current, next) =>
            {
                current.AddFaction(newFaction);
                next.AddFaction(newFaction);
            });
        }

        private HexGridCellContainer GetCurrentCell(Coord position) { return _currentCells[position.Row, position.Col]; }
        private HexGridCellContainer GetNextCell(Coord position) { return _nextCells[position.Row, position.Col]; }

        #endregion

        #region iterators and helpers

        public void ForEachCell(Action<HexGridCellContainer, HexGridCellContainer, Coord> operation)
        {
            for (int row = 0; row < RowCount; row++)
                for (int col = 0; col < ColCount; col++)
                    operation(_currentCells[row, col], _nextCells[row, col], new Coord(row, col));
        }

        public void ForEachCell(Action<HexGridCellContainer, HexGridCellContainer> operation)
        {
            for (int row = 0; row < RowCount; row++)
                for (int col = 0; col < ColCount; col++)
                    operation(_currentCells[row, col], _nextCells[row, col]);
        }

        public void ForEachPosition(Action<int, int> operation)
        {
            for (int row = 0; row < RowCount; row++)
                for (int col = 0; col < ColCount; col++)
                    operation(row, col);
        }

        public IEnumerable<HexGridCellContainer> CurrentCells
        {
            get
            {
                for (int row = 0; row < RowCount; row++)
                    for (int col = 0; col < ColCount; col++)
                    {
                        yield return _currentCells[row, col];
                    }
            }
        }

        public IEnumerable<Coord> ComputeNeighborPositions(int row, int col)
        {
            // this shit is a little wierd.  example, coords are row, col:
            //
            //            (2,0) (2,1) (2,2) (2,3)
            //         (1,0) (1,1) (1,2) (1,3)
            //      (0,0) (0,1) (0,2) (0,3)
            //
            // cells classified by neighbor counts:
            //
            // -acute corner cells,     2 neighbors:    (0,0)
            //	                    			        (2,3)
            //
            // -obtuse corner cells,    3 neighbors:    (0,3)
            //					                        (2,0)
            //
            // -edge cells,		        4 neighbors:    (0,1)
            //					                        (0,2)
            //				                        	(1,0)
            //			                        		(1,3)
            //			                        		(2,1)
            //			                        		(2,2)
            //
            // -interior cells,	        6 neighbors:    (1,1)
            //				                        	(1,2)

            int maxRow = RowCount - 1;
            int maxCol = ColCount - 1;
            
            var allPossibleNbrs = new Coord[]
            {
                new Coord(row, col + 1),        // east
                new Coord(row + 1, col),        // north east
                new Coord(row + 1, col - 1),    // north west
                new Coord(row, col - 1),        // west
                new Coord(row - 1, col),        // south west
                new Coord(row - 1, col + 1),    // south east
            };

            return allPossibleNbrs.Where((coord) =>
                coord.Row >= 0 && coord.Row <= maxRow &&
                coord.Col >= 0 && coord.Col <= maxCol
                );
        }

        #endregion

    }

    public struct Coord
    {
        public Coord(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public int Row;
        public int Col;

        public override string ToString()
        {
            return $"(Row: {Row}, Col: {Col}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Coord)) return false;

            var o = (Coord)obj;

            return Row == o.Row && Col == o.Col;
        }

        public override int GetHashCode()
        {
            return Row.GetHashCode() ^ Col.GetHashCode();
        }
    }
}