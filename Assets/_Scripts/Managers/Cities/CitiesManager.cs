using System.Collections.Generic;
using Cities;
using SA.EasyConsole;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Managers.Cities
{
    public class CitiesManager : MonoBehaviour
    {
        [SerializeField] private Grid mainGrid;
        [SerializeField] private Tilemap citiesTileMap;
        [SerializeField] private Tilemap roadsTileMap;

        public static CitiesManager Instance { get; private set; }

        private Pathfinder _pathfinder;

        private Vector3[] _citiesPositions;

        private void Awake()
        {
            Instance = this;

            // Compressing bounds for easier use
            citiesTileMap.CompressBounds();
            roadsTileMap.CompressBounds();

            _pathfinder = new Pathfinder(roadsTileMap);

            // Getting all cities positions in world coordinates
            _citiesPositions = GetCitiesPositions();
        }

        #region Cities

        private Vector3[] GetCitiesPositions()
        {
            // Bounds of the TileMap
            var bounds = citiesTileMap.cellBounds;

            var cityPositions = new List<Vector3>();

            // Iterate through all tiles in the bounds
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    var tilePosition = new Vector3Int(x, y, 0);
                    var tile = citiesTileMap.GetTile(tilePosition);

                    // Check if the tile is not empty
                    if (tile != null)
                    {
                        var worldPosition = citiesTileMap.CellToWorld(tilePosition);
                        cityPositions.Add(worldPosition);
                    }
                }
            }

            return cityPositions.ToArray();
        }
    
        public Vector3[] GetTwoRandomCities()
        {
            if (_citiesPositions.Length < 2)
            {
                EasyConsole.LogError("Array must contain at least 2 elements.");
                return null;
            }

            // Select a random index for the first value
            int firstIndex = Random.Range(0, _citiesPositions.Length);

            // Select a random index for the second value ensuring it is different from the first index
            int secondIndex;
            do
            {
                secondIndex = Random.Range(0, _citiesPositions.Length);
            } while (secondIndex == firstIndex);

            // Return the two random distinct values
            return new[] { _citiesPositions[firstIndex], _citiesPositions[secondIndex] };
        }

        public Vector3[] GetCities() => _citiesPositions;

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
            var from = roadsTileMap.WorldToCell(startPosition);
            var to = roadsTileMap.WorldToCell(targetPosition);

            var cellsPath = _pathfinder.GetRoute(from, to);

            if(cellsPath == null)
                return null;

            var resultPath = new Vector2[cellsPath.Length];

            for(int i = 0; i < cellsPath.Length; i++)
            {
                var result = CellToWorld(cellsPath[i]);
                resultPath[i] = result;
            }

            return resultPath;
        }

        private Vector2 CellToWorld(Vector2Int cell)
        {
            // Debug.Log(ConvertCellPointsToWorld(cell, grid, roadsTileMap));

            return ConvertCellPointsToWorld(cell, mainGrid, roadsTileMap);
        }

        Vector3 ConvertCellPointsToWorld(Vector2Int cellPoints, Grid grid, Tilemap tileMap)
        {
            // Get the origin of the tileMap in world space
            Vector3 tileMapOrigin = tileMap.transform.position;

            // Get the cell size from the grid
            Vector3 cellSize = grid.cellSize;

            Vector3 tileMapStart = new Vector3(-3.5f, -8.5f);

            // Calculate the world position for each cell point
            Vector3Int cellPosition = new Vector3Int(cellPoints.x, cellPoints.y, 0);
            Vector3 worldPosition = tileMapOrigin + Vector3.Scale(cellPosition, cellSize);

            return worldPosition + tileMapStart;
        }

        #endregion
    }
}
