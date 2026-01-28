using Fusion;
using UnityEngine;

public class ConversationInteractable : MonoBehaviour, IInteractable
{
    public bool CanInteract(GameObject interactor) => true;

    public void Interact(GameObject interactor)
    {
        var qNO = interactor.GetComponentInParent<NetworkObject>();
        var rNO = GetComponentInParent<NetworkObject>();

        if (qNO == null || rNO == null) return;

        var agent = qNO.GetComponent<PlayerConversationAgent>();
        if (agent == null) return;

        if(!agent.Object.HasInputAuthority) return;

        agent.RPC_RequestStartConversation(rNO);
        Debug.Log("E");
    }
}
