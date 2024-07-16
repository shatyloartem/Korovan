using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Cities
{
    public class CitiesManager : MonoBehaviour
    {
        [SerializeField] private Tilemap citiesTilemap;
        [SerializeField] private Tilemap roadsTilemap;

        public static CitiesManager Instance { get; private set; }

        private Pathfinder _pathfinder;

        private Vector3[] citiesPositions;

        private void Awake()
        {
            Instance = this;

            _pathfinder = new Pathfinder(roadsTilemap);

            citiesPositions = GetCitiesPositions();
        }

        #region Cities

        private Vector3[] GetCitiesPositions()
        {
            // Bounds of the Tilemap
            var bounds = citiesTilemap.cellBounds;

            var cityPositions = new List<Vector3>();

            // Iterate through all tiles in the bounds
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    var tilePosition = new Vector3Int(x, y, 0);
                    var tile = citiesTilemap.GetTile(tilePosition);

                    // Check if the tile is not empty
                    if (tile != null)
                    {
                        var worldPosition = citiesTilemap.CellToWorld(tilePosition);
                        cityPositions.Add(worldPosition);
                    }
                }
            }

            return cityPositions.ToArray();
        }
    
        public Vector3[] GetCities() => citiesPositions;

        #endregion

        #region Pathfinding
        
        public Vector2[] GetRoute(Vector2 startPosition, Vector2 targetPosition)
        {
            return _pathfinder.GetRoute(
                roadsTilemap.WorldToCell(startPosition),
                roadsTilemap.WorldToCell(targetPosition));
        }

        #endregion
    }
}
