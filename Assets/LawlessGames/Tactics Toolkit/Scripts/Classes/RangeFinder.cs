using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TacticsToolkit
{
    public class RangeFinder
    {
        //Gets all tiles within a range
        public List<OverlayTile> GetTilesInRange(OverlayTile startingTile, int range, bool ignoreObstacles = false, bool walkThroughAllies = true)
        {
            var inRangeTiles = new List<OverlayTile>();
            int stepCount = 0;

            inRangeTiles.Add(startingTile);

            var tileForPreviousStep = new List<OverlayTile>();
            tileForPreviousStep.Add(startingTile);

            while (stepCount < range)
            {
                var surroundingTiles = new List<OverlayTile>();

                foreach (var item in tileForPreviousStep)
                {
                    surroundingTiles.AddRange(MapManager.Instance.GetNeighbourTiles(item, new List<OverlayTile>(), ignoreObstacles, walkThroughAllies));
                }

                inRangeTiles.AddRange(surroundingTiles);
                tileForPreviousStep = surroundingTiles;
                stepCount++;
            }

            return inRangeTiles.Distinct().ToList();
        }
    }
}
