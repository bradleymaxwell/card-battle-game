using UnityEngine;

public static class UnitPrefabExtensions
{
    public static void Face(this UnitPrefab unit, Component target)
    {
        unit.transform.Face(target);
    }
    
    public static void Face(this Transform transform, Component target)
    {
        var targetPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        var moveDirection = targetPosition - transform.position;
        moveDirection.y = 0f;
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        }
    }
}
