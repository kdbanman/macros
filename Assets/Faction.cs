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
            CreatureReboundRate = 0.2;
            CreatureMultiplicationRate = 0.005;
            HormoneEvaporationRate = 0.05;
            HormoneDissipationRate = 0.5;
            HormoneCreatureDepositionRatio = 0.3;
            HormoneTotemDepositionRate = 200;
        }

        public int ColonyID { get; private set; }
        public string Name { get; private set; }

        public int MaxMoveHormoneDensity { get; set; }
        public int MaxCreatureDensity { get; set; }

        public double CreatureMoveRate { get; set; }
        public double CreatureReboundRate { get; set; }
        public double CreatureMultiplicationRate { get; set; }

        public double HormoneEvaporationRate { get; set; }
        public double HormoneDissipationRate { get; set; }
        public double HormoneCreatureDepositionRatio { get; set; }

        public int HormoneTotemDepositionRate { get; set; }
    } 
}