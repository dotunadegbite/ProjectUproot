using System.Linq;
using UnityEngine;

namespace TacticsToolkit
{
    public class MouseController : MonoBehaviour
    {
        public CharacterSpawner characterSpawner;

        public GameEventGameObject focusOnNewTile;
        private OverlayTile focusedOnTile;


        // Update is called once per frame
        void Update()
        {
            var focusedTileHit = GetFocusedOnTile();

            if (focusedTileHit.HasValue)
            {
                OverlayTile overlayTile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();
                if (focusedOnTile != overlayTile)
                {
                    if (characterSpawner)
                    {
                        characterSpawner.focusedOnTile = overlayTile;
                    }
                    transform.position = overlayTile.transform.position;
                    gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;
                    focusedOnTile = overlayTile;

                    if (focusOnNewTile)
                        focusOnNewTile.Raise(overlayTile.gameObject);
                }
            }
        }

        //Returns the tile you are currently moused over
        public RaycastHit2D? GetFocusedOnTile()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);

            if (hits.Length > 0)
            {
                return hits.OrderByDescending(i => i.collider.transform.position.z).First();
            }
            return null;
        }
    }
}
