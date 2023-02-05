using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TacticsToolkit
{
    //A* Pathfinding. 
    public class PathFinder
    {
        public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end, List<OverlayTile> searchableTiles, bool ignoreObstacles = false, bool walkTroughAllies = true)
        {
            List<OverlayTile> openList = new List<OverlayTile>();
            List<OverlayTile> closedList = new List<OverlayTile>();

            openList.Add(start);

            while (openList.Count > 0)
            {
                OverlayTile currentOverlayTile = openList.OrderBy(x => x.F).First();

                openList.Remove(currentOverlayTile);
                closedList.Add(currentOverlayTile);

                if (currentOverlayTile == end)
                {
                    //finalize our path. 
                    return GetFinishedList(start, end);
                }

                var neighbourTiles = MapManager.Instance.GetNeighbourTiles(currentOverlayTile, searchableTiles, ignoreObstacles, walkTroughAllies);

                foreach (var neighbour in neighbourTiles)
                {
                    //1 = jump height
                    if (closedList.Contains(neighbour))
                    {
                        continue;
                    }

                    neighbour.G = GetManhattenDistance(start, neighbour);
                    neighbour.H = GetManhattenDistance(end, neighbour);

                    neighbour.previous = currentOverlayTile;

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }

            return new List<OverlayTile>();
        }

        private List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end)
        {
            List<OverlayTile> finishedList = new List<OverlayTile>();

            OverlayTile currentTile = end;

            while (currentTile != start)
            {
                finishedList.Add(currentTile);
                currentTile = currentTile.previous;
            }

            finishedList.Reverse();

            return finishedList;
        }

        public int GetManhattenDistance(OverlayTile start, OverlayTile neighbour)
        {
            return Mathf.Abs(start.gridLocation.x - neighbour.gridLocation.x) + Mathf.Abs(start.gridLocation.y - neighbour.gridLocation.y);
        }
    }
}
