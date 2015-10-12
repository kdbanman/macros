using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace HexEngine
{
    /// <summary>
    ///     For a single (row, col) coordinate, holds FactionCells for each faction.
    /// </summary>
    public class HexGridCellContainer
    {
        private IList<HexGridCellContainer> _neighbors;
        private IDictionary<Faction, FactionCell> _factionCells;

        public HexGridCellContainer(int row, int col)
        {
            _neighbors = new List<HexGridCellContainer>();
            _factionCells = new Dictionary<Faction, FactionCell>();

            Row = row;
            Col = col;
        }

        public int Row { get; private set; }
        public int Col { get; private set; }

        public void AddNeighbor(HexGridCellContainer neighbor)
        {
            _neighbors.Add(neighbor);
        }

        public void AddNeighbors(IEnumerable<HexGridCellContainer> neighbors)
        {
            foreach (var neighbor in neighbors)
            {
                _neighbors.Add(neighbor);
            }
        }

        public void AddFaction(Faction newFaction)
        {
            _factionCells.Add(newFaction, new FactionCell(newFaction));
        }

        public IEnumerable<Faction> Factions { get { return _factionCells.Keys; } }

        public IEnumerable<FactionCell> Cells { get { return _factionCells.Values; } }

        public void AddCreatureCells(Faction faction, int density)
        {
            _factionCells[faction].CreatureCellDensity += density;
        }

        public void AddMoveHormone(Faction faction, int density)
        {
            _factionCells[faction].MoveHormoneDensity += density;
        }

        public int GetCreatureCellDensity(Faction faction) { return _factionCells[faction].CreatureCellDensity; }

        public int GetMoveHormoneDensity(Faction faction) { return _factionCells[faction].MoveHormoneDensity; }

        public void MutateToNextGen(HexGridCellContainer previousGenContainer)
        {
            // There are multiple FactionCells per Faction in this location's HexGridCellContainer
            foreach (FactionCell cell in _factionCells.Values)
            {
                // dissipate movement hormone
                cell.MoveHormoneDensity = previousGenContainer.GetMoveHormoneDensity(cell.Faction) / 2;

                // TODO What in the fuck was I doing in JavaScript?
            }
        }
    }

    public class FactionCell
    {
        public FactionCell(Faction faction)
        {
            Faction = faction;
        }

        public Faction Faction { get; private set; }

        public int MoveHormoneDensity { get; set; }

        public int CreatureCellDensity { get; set; }
    } 
}