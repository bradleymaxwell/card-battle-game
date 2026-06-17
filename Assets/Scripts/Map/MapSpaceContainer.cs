using System;
using System.Collections.Generic;
using Map;
using UnityEngine;

public class MapSpaceContainer : MonoBehaviour
{
    [SerializeField] private MapSpacePrefab mapSpacePrefab;
    [SerializeField] private Vector3 anchorPosition;
    [SerializeField] private float padding;
    [SerializeField] private float prefabSize;
    [SerializeField] private MapDirection previewDirection;
    [SerializeField] private MapSpacePrefab referenceMapSpace;

    public MapDirection PreviewDirection
    {
        get => previewDirection;
        set => previewDirection = value;
    }

    public MapSpacePrefab ReferenceMapSpace
    {
        get => referenceMapSpace;
        set => referenceMapSpace = value;
    }

    public MapSpacePrefab CreateAnchorSpace()
    {
        return CreateSpace(0, 0);
    }

    public MapSpacePrefab CreateSpaceFrom(MapSpacePrefab reference, MapDirection direction)
    {
        if (reference == null)
        {
            Debug.LogWarning("Cannot create space because the reference space is missing or has not been initialized.", this);
            return null;
        }

        var (offsetQ, offsetR) = GetOffset(direction);
        return CreateSpace(reference.Q + offsetQ, reference.R + offsetR);
    }

    public bool CanCreateSpaceFrom(MapSpacePrefab reference, MapDirection direction)
    {
        if (reference == null)
        {
            return false;
        }

        var (offsetQ, offsetR) = GetOffset(direction);
        var q = reference.Q + offsetQ;
        var r = reference.R + offsetR;

        return !ContainsSpace(q, r);
    }

    public bool ContainsSpace(int q, int r)
    {
        return TryGetSpace(q, r, out _);
    }

    public bool TryGetSpace(int q, int r, out MapSpacePrefab foundMapSpace)
    {
        foreach (Transform child in transform)
        {
            if (!child.TryGetComponent(out MapSpacePrefab space))
            {
                continue;
            }

            if (space.Q == q && space.R == r)
            {
                foundMapSpace = space;
                return true;
            }
        }

        foundMapSpace = null;
        return false;
    }

    public Vector3 GetPreviewWorldPosition(MapSpacePrefab reference, MapDirection direction)
    {
        if (reference == null)
        {
            return anchorPosition;
        }

        var (offsetQ, offsetR) = GetOffset(direction);
        var q = reference.Q + offsetQ;
        var r = reference.R + offsetR;

        return GetWorldPosition(q, r);
    }
    
    public IReadOnlyList<MapSpacePrefab> GetMapSpacePrefabs()
    {
        var mapSpacePrefabs = new List<MapSpacePrefab>();

        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out MapSpacePrefab mapSpacePrefab))
            {
                mapSpacePrefabs.Add(mapSpacePrefab);
            }
        }

        return mapSpacePrefabs;
    }

    private MapSpacePrefab CreateSpace(int q, int r)
    {
        if (mapSpacePrefab == null)
        {
            Debug.LogWarning("Cannot create space because no space prefab has been assigned.", this);
            return null;
        }

        if (ContainsSpace(q, r))
        {
            Debug.LogWarning($"Cannot create space at q={q}, r={r} because one already exists.", this);
            return null;
        }

        var space = Instantiate(mapSpacePrefab, transform);
        space.Initialize(q, r);
        space.transform.position = GetWorldPosition(q, r);
        space.name = $"Space ({q}, {r})";

        return space;
    }

    public Vector3 GetWorldPosition(int q, int r)
    {
        var xSpacing = Mathf.Sqrt(3f) * prefabSize + padding;
        var zSpacing = 1.5f * prefabSize + padding;

        var x = xSpacing * (q + r * 0.5f);
        var z = zSpacing * r;

        return anchorPosition + new Vector3(x, 0f, z);
    }

    public void RegenerateSpacePositions()
    {
        foreach (Transform child in transform)
        {
            if (!child.TryGetComponent(out MapSpacePrefab space))
            {
                continue;
            }

            space.transform.position = GetWorldPosition(space.Q, space.R);
            space.name = $"Space ({space.Q}, {space.R})";
        }
    }
    
    private (int q, int r) GetOffset(MapDirection direction)
    {
        switch (direction)
        {
            case MapDirection.Left:
                return (-1, 0);
            case MapDirection.Right:
                return (1, 0);
            case MapDirection.LeftUp:
                return (-1, 1);
            case MapDirection.RightUp:
                return (0, 1);
            case MapDirection.LeftDown:
                return (0, -1);
            case MapDirection.RightDown:
                return (1, -1);
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(anchorPosition, Mathf.Max(0.1f, prefabSize * 0.15f));

        if (referenceMapSpace == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(referenceMapSpace.transform.position, Mathf.Max(0.2f, prefabSize * 0.35f));

        var previewPosition = GetPreviewWorldPosition(referenceMapSpace, previewDirection);
        var canCreate = CanCreateSpaceFrom(referenceMapSpace, previewDirection);

        Gizmos.color = canCreate ? Color.green : Color.red;
        Gizmos.DrawWireSphere(previewPosition, Mathf.Max(0.25f, prefabSize * 0.45f));
        Gizmos.DrawLine(referenceMapSpace.transform.position, previewPosition);
    }
}
