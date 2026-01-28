using Fusion;
using UnityEngine;

public class PlayerConversationAgent : NetworkBehaviour
{
    [Header("Roots")]
    [SerializeField] private Transform inspectorRoot;
    [SerializeField] private Transform alienRoot;

    [SerializeField] private InspectorUI inspectorUI;
    private LocalConversationController localController;

    private NetworkPlayer _player;
    public override void Spawned()
    {
        _player = GetComponent<NetworkPlayer>();

        ResolveLocalComponents();
    }

    private void ResolveLocalComponents()
    {
        if (_player == null || _player.Role == AssignedRole.None) return;

        //Inspector
        if(_player.Role == AssignedRole.Inspector)
        {
            if (inspectorRoot == null) return;

            localController = inspectorRoot.GetComponentInChildren<LocalConversationController>(true);
            inspectorUI = GetComponentInChildren<InspectorUI>(true);
        }
        //Alien
        else if(_player.Role == AssignedRole.Alien)
        {
            if(alienRoot == null) return;
            localController = inspectorRoot.GetComponentInChildren<LocalConversationController>(true);
            inspectorUI = null;
        }
    }

    //서버 -> 특정 클라 지시
    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    public void RPC_BeginLocalDialogue(int sessionId, string nodeName, NetworkObject other)
    {
        Debug.Log($"[PCA] BeginLocalDialogue session={sessionId} node={nodeName} other={other}", this);

        if (localController == null) return;
        
        localController.BeginSession(sessionId, nodeName, other);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    public void RPC_EndLocalDialogue(int sessionId)
    {
        if(localController == null) return;
        localController.EndSession(sessionId);
    }

    // I->A일떄 답변 전달
    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    public void RPC_ReceiveInspectorAnswer(int sessionId, InfoKey key, string value)
    {
        if (inspectorUI == null) return;

        inspectorUI.AddLine($"답변: {{key}} = {value}");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestStartConversation(NetworkObject targetNO, RpcInfo info = default)
    {
        if (!Object.HasStateAuthority) return;
        if (targetNO == null) return;

        var mgr = ConversationSessionManager.Instance_LocalOrFind();
        if (mgr == null) return;

        mgr.Server_TryStartConversation(Object, targetNO);
    }

}
