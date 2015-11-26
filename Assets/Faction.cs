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
        }

        public int ColonyID { get; private set; }
        public string Name { get; private set; }

        public int MaxMoveHormoneDensity { get; set; }
        public int MaxCreatureDensity { get; set; }
    } 
}