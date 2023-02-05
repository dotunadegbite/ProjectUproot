using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TacticsToolkit
{
    //A tile type to be attached to the overlay tiles. Currently only using effect but there's lots of potential usages here. 
    [CreateAssetMenu(fileName = "TileData", menuName = "ScriptableObjects/TileData")]
    public class TileData : ScriptableObject
    {
        public List<TileBase> baseTiles;
        public string message;
        public TileTypes type = TileTypes.Traversable;
        public ScriptableEffect effect;
    }
}