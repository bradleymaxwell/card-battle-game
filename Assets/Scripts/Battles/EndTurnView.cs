using Units;
using UnityEngine;
using UnityEngine.UI;

namespace Battles
{
    public class EndTurnView : MonoBehaviour
    {
        [SerializeField] private Button button;
        private BattleService _battleService;
        
        private void Awake()
        {
            _battleService = Locator.Get<BattleService>();
        }
        
        private void OnEnable()
        {
            button.onClick.AddListener(OnClick);
        }
        
        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
        }
        
        private void OnClick()
        {
            if (_battleService.IsTurn(TeamType.Player))
            {
                _battleService.StartNextTurn();
            }
        }

        public void OnTurnChanged(IUnit unit)
        {
            button.gameObject.SetActive(unit.Team == TeamType.Player);
        }
    }
}