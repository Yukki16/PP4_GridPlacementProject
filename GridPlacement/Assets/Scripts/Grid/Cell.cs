using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector2 gridPosition { get; private set; }

    public GameObject placedObject;

    //////More info of the cell to come;
    //Floor type
    //maybe even carpet.

    public void Initialize(Vector2 position)
    {
        gridPosition = position;
        name = $"Cell_x{gridPosition.x}_y{gridPosition.y}";
    }
}
