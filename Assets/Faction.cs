using UnityEngine;
using System.Collections;


namespace HexEngine
{
    public class Faction
    {
        public Faction(int id, string name)
        {
            FactionID = id;
            Name = name;
        }

        public int FactionID { get; private set; }
        public string Name { get; private set; }
    } 
}