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

        private void Start()
        {
            var path = GetRoute(citiesPositions[0], citiesPositions[1]);

            string res = "";
            foreach (var point in path)
                res += point + "\n";

            Debug.Log(res);
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
            var from = roadsTilemap.WorldToCell(startPosition);
            var to = roadsTilemap.WorldToCell(targetPosition);

            var cellsPath = _pathfinder.GetRoute(
                from,
                to);

            var resultPath = new Vector2[cellsPath.Length];

            for(int i = 0; i < cellsPath.Length; i++)
                resultPath[i] = roadsTilemap.CellToWorld((Vector3Int)cellsPath[i]);

            return resultPath;
        }

        #endregion
    }
}
