using UnityEngine;
using UnityEngine.InputSystem;

public class GatherInput : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    private InputActionMap playerMap;
    private InputActionMap uiMap;
    
    [SerializeField] private InputActionReference moveActionRef;
    [SerializeField] private InputActionReference verticalActionRef;

    [HideInInspector]
    public float horizontalInput;
    [HideInInspector]
    public float verticalInput;

    private void OnEnable() {}

    private void OnDisable() {
        // playerMap.Disable();
    }

    private void Start()
    {
        playerMap = playerInput.actions.FindActionMap("Player");
        uiMap = playerInput.actions.FindActionMap("UI");
        playerMap.Enable();
    }

    private void Update()
    {
        horizontalInput = moveActionRef.action.ReadValue<float>();
        verticalInput = verticalActionRef.action.ReadValue<float>();
    }

    public void DisablePlayerMap()
    {
        playerMap.Disable();
    }
}
