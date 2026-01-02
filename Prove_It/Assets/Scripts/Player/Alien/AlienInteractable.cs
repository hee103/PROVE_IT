using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string startNode = "Alien_Intro";
    public bool CanInteract(GameObject interactor) => !string.IsNullOrEmpty(startNode);

    public void Interact(GameObject interactor)
    {
        var conv = interactor.GetComponentInParent<ConversationController>();
        if (conv == null) return;

        var alienInfo = GetComponent<AlienInfo>();
        if (alienInfo == null) Debug.Log("AlienInfo x");
        conv.Begin(startNode, alienInfo);
    }
    
}
