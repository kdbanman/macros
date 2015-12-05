using UnityEngine;
using System.Collections;
using System;

namespace HexEngine
{
    public class Colony
    {
        public Colony(int id, string name)
        {
            ColonyID = id;
            Name = name;

            MaxMoveHormoneDensity = 1000;
            MaxCreatureDensity = 3000;

            CreatureMoveRate = 2.0;
            CreatureReboundRatio = 0.2;
            CreatureMultiplicationRate = 0.005;

            HormoneEvaporationRatio = 0.05;
            HormoneDissipationRatio = 0.5;
            HormoneLeavingCreatureDepositionRatio = 0.3;
            HormoneArrivingCreatureDepositionRatio = 0.2;
        
            HormoneTotemDepositionRate = 200;
        }

        public int ColonyID { get; private set; }
        public string Name { get; private set; }

        public int MaxMoveHormoneDensity { get; set; }
        public int MaxCreatureDensity { get; set; }

        public double CreatureMoveRate { get; set; }
        public double CreatureReboundRatio { get; set; }
        public double CreatureMultiplicationRate { get; set; }

        public double HormoneEvaporationRatio { get; set; }
        public double HormoneDissipationRatio { get; set; }
        public double HormoneLeavingCreatureDepositionRatio { get; set; }
        public double HormoneArrivingCreatureDepositionRatio { get; set; }

        public Coord HormoneTotemPosition { get; set; }
        public int HormoneTotemDepositionRate { get; set; }

        public override string ToString()
        {
            return "Colony " + ColonyID + ": " + Name;
        }
    } 
}