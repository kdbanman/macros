using UnityEngine;
using System.Collections;


namespace HexEngine
{
    public class Colony
    {
        public Colony(int id, string name)
        {
            ColonyID = id;
            Name = name;

            MaxMoveHormoneDensity = 100;
            MaxCreatureDensity = 100;
            HormoneEvaporationRate = 0.25;
            HormoneDissipationRate = 0.5;
        }

        public int ColonyID { get; private set; }
        public string Name { get; private set; }

        public int MaxMoveHormoneDensity { get; set; }
        public int MaxCreatureDensity { get; set; }
        public double HormoneEvaporationRate { get; set; }
        public double HormoneDissipationRate { get; set; }
    } 
}