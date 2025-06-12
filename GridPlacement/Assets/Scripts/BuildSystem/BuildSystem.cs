using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class BuildSystem : MonoBehaviour, InputSystem_Actions.IGridInteractionActions
{
    public static BuildSystem Instance { get; private set; }

    public InputActionAsset input;

    public bool buildingFloor;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public void OnBuilding(InputAction.CallbackContext context)
    {
        if (!buildingFloor)
        {
            return;
        }

        Vector2 initialPosition = new Vector2();
        if(context.started)
        {
            initialPosition = Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, Camera.main.nearClipPlane));
        }

        while(context.interaction is HoldInteraction)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, Camera.main.nearClipPlane));
            Debug.Log("building");
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
}
