using TMPro;
using UnityEngine;

namespace SA.EasyConsole
{
    public class ConsoleUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI logText;
        [SerializeField] private RectTransform logPanel;

        private void Awake()
        {
            EasyConsole.OnLogged += UpdateLogPanel;
            
            UpdateLogPanel();
        }

        private void UpdateLogPanel()
        {
            logPanel.sizeDelta = new Vector2(
                logPanel.sizeDelta.x,
                logText.preferredHeight);
        }
    }
}