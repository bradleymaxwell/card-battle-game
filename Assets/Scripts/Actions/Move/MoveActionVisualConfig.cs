using Units.Actions.VisualPlayback;
using Units.VisualPlayback;
using UnityEngine;

namespace Units
{
    [CreateAssetMenu(menuName = "Game Config/Action/Visual/Move")]
    public class MoveActionVisualConfig : ActionVisualConfig
    {
        [SerializeField] private float speed;
        public float Speed => speed;
        
        public override IActionVisualHandler Handler => new MoveActionVisualHandler(this);
    }
}