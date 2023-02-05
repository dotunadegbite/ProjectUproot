using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TacticsToolkit
{
    public class MapManager : MonoBehaviour
    {
        private static MapManager _instance;
        public static MapManager Instance { get { return _instance; } }

        public OverlayTile overlayTilePrefab;
        public GameObject overlayContainer;
        public TileDataRuntimeSet tileTypeList;
        public Dictionary<Vector2Int, OverlayTile> map;
        private Dictionary<TileBase, TileData> dataFromTiles = new Dictionary<TileBase, TileData>();

        public Tilemap tilemap;

        private Entity activeCharacter;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }

            if (tileTypeList)
            {
                foreach (var tileData in tileTypeList.items)
                {
                    foreach (var item in tileData.baseTiles)
                    {
                        dataFromTiles.Add(item, tileData);
                    }
                }
            }

            tilemap = gameObject.GetComponentInChildren<Tilemap>();
            map = new Dictionary<Vector2Int, OverlayTile>();
            BoundsInt bounds = tilemap.cellBounds;

            //loop through the tilemap and create all the overlay tiles
            for (int z = bounds.max.z; z >= bounds.min.z; z--)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y++)
                {
                    for (int x = bounds.min.x; x < bounds.max.x; x++)
                    {
                        var tileLocation = new Vector3Int(x, y, z);
                        var tileKey = new Vector2Int(x, y);
                        if (tilemap.HasTile(tileLocation) && !map.ContainsKey(tileKey))
                        {
                            var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                            var cellWorldPosition = tilemap.GetCellCenterWorld(tileLocation);
                            var baseTile = tilemap.GetTile(tileLocation);
                            overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z + 1);
                            overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tilemap.GetComponent<TilemapRenderer>().sortingOrder;
                            overlayTile.gridLocation = tileLocation;

                            if (dataFromTiles.ContainsKey(baseTile)) {
                                overlayTile.tileData = dataFromTiles[baseTile];
                                if(dataFromTiles[baseTile].type == TileTypes.NonTraversable)
                                    overlayTile.isBlocked = true;
                            }

                            map.Add(tileKey, overlayTile);
                        }
                    }
                }
            }
        }

        public void SetActiveCharacter(GameObject activeCharacter)
        {
            this.activeCharacter = activeCharacter.GetComponent<Entity>();
        }

        //Get all tiles next to a tile
        public List<OverlayTile> GetNeighbourTiles(OverlayTile currentOverlayTile, List<OverlayTile> searchableTiles, bool ignoreObstacles = false, bool walkThroughAllies = true)
        {
            Dictionary<Vector2Int, OverlayTile> tileToSearch = new Dictionary<Vector2Int, OverlayTile>();

            if (searchableTiles.Count > 0)
            {
                foreach (var item in searchableTiles)
                {
                    tileToSearch.Add(item.grid2DLocation, item);
                }
            }
            else
            {
                tileToSearch = map;
            }

            List<OverlayTile> neighbours = new List<OverlayTile>();
            if (currentOverlayTile != null)
            {
                //top
                Vector2Int locationToCheck = new Vector2Int(
                    currentOverlayTile.gridLocation.x,
                    currentOverlayTile.gridLocation.y + 1
                    );

                ValidateNeighbour(currentOverlayTile, ignoreObstacles, walkThroughAllies, tileToSearch, neighbours, locationToCheck);

                //Bottom
                locationToCheck = new Vector2Int(
                    currentOverlayTile.gridLocation.x,
                    currentOverlayTile.gridLocation.y - 1
                    );


                ValidateNeighbour(currentOverlayTile, ignoreObstacles, walkThroughAllies, tileToSearch, neighbours, locationToCheck);

                //right
                locationToCheck = new Vector2Int(
                    currentOverlayTile.gridLocation.x + 1,
                    currentOverlayTile.gridLocation.y
                    );


                ValidateNeighbour(currentOverlayTile, ignoreObstacles, walkThroughAllies, tileToSearch, neighbours, locationToCheck);

                //left
                locationToCheck = new Vector2Int(
                    currentOverlayTile.gridLocation.x - 1,
                    currentOverlayTile.gridLocation.y
                    );


                ValidateNeighbour(currentOverlayTile, ignoreObstacles, walkThroughAllies, tileToSearch, neighbours, locationToCheck);
            }

            return neighbours;
        }

        //Check the neighbouring tile is valid.
        private static void ValidateNeighbour(OverlayTile currentOverlayTile, bool ignoreObstacles, bool walkThroughAllies, Dictionary<Vector2Int, OverlayTile> tilesToSearch, List<OverlayTile> neighbours, Vector2Int locationToCheck)
        {
            if (tilesToSearch.ContainsKey(locationToCheck) &&
                (ignoreObstacles ||
                (!ignoreObstacles && !tilesToSearch[locationToCheck].isBlocked) ||
                (!ignoreObstacles &&
                walkThroughAllies &&
                (tilesToSearch[locationToCheck].activeCharacter && Instance.activeCharacter && tilesToSearch[locationToCheck].activeCharacter.teamID == Instance.activeCharacter.teamID))))
            {
                if (Mathf.Abs(currentOverlayTile.gridLocation.z - tilesToSearch[locationToCheck].gridLocation.z) <= 1)
                    neighbours.Add(tilesToSearch[locationToCheck]);
            }
        }

        //Hide all overlayTiles currently being shown.
        public void ClearTiles()
        {
            foreach (var item in map.Values)
            {
                item.HideTile();
            }
        }

        //Get a tile by world position. 
        public OverlayTile GetOverlayByTransform(Vector3 position)
        {
            var gridLocation = tilemap.WorldToCell(position);
            if(map.ContainsKey(new Vector2Int(gridLocation.x, gridLocation.y)))
                return map[new Vector2Int(gridLocation.x, gridLocation.y)];

            return null;
        }

        //Get list of overlay tiles by grid positions. 
        public List<OverlayTile> GetOverlayTilesFromGridPositions(List<Vector2Int> positions)
        {
            List<OverlayTile> overlayTiles = new List<OverlayTile>();

            foreach (var item in positions)
            {
                overlayTiles.Add(map[item]);
            }

            return overlayTiles;
        }
    }
}
