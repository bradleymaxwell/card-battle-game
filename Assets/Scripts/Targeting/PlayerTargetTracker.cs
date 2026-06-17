using Battles;
using DefaultNamespace;
using Targeting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTargetTracker : MonoBehaviour
{
    [SerializeField] private LayerMask targetMask = ~0;
    private TargetService _targetService;
    private Camera _camera;
    private InputAction _pointAction;
    private ITargetable _target;
    
    private void Awake()
    {
        _targetService = Locator.Get<TargetService>();
        _camera = Camera.main;
        var inputService = Locator.Get<InputService>();
        _pointAction = inputService.GetAction(PlayerInputConstants.UI, PlayerInputConstants.Point);
    }
    
    private void Update()
    {
        UpdateTarget();
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

        var target = hit.collider.GetComponentInParent<ITargetable>();
        if (target == _target)
        {
            return;
        }

        _target = target;
        _targetService.UpdateTarget(_target, TeamType.Player);
    }

    private void ClearTarget()
    {
        if (_target == null)
        {
            return;
        }

        _target = null;
        _targetService.UpdateTarget(null, TeamType.Player);
    }
}
