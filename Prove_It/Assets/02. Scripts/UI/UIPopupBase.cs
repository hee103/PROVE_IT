using UnityEngine;

public abstract class UIPopupBase : MonoBehaviour
{
    protected UIManager UI { get; private set; }

    [SerializeField] private CanvasGroup canvasGroup;

    public void Initialize(UIManager ui)
    {
        UI = ui;
        if(canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        SetInteractable(true);
    }

    public virtual void OnShown() { } //띄웠을 때 동작
    public virtual void OnBeforeClosed() { } //닫기전 동작

    public void Close()
    {
        UI.CloseTopPopup();
    }

    public void SetInteractable(bool interactable)
    {
        if (canvasGroup == null) return;
        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = interactable;
    }
}
