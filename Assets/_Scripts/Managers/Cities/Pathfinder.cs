using System.Collections.Generic;
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
        }

        public Vector2Int[] GetRoute(Vector3Int from, Vector3Int to)
        {
            var start = new Vector2Int(from.x - roadsTilemap.cellBounds.xMin, from.y - roadsTilemap.cellBounds.yMin);
            var finish = new Vector2Int(to.x - roadsTilemap.cellBounds.xMin, to.y - roadsTilemap.cellBounds.yMin);

            return AStarPathfinding.FindPath(roads, start, finish);
        }

        private bool[,] GetRoads()
        {
            // Bounds of the tilemap
            var bounds = roadsTilemap.cellBounds;

            var result = new bool[bounds.xMax - bounds.xMin, bounds.yMax - bounds.yMin];

            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                    result[x - bounds.xMin, y - bounds.yMin] = roadsTilemap.GetTile(new Vector3Int(x, y, 0)) != null;
            }

            return result;
        }
    }

    public class AStarPathfinding
    {
        private class Node
        {
            public Vector2Int Position;
            public float GCost;
            public float HCost;
            public float FCost => GCost + HCost;
            public Node? Parent;

            public Node(Vector2Int position, float gCost, float hCost, Node? parent)
            {
                Position = position;
                GCost = gCost;
                HCost = hCost;
                Parent = parent;
            }
        }

        public static Vector2Int[] FindPath(bool[,] grid, Vector2Int start, Vector2Int end)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);

            List<Vector2Int> GetNeighbors(Vector2Int position)
            {
                var neighbors = new List<Vector2Int>();
                var possibleMoves = new Vector2Int[]
                {
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 0),
                    new Vector2Int(0, -1),
                    new Vector2Int(-1, 0)
                };

                foreach (var move in possibleMoves)
                {
                    Vector2Int neighborPos = position + move;
                    if (neighborPos.x >= 0 && neighborPos.x < width && neighborPos.y >= 0 && neighborPos.y < height)
                    {
                        if (grid[neighborPos.x, neighborPos.y])
                        {
                            neighbors.Add(neighborPos);
                        }
                    }
                }

                return neighbors;
            }

            float GetDistance(Vector2Int a, Vector2Int b) => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

            var openList = new Dictionary<Vector2Int, Node>();
            var closedList = new HashSet<Vector2Int>();

            Node startNode = new Node(start, 0, GetDistance(start, end), null);
            openList[start] = startNode;

            while (openList.Count > 0)
            {
                Node currentNode = GetLowestFCostNode(openList);

                if (currentNode.Position == end)
                {
                    return RetracePath(currentNode).ToArray();
                }

                openList.Remove(currentNode.Position);
                closedList.Add(currentNode.Position);

                foreach (var neighborPos in GetNeighbors(currentNode.Position))
                {
                    if (closedList.Contains(neighborPos))
                        continue;

                    float tentativeGCost = currentNode.GCost + GetDistance(currentNode.Position, neighborPos);
                    if (!openList.ContainsKey(neighborPos) || tentativeGCost < openList[neighborPos].GCost)
                    {
                        Node neighborNode = new Node(neighborPos, tentativeGCost, GetDistance(neighborPos, end), currentNode);
                        openList[neighborPos] = neighborNode;
                    }
                }
            }

            return null; // Path not found
        }

        private static Node GetLowestFCostNode(Dictionary<Vector2Int, Node> openList)
        {
            Node lowestFCostNode = default;
            float lowestFCost = float.MaxValue;

            foreach (var kvp in openList)
            {
                if (kvp.Value.FCost < lowestFCost)
                {
                    lowestFCost = kvp.Value.FCost;
                    lowestFCostNode = kvp.Value;
                }
            }

            return lowestFCostNode;
        }

        private static List<Vector2Int> RetracePath(Node endNode)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            Node currentNode = endNode;

            while (currentNode != null)
            {
                path.Add(currentNode.Position);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path;
        }
    }
}
