using UnityEngine;

namespace Units.MotherOfAllLoveBombs
{
    [CreateAssetMenu(menuName = "Game Config/Action/Mother of All Love Bombs")]
    public class MotherOfAllLoveBombsActionConfig : ActionConfig
    {
        [SerializeField] private int radius;
        public int Radius => radius;
        
        [SerializeField] private int damage;
        public int Damage => damage;
        
        public override IAction Action => new MotherOfAllLoveBombsAction(this);
    }
}