using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace HexEngine
{
    /// <summary>
    ///     For a single (row, col) coordinate, holds ColonyCells for each Colony.
    /// </summary>
    public class WorldCell
    {
        private IList<WorldCell> _neighbors;
        private IDictionary<Colony, ColonyCell> _colonyCells;

        public WorldCell(int row, int col)
        {
            _neighbors = new List<WorldCell>();
            _colonyCells = new Dictionary<Colony, ColonyCell>();

            Row = row;
            Col = col;
        }

        public int Row { get; private set; }
        public int Col { get; private set; }

        public void AddNeighbor(WorldCell neighbor)
        {
            _neighbors.Add(neighbor);
        }

        public void AddNeighbors(IEnumerable<WorldCell> neighbors)
        {
            foreach (var neighbor in neighbors)
            {
                _neighbors.Add(neighbor);
            }
        }

        public void AddColony(Colony newColony)
        {
            _colonyCells.Add(newColony, new ColonyCell(newColony));
        }

        public IEnumerable<Colony> Colonies { get { return _colonyCells.Keys; } }

        public IEnumerable<ColonyCell> Cells { get { return _colonyCells.Values; } }

        public void AddCreatureCells(Colony colony, int density)
        {
            _colonyCells[colony].CreatureCellDensity += density;
        }

        public void AddMoveHormone(Colony colony, int density)
        {
            _colonyCells[colony].MoveHormoneDensity += density;
        }

        public int GetCreatureCellDensity(Colony colony) { return _colonyCells[colony].CreatureCellDensity; }

        public int GetMoveHormoneDensity(Colony colony) { return _colonyCells[colony].MoveHormoneDensity; }

        public void MutateToNextGen(WorldCell previousGenContainer)
        {
            // There are multiple ColonyCells per Colony in this location's WorldCell
            foreach (ColonyCell cell in _colonyCells.Values)
            {
                // dissipate movement hormone
                cell.MoveHormoneDensity = previousGenContainer.GetMoveHormoneDensity(cell.Colony) / 2;

                // TODO What in the fuck was I doing in JavaScript?
            }
        }
    }

    public class ColonyCell
    {
        public ColonyCell(Colony colony)
        {
            Colony = colony;
        }

        public Colony Colony { get; private set; }

        public int MoveHormoneDensity { get; set; }

        public int CreatureCellDensity { get; set; }
    } 
}