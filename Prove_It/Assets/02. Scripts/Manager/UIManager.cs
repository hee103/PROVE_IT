using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance {  get; private set; }

    [Header("Main Canvas")]
    [SerializeField] private Transform mainCanvasRoot;

    [Header("Yarn Canvas")]
    [SerializeField] private CanvasGroup yarnCanvasGroup;
    [SerializeField] private GameObject yarnCanvasRoot;

    [Header("Event System")]
    [SerializeField] private EventSystem eventSystem;

    [Header("PopUp")]
    [SerializeField] private CanvasGroup popupDim;

    private Stack<UIPopupBase> _popupStack = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowDialogueUI(bool interactable = true)
    {
        if (yarnCanvasGroup == null) return;

        if(yarnCanvasRoot != null && !yarnCanvasRoot.activeSelf)
            yarnCanvasRoot.SetActive(true);

        SetVisible(yarnCanvasGroup, true, interactable);
    }

    public void HideDialogueUI()
    {
        if (yarnCanvasGroup == null) return;
        SetVisible(yarnCanvasGroup, false, false);
    }

    public void SetMainUIInteractable(bool interactable)
    {
        if (mainCanvasRoot == null) return;

        var cg = mainCanvasRoot.GetComponent<CanvasGroup>();
        if (cg == null) return;

        cg.interactable = interactable;
        cg.blocksRaycasts = interactable;
    }

    public T ShowPopup<T>(T popupPrefab, bool closeByDimClick = false) where T : UIPopupBase
    {
        if (popupPrefab == null) throw new ArgumentNullException(nameof(popupPrefab));
        if (mainCanvasRoot == null) throw new InvalidOperationException("MainCanvasRoot x");

        if (_popupStack.Count > 0)
            _popupStack.Peek().SetInteractable(false);

        var popup = Instantiate(popupPrefab, mainCanvasRoot);
        popup.Initialize(this);

        _popupStack.Push(popup);
        RefreshDim(closeByDimClick);

        popup.OnShown();
        return popup;
    }

    public void CloseTopPopup()
    {
        if (_popupStack.Count == 0) return;

        var top = _popupStack.Pop();
        top.OnBeforeClosed();
        Destroy(top.gameObject);

        if (_popupStack.Count > 0)
            _popupStack.Peek().SetInteractable(true);

        RefreshDim();
    }

    public void CloseAllPopups()
    {
        while(_popupStack.Count > 0)
        {
            var p = _popupStack.Pop();
            p.OnBeforeClosed();
            Destroy(p.gameObject);
        }
        RefreshDim();
    }

    public int PopupCount => _popupStack.Count;

    private void RefreshDim(bool closeByDimClick = false)
    {
        if (popupDim == null) return;

        bool hasPopup = _popupStack.Count > 0;
        SetVisible(popupDim, hasPopup, hasPopup);
    }

    private static void SetVisible(CanvasGroup cg, bool visible, bool interactable)
    {
        cg.alpha = visible ? 1f : 0f;
        cg.interactable = visible && interactable;
        cg.blocksRaycasts = visible && interactable;
    }

    public void EnterDialogueMode(GameObject firstSelected = null)
    {
        ShowDialogueUI(true);

        SetMainUIInteractable(false);

        if (_popupStack.Count > 0)
            _popupStack.Peek().SetInteractable(false);

        if(eventSystem != null)
            eventSystem.SetSelectedGameObject(firstSelected);
    }

    public void ExitDialogueMode()
    {
        HideDialogueUI();

        SetMainUIInteractable(true);

        if (_popupStack.Count > 0)
            _popupStack.Peek().SetInteractable(true);

        if(eventSystem != null)
            eventSystem.SetSelectedGameObject(null);
    }
}
