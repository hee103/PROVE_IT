
using UnityEngine;

public class InteractUI : MonoBehaviour
{
    [SerializeField] private GameObject interactButton;

    public void Show()
    {
        if (!interactButton.activeSelf)
            interactButton.SetActive(true);
    }

    public void Hide()
    {
        if (interactButton.activeSelf)
            interactButton.SetActive(false);
    }
}
