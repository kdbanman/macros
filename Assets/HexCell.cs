using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace HexEngine
{
    /// <summary>
    ///     For a single (row, col) coordinate, holds ColonyCells for each Colony.
    /// </summary>
    /// <remarks>
    ///     It feels a bit smelly that the HexWorld, WorldCell, *and* ColonyCells all have fields
    ///     that depend on Colony.
    /// </remarks>
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
            foreach (var neighbor in neighbors) {
                _neighbors.Add(neighbor);
            }
        }

        public void AddColony(Colony newColony)
        {
            _colonyCells.Add(newColony, new ColonyCell(newColony));
        }

        public IEnumerable<ColonyCell> ColonyCells { get { return _colonyCells.Values; } }

        public Coord Coord { get { return new Coord(Row, Col); } }
        
        public void AddCreatures(Colony colony, int density)
        {
            _colonyCells[colony].CreatureDensity += density;
        }

        public void AddMoveHormone(Colony colony, int density)
        {
            _colonyCells[colony].MoveHormoneDensity += density;
        }

        public int GetCreatureDensity(Colony colony) { return _colonyCells[colony].CreatureDensity; }

        public int GetMoveHormoneDensity(Colony colony) { return _colonyCells[colony].MoveHormoneDensity; }

        public void MutateToNextGen(WorldCell previousGenWorldCell)
        {
            foreach (ColonyCell cell in _colonyCells.Values) {
                cell.MoveHormoneDensity = ComputeNewHormoneDensity(previousGenWorldCell, cell);
                cell.CreatureDensity = ComputeNewCreatureDensity(previousGenWorldCell, cell);
            }
        }

        private int ComputeNewHormoneDensity(WorldCell previousGenWorldCell, ColonyCell cell)
        {
            var previousHormoneDensity = previousGenWorldCell.GetMoveHormoneDensity(cell.Colony);
            // evaporate movement hormone from cell
            double evaporated = (double)previousHormoneDensity * cell.Colony.HormoneEvaporationRate;
            // remove movement hormone that will be taken by neighboring cells
            double dissipated = (double)previousHormoneDensity * cell.Colony.HormoneDissipationRate;
            // add movement hormone dissipated from neighboring cells of the same colony
            double collected = 0.0;
            foreach (var neighborCell in previousGenWorldCell._neighbors) {
                double neighborDissipation = neighborCell.GetMoveHormoneDensity(cell.Colony) * cell.Colony.HormoneDissipationRate;
                double collectedFromNeighbor = neighborDissipation / neighborCell._neighbors.Count;
                collected += collectedFromNeighbor;
            }
            // add movement hormone deposited by moving cells
            double deposited = previousGenWorldCell.ComputeCreatureDensityMovementLoss(cell.Colony) * cell.Colony.HormoneCreatureDepositionRatio / cell.Colony.CreatureMoveRate;

            var newHormoneDensity = (int)(previousHormoneDensity - evaporated - dissipated + collected + deposited);
            return newHormoneDensity;
        }

        private int ComputeNewCreatureDensity(WorldCell previousGenWorldCell, ColonyCell cell)
        {
            double previousCreatureDensity = previousGenWorldCell.GetCreatureDensity(cell.Colony);

            // trans movement
            double movementCreatureLoss = previousGenWorldCell.ComputeCreatureDensityMovementLoss(cell.Colony);
            double movementCreatureGain = previousGenWorldCell.ComputeCreatureDensityMovementGain(cell.Colony);

            // overpressure rebound movement
            double reboundCreatureLoss = previousGenWorldCell.ComputeCreatureDensityReboundLoss(cell.Colony);
            double reboundCreatureGain = previousGenWorldCell.ComputeCreatureDensityReboundGain(cell.Colony);

            double creatureMultiplicationGain = previousGenWorldCell.ComputeCreatureDensityMultiplicationGain(cell.Colony);

            return (int)(
                previousCreatureDensity - 
                movementCreatureLoss - 
                reboundCreatureLoss + 
                movementCreatureGain + 
                reboundCreatureGain +
                creatureMultiplicationGain);
        }

        private double ComputeCreatureDensityMultiplicationGain(Colony colony)
        {
            return GetCreatureDensity(colony) * colony.CreatureMultiplicationRate;
        }

        private double ComputeCreatureDensityReboundGain(Colony colony)
        {
            double collected = 0.0;
            foreach(var neighborCell in _neighbors) {
                double neighborRebound = neighborCell.ComputeCreatureDensityReboundLoss(colony);
                double collectedFromNeighbor = neighborRebound / neighborCell._neighbors.Count;

                collected += collectedFromNeighbor;
            }

            return collected;
        }

        private double ComputeCreatureDensityReboundLoss(Colony colony)
        {
            return GetCreatureDensity(colony) * colony.CreatureReboundRate;
        }

        public double ComputeCreatureDensityMovementLoss(Colony colony)
        {
            double currentCreatureDensity = GetCreatureDensity(colony);
            double maxCreatureDensityLoss = (int)(ComputeOutwardHormoneGraident(colony) * colony.CreatureMoveRate);
            return Math.Min(currentCreatureDensity, maxCreatureDensityLoss);
        }

        private int ComputeOutwardHormoneGraident(Colony colony)
        {
            int selfHormone = GetMoveHormoneDensity(colony);
            int outwardGradient = 0;
            foreach (var neighbor in _neighbors) {
                int neighborHormone = neighbor.GetMoveHormoneDensity(colony);
                if (neighborHormone > selfHormone)
                    outwardGradient += neighborHormone - selfHormone;
            }
            return outwardGradient;
        }

        private double ComputeCreatureDensityMovementGain(Colony colony)
        {
            double creatureDensityGain = 0.0;

            double selfHormone = GetMoveHormoneDensity(colony);
            foreach (var neighbor in _neighbors) {
                double neighborHormone = neighbor.GetMoveHormoneDensity(colony);
                if (selfHormone > neighborHormone) {
                    double inwardGradient = selfHormone - neighborHormone;
                    double neighborOutwardGradient = neighbor.ComputeOutwardHormoneGraident(colony);
                    double neighborCreatureLoss = neighbor.ComputeCreatureDensityMovementLoss(colony);

                    creatureDensityGain += inwardGradient * neighborCreatureLoss / neighborOutwardGradient;
                }
            }

            return creatureDensityGain;
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

        public int CreatureDensity { get; set; }
    }
}