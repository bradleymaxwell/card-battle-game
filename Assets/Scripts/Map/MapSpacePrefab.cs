using Map;
using UnityEngine;

public class MapSpacePrefab : MonoBehaviour
{
    [SerializeField] private int q;
    public int Q => q;
    
    [SerializeField] private int r;
    public int R => r;
    
    private MapSpace Space { get; set; }
    
    public void Initialize(int q, int r)
    {
        this.q = q;
        this.r = r;
    }

    public void Bind(MapSpace space)
    {
        Space = space;
    }
}
