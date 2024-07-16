using UnityEngine;
using UnityEngine.Tilemaps;

namespace Cities
{
    public class Pathfinder
    {
        private Tilemap roadsTilemap;

        private bool[,] roads;
    
        public Pathfinder(Tilemap roadsTilemap)
        {
            this.roadsTilemap = roadsTilemap;

            roads = GetRoads();

            string result = "Roads:\n";
            foreach (var road in roads)
            {
                foreach(var road2 in roads)
                {
                    result += road2 ? "*" : " ";
                }
                result += "\n";
            }

            Debug.Log(result);
        }

        public Vector2[] GetRoute(Vector3Int from, Vector3Int to)
        {
            return null;
        }

        private bool[,] GetRoads()
        {
            // Bounds of the tilemap
            var bounds = roadsTilemap.cellBounds;

            var result = new bool[bounds.xMax - bounds.xMin, bounds.yMax - bounds.yMin];

            for(int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for(int y = bounds.yMin; y < bounds.yMax; y++)
                    result[x - bounds.xMin, y - bounds.yMin] = roadsTilemap.GetTile(new Vector3Int(x, y, 0)) != null;
            }

            return result;
        }
    }
}
