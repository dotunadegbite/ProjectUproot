using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TacticsToolkit
{
    public class CharacterSpawner : MonoBehaviour
    {
        public List<CharacterManager> characters;
        public List<SpawnTileContainer> spawnZones;
        public OverlayTile focusedOnTile;

        public GameEventGameObject setNewActiveCharacter;
        public GameEventGameObject spawnCharacter;

        public bool globalSpawn;

        private SpriteRenderer CharacterPreview;

        // Start is called before the first frame update
        void Start()
        {
            CharacterPreview = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            //If there is a list of characters to spawn, loop through and spawn them. 
            if (CheckIsTileOnSpawnTile() || (globalSpawn && characters.Count > 0))
            {
                CharacterPreview.sprite = characters[0].GetComponent<SpriteRenderer>().sprite;
                CharacterPreview.color = new Color(1, 1, 1, 0.75f);

                if (Input.GetMouseButtonDown(0))
                {
                    CharacterManager newCharacter = null;
                    if (characters.Count > 0)
                    {
                        newCharacter = Instantiate(characters[0]).GetComponent<CharacterManager>();
                        //newCharacter.SpawnCharacter();
                        if (spawnCharacter)
                            spawnCharacter.Raise(newCharacter.gameObject);

                        characters.RemoveAt(0);
                    }

                    if (characters.Count == 0)
                    {
                        foreach (var item in spawnZones)
                        {
                            item.gameObject.SetActive(false);
                        }

                        if (setNewActiveCharacter)
                            setNewActiveCharacter.Raise(newCharacter.gameObject);
                    }
                }
            }
            else
            {
                CharacterPreview.color = new Color(1, 1, 1, 0);
            }
        }

        //If using spawnZones check the tile is valid.
        private bool CheckIsTileOnSpawnTile()
        {
            if (spawnZones.Count == 0)
                return false;

            if (characters.Count > 0 && focusedOnTile && !focusedOnTile.isBlocked)
            {
                var nextChar = characters.First();
                var charactersSpawnZone = spawnZones.Where(x => x.TeamID == nextChar.teamID).ToList().First();

                foreach (var item in charactersSpawnZone.spawnTiles)
                {
                    if (focusedOnTile.grid2DLocation == item.grid2DLocation)
                        return true;
                }
            }

            return false;
        }
    }
}
