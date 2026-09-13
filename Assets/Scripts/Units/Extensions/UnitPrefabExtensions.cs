using UnityEngine;

public static class UnitPrefabExtensions
{
    public static void Face(this UnitPrefab unit, Component target)
    {
        var targetPosition = new Vector3(target.transform.position.x, unit.transform.position.y, target.transform.position.z);
        var moveDirection = targetPosition - unit.transform.position;
        moveDirection.y = 0f;
        if (moveDirection != Vector3.zero)
        {
            unit.transform.rotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        }
    }
}
