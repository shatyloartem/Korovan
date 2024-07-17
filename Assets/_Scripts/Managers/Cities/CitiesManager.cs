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

            // Compressing bounds for easier use
            citiesTilemap.CompressBounds();
            roadsTilemap.CompressBounds();

            _pathfinder = new Pathfinder(roadsTilemap);

            // Getting all cities positions in world coordinates
            citiesPositions = GetCitiesPositions();
        }

        private void Start()
        {
            var path = GetRoute(citiesPositions[0], citiesPositions[1]);

            // Debugging path to console
            // TODO: Remove after pathfinder class completed
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
        
        /// <summary>
        /// Used to get route from one point to another using roads
        /// </summary>
        /// <param name="startPosition">Starter position in world coordinates</param>
        /// <param name="targetPosition">Target position in world coordinates</param>
        /// <returns>Array of path points</returns>
        public Vector2[] GetRoute(Vector2 startPosition, Vector2 targetPosition)
        {
            var from = roadsTilemap.WorldToCell(startPosition);
            var to = roadsTilemap.WorldToCell(targetPosition);

            var cellsPath = _pathfinder.GetRoute(from, to);

            var resultPath = new Vector2[cellsPath.Length];

            for(int i = 0; i < cellsPath.Length; i++)
                resultPath[i] = roadsTilemap.CellToWorld((Vector3Int)cellsPath[i]);

            return resultPath;
        }

        #endregion
    }
}
