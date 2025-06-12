using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance;

    public float cellSize = 1f;

    Dictionary<Vector2, Cell> grid = new Dictionary<Vector2, Cell>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
