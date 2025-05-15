using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance
    {
        get { return _instance; }
    }
    
    [SerializeField] private GameObject gridOverlayPrefab;
    [SerializeField] private Tilemap groundTileMap;

    
    public Dictionary<Vector3Int, GridNode> Grids;
    
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
            _instance = this;

        Grids = new Dictionary<Vector3Int, GridNode>();
        SpawnOverlayInMap();

    }
    
    
    private void SpawnOverlayInMap()
    {
        BoundsInt bounds = groundTileMap.cellBounds;
        for (int i = bounds.xMin; i <= bounds.xMax; i++)
        {
            for (int j = bounds.yMin; j <= bounds.yMax; j++)
            {
                Vector3Int pos = new Vector3Int(i, j, 0);
                if (groundTileMap.HasTile(pos) && !Grids.ContainsKey(pos))
                {
                    GameObject newGridOverlay = Instantiate(gridOverlayPrefab, transform.Find("GridOverlay Container"));
                    newGridOverlay.name = "GridOverlay " + pos;
                    newGridOverlay.transform.localPosition = pos;
                    Grids.Add(pos, newGridOverlay.GetComponent<GridNode>());
                }
            }
        }
    }
    
    public List<GridNode> GetNeighbourTile(GridNode currentGridNode)
    {
        List<GridNode> neighbours = new List<GridNode>();
        Vector2Int[] directions = new[] { Vector2Int.down, Vector2Int.left, Vector2Int.right, Vector2Int.up };
        foreach (var dir in directions)
        {
            Vector2Int positionToCheck = dir + currentGridNode.GridLocation;
            if(Grids.ContainsKey((Vector3Int)positionToCheck))// && _grid[positionToCheck].GetComponent<GridNode>().occupyingObject == null)
                neighbours.Add(Grids[(Vector3Int)positionToCheck].GetComponent<GridNode>());
        }

        return neighbours;
    }
    
    public List<GridNode> GetSurroundingGridNode(GridNode currentGridNode, int moveRange)
    {
        HashSet<GridNode> visited = new HashSet<GridNode>();
        List<GridNode> currentStepNodes = new List<GridNode>{currentGridNode};

        visited.Add(currentGridNode);

        int stepCount = 0;
        while (stepCount < moveRange)
        {
            List<GridNode> nextStepTiles = new List<GridNode>();
            foreach (var tile in currentStepNodes)
            {
                foreach (var neighbour in GetNeighbourTile(tile))
                {
                    if (!visited.Contains(neighbour))
                    {
                        visited.Add(neighbour);
                        nextStepTiles.Add(neighbour);
                    }
                }
            }

            currentStepNodes = nextStepTiles;
            stepCount++;
        }

        //visited.Remove(currentGridNode);
        return visited.ToList();
    }
}
