using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Cities
{
    public class CitiesManager : MonoBehaviour
    {
        [SerializeField] private Grid grid;
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
    
        public Vector3[] GetTwoRandomCities()
        {
            if (citiesPositions.Length < 2)
            {
                Debug.LogError("Array must contain at least 2 elements.");
                return null;
            }

            // Select a random index for the first value
            int firstIndex = Random.Range(0, citiesPositions.Length);

            // Select a random index for the second value ensuring it is different from the first index
            int secondIndex;
            do
            {
                secondIndex = Random.Range(0, citiesPositions.Length);
            } while (secondIndex == firstIndex);

            // Return the two random distinct values
            return new Vector3[] { citiesPositions[firstIndex], citiesPositions[secondIndex] };
        }

        public Vector3[] GetCities() => citiesPositions;

        #endregion

        #region Pathfinding
        
        /// <summary>
        /// Used to get route from one point to another using roads
        /// </summary>
        /// <param name="startPosition">Starter position in world coordinates</param>
        /// <param name="targetPosition">Target position in world coordinates</param>
        /// <returns>Array of path points in world coordinates</returns>
        public Vector2[] GetRoute(Vector2 startPosition, Vector2 targetPosition)
        {
            var from = roadsTilemap.WorldToCell(startPosition);
            var to = roadsTilemap.WorldToCell(targetPosition);

            var cellsPath = _pathfinder.GetRoute(from, to);

            if(cellsPath == null)
                return null;

            var resultPath = new Vector2[cellsPath.Length];

            for(int i = 0; i < cellsPath.Length; i++)
            {
                var result = CellToWorld(cellsPath[i]);
                resultPath[i] = result;
                Debug.Log(result);
            }

            return resultPath;
        }

        private Vector2 CellToWorld(Vector2Int cell)
        {
            // Debug.Log(ConvertCellPointsToWorld(cell, grid, roadsTilemap));

            return ConvertCellPointsToWorld(cell, grid, roadsTilemap);
        }

        Vector3 ConvertCellPointsToWorld(Vector2Int cellPoints, Grid grid, Tilemap tilemap)
        {
            // Get the origin of the tilemap in world space
            Vector3 tilemapOrigin = tilemap.transform.position;

            // Get the cell size from the grid
            Vector3 cellSize = grid.cellSize;

            Vector3 tilemapStart = new Vector3(-3.5f, -8.5f);

            // Calculate the world position for each cell point
            Vector3Int cellPosition = new Vector3Int(cellPoints.x, cellPoints.y, 0);
            Vector3 worldPosition = tilemapOrigin + Vector3.Scale(cellPosition, cellSize);

            return worldPosition + tilemapStart;
        }

        #endregion
    }
}
