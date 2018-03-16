using System.Collections;
using UnityEngine;

public class NodeGrid : MonoBehaviour {
    public Vector2 gridSize;
    public float nodeSize;
    public LayerMask obstacles;

    Node[,] grid;
    
    void Start() {    
        GenerateGrid(Mathf.RoundToInt(gridSize.x / nodeSize),
                     Mathf.RoundToInt(gridSize.y / nodeSize));
    }

    void GenerateGrid(int width, int height) {
        grid = new Node[width, height];
        Vector3 gridGenStartingPoint = transform.position - Vector3.right * gridSize.x / 2 - Vector3.up * gridSize.y / 2;

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                Vector3 nodePosition = gridGenStartingPoint 
                                       + Vector3.right * (i * nodeSize + nodeSize * 0.5f) 
                                       + Vector3.up * (j * nodeSize + nodeSize * 0.5f);
                bool isThisNodeTraversable = !(Physics2D.OverlapCircle(nodePosition, nodeSize * 0.5f, obstacles));
                grid[i, j] = new Node(isThisNodeTraversable, nodePosition.x, nodePosition.y);
            }
        }
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.y, 10));

        if (grid != null) {
            foreach (Node n in grid) {
                Gizmos.color = (n.isTraversable) ? Color.white : Color.red;
                Gizmos.DrawCube(new Vector3(n.x, n.y, 0), Vector3.one * (nodeSize - 0.1f));
            }
        }
    }
}