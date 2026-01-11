using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchingPopup : UIPopupBase
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button cancelButton;

    private System.Action _onCancle;

    public void Setup(string msg, System.Action onCancle)
    {
        if(messageText) messageText.text = msg;
        _onCancle = onCancle;

        if (cancelButton)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() => _onCancle?.Invoke());
        }
    }
}
