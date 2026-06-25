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
            _battleService.OnTurnChanged += OnTurnChanged;
        }
        
        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
            if (_battleService != null)
            {
                _battleService.OnTurnChanged -= OnTurnChanged;
            }
        }
        
        private void OnClick()
        {
            _battleService.EndTurn(TeamType.Player);
        }

        private void OnTurnChanged(TeamType turn)
        {
            button.gameObject.SetActive(turn == TeamType.Player);
        }
    }
}