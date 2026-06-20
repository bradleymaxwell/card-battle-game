using Battles;
using DefaultNamespace;
using Targeting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHoverTracker : MonoBehaviour
{
    [SerializeField] private LayerMask targetMask = ~0;
    private InputAction _clickAction;
    private Camera _camera;
    private InputAction _pointAction;
    private IHoverable _target;
    private SelectService _selectService;
    
    private void Awake()
    {
        _camera = Camera.main;
        var inputService = Locator.Get<InputService>();
        _pointAction = inputService.GetAction(PlayerInputConstants.UI, PlayerInputConstants.Point);
        _clickAction = inputService.GetAction(PlayerInputConstants.UI, PlayerInputConstants.Click);
        _selectService = Locator.Get<SelectService>();
    }
    
    private void Update()
    {
        UpdateTarget();
    }

    private void OnEnable()
    {
        _clickAction.performed += OnPlayerClick;
    }
    
    private void UpdateTarget()
    {
        if (_camera == null || _pointAction == null)
        {
            ClearTarget();
            return;
        }

        var pointerPosition = _pointAction.ReadValue<Vector2>();
        var ray = _camera.ScreenPointToRay(pointerPosition);

        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, targetMask))
        {
            ClearTarget();
            return;
        }

        var target = hit.collider.GetComponentInParent<IHoverable>();
        if (target == _target)
        {
            return;
        }
        
        _target?.OnUnhover();
        _target = target;
        _target?.OnHover();
    }

    private void ClearTarget()
    {
        if (_target == null)
        {
            return;
        }

        _target?.OnUnhover();
        _target = null;
    }

    private void OnPlayerClick(InputAction.CallbackContext context)
    {
        _target?.OnHoverClick();
    }
}
