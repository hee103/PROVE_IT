using Fusion;
using System.Collections.Generic;

using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ConversationSessionManager : NetworkBehaviour
{
    public static ConversationSessionManager Instance { get; private set; }

    private class Session
    {
        public int Id;
        public ConversationKind Kind;
        public ConversationPhase Phase;

        public PlayerRef Qer;   //질문자
        public PlayerRef Rer;   //응답자
        public NetworkObject QerNO;
        public NetworkObject RerNO;

        public InfoKey PendingKey;  // A <-> A 질문 키
        public bool Active;

        public NetworkObject Target;    //대화 대상(NPC or 플레이어)
        public string StartNode;    // A -> N or I -> A 시작 노드
    }

    private int _nextId = 1;
    private Dictionary<int, Session> _sessions = new();

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public static ConversationSessionManager Instance_LocalOrFind()
    {
        if (Instance != null) return Instance;
        return FindAnyObjectByType<ConversationSessionManager>();
    }

    // =========== 시작 요청 ===========
    // Qer가 Interact 했을 때 서버로 요청
    public void Server_TryStartConversation(NetworkObject qerNO, NetworkObject target)
    {
        if (!Object.HasStateAuthority) return; 
        if (qerNO == null || target == null) return; 

        //서버 검증(역할 검사)
        var qPlayer = qerNO.GetComponent<NetworkPlayer>();
        if (qPlayer == null) return;

        if (qPlayer.Role == AssignedRole.None)
        {
            Debug.Log("[CSM] reject: initiator role not ready", this);
            return;
        }
        var rPlayer = target.GetComponent<NetworkPlayer>();
        var npc = target.GetComponent<NpcConversation>();

        // ------- 조합 판정 --------
        // 1. A -> A
        if (rPlayer != null)
        {
            if (qPlayer.Role == AssignedRole.Alien && rPlayer.Role == AssignedRole.Alien)
            {
                StartAlienToAlien(qPlayer, rPlayer, qerNO, target);
                return;
            }

            // 2. I -> A
            if(qPlayer.Role == AssignedRole.Inspector && rPlayer.Role == AssignedRole.Alien)
            {
                StartInspectorToAlien(qPlayer, rPlayer, qerNO, target);
                return;
            }
            // A -> I는 없음
            return;
        }

        // 3. A -> N
        if(npc != null)
        {
            if (qPlayer.Role != AssignedRole.Alien) return;
            StartAlienToNpc(qPlayer, npc, qerNO, target);
            return;
        }
    }

    private bool IsBusy(PlayerRef player)
    {
        foreach (var kv in _sessions)
        {
            var s = kv.Value;
            if (!s.Active) continue;
            if (s.Qer == player || s.Rer == player) return true;
        }
        return false;
    }

    private int NewId() => _nextId++;

    private void StartAlienToAlien(NetworkPlayer qPlayer, NetworkPlayer rPlayer, NetworkObject qNO, NetworkObject rNO)
    {
        var qRef = qPlayer.Object.InputAuthority;
        var rRef = rPlayer.Object.InputAuthority;

        if (IsBusy(qRef) || IsBusy(rRef)) return;

        var s = new Session
        {
            Id = NewId(),
            Kind = ConversationKind.AlienToAlien,
            Phase = ConversationPhase.Asking,
            Active = true,
            Qer = qRef,
            Rer = rRef,
            QerNO = qNO,
            RerNO = rNO,
            Target = rNO,
        };
        _sessions[s.Id] = s;

        BeginAskPhase_Server(s);
    }

    private void BeginAskPhase_Server(Session s)
    {
        s.Phase = ConversationPhase.Asking;

        var qAgent = s.QerNO.GetComponent<PlayerConversationAgent>();
        if (qAgent != null)
        {
            qAgent.RPC_BeginLocalDialogue(s.Id, "A2A_Ask", s.RerNO);
        }
    }

    private void BeginAnswerPhase_Server(Session s, InfoKey key)
    {
        s.Phase = ConversationPhase.Answering;
        s.PendingKey = key;

        var qAgent = s.QerNO.GetComponent<PlayerConversationAgent>();
        if(qAgent != null) qAgent.RPC_EndLocalDialogue(s.Id);

        var rAgent = s.RerNO.GetComponent<PlayerConversationAgent>();
        if (rAgent != null)
        {
            string node = $"A2A_Answer_{key}";
            rAgent.RPC_BeginLocalDialogue(s.Id, node, s.QerNO);
        }
    }

    //I -> A
    private void StartInspectorToAlien(NetworkPlayer inspector, NetworkPlayer alien, NetworkObject iNO, NetworkObject aNO)
    {
        var qRef = inspector.Object.InputAuthority;
        var rRef = alien.Object.InputAuthority;

        if (IsBusy(qRef) || IsBusy(rRef)) return;

        // 응답자(Alien)만 UI 실행
        var s = new Session
        {
            Id = NewId(),
            Kind = ConversationKind.InspectorToAlien,
            Phase = ConversationPhase.ResponderOnly,
            Active = true,
            Qer = qRef,
            Rer = rRef,
            QerNO = iNO,
            RerNO = aNO,
            Target = aNO,
            StartNode = "Inspector_Dafulat"
        };
        _sessions[s.Id] = s;

        var rAgent = s.RerNO.GetComponent<PlayerConversationAgent>();
        if(rAgent != null) 
            rAgent.RPC_BeginLocalDialogue(s.Id, s.StartNode, s.QerNO);
    }

    // A -> N
    private void StartAlienToNpc(NetworkPlayer alien, NpcConversation npc, NetworkObject aNO, NetworkObject nNO)
    {
        var rRef = alien.Object.InputAuthority;
        if (IsBusy(rRef)) return;

        var s = new Session
        {
            Id = NewId(),
            Kind = ConversationKind.AlienToNpc,
            Phase = ConversationPhase.ResponderOnly,
            Active = true,
            Qer = rRef,
            Rer = rRef,
            QerNO = aNO,
            RerNO = aNO,
            Target = nNO,
            StartNode = string.IsNullOrEmpty(npc.startNode) ? "NPC_Default" : npc.startNode
        };
        _sessions[s.Id] = s;

        var agent = aNO.GetComponent<PlayerConversationAgent>();
        if (agent != null)
            agent.RPC_BeginLocalDialogue(s.Id, s.StartNode, nNO);
    }

    // ========= 클라 제출 API(로컬 컨트롤러가 호출) =========
    public void SubmitAsk(int sessionId, InfoKey key) => RPC_SubmitAsk(sessionId, key);
    public void SubmitAnswer(int sessoinId, InfoKey key, string value) => RPC_SubmitAnswer(sessoinId, key, value);
    public void SubmitLearnInfo(int sessionId, InfoKey key, string value) => RPC_SubmitLearnInfo(sessionId, key, value);
    public void SubmitTellInfo(int sessionId, InfoKey key, string value) => RPC_SubmitTellInfo(sessionId, key, value);
    public void SubmitEnd(int sessoinId) => RPC_SubmitEnd(sessoinId);

    // =========ㅣ RPC 핸들러(서버) ㅣ==========
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SubmitAsk(int sessionIdd, InfoKey key, RpcInfo info = default)
    {
        if (!Object.HasStateAuthority) return;
        if (!_sessions.TryGetValue(sessionIdd, out var s) || !s.Active) return;

        //검증: phase == Asking && 호출자는 Qer
        if (s.Kind != ConversationKind.AlienToAlien) return;
        if (s.Phase != ConversationPhase.Asking) return;
        if (info.Source != s.Qer) return;

        BeginAnswerPhase_Server(s, key);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SubmitAnswer(int sessoinId, InfoKey key, string value, RpcInfo info = default)
    {
        if (!Object.HasStateAuthority) return;
        if (!_sessions.TryGetValue(sessoinId, out var s) || !s.Active) return;

        //검증: phase == Answering && 호출자는 Rer
        if (s.Kind != ConversationKind.AlienToAlien) return;
        if (s.Phase != ConversationPhase.Answering) return;
        if (info.Source != s.Rer) return;

        //검증: key 일치
        if (key != s.PendingKey) return;

        var qInfo = s.QerNO.GetComponent<AlienInfo>();
        if (qInfo != null) qInfo.Server_LearnInfo(key, value);

        //일단 한번 답변하고 종료
        EndSession_Server(s);
        //확장 시 BeginAskPhase_Server(s);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SubmitLearnInfo(int sessionId, InfoKey key, string value, RpcInfo info = default)
    {
        if(!Object.HasStateAuthority) return;
        if (!_sessions.TryGetValue(sessionId, out var s) || !s.Active) return;

        if (s.Kind != ConversationKind.AlienToNpc) return;
        if (info.Source != s.Rer) return;

        var infoComp = s.RerNO.GetComponent<AlienInfo>();
        if (infoComp != null) infoComp.Server_LearnInfo(key, value);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SubmitTellInfo(int sessionId, InfoKey key, string value, RpcInfo info = default)
    {
        if (!Object.HasStateAuthority) return;
        if (!_sessions.TryGetValue(sessionId, out var s) || !s.Active) return;

        if (s.Kind != ConversationKind.InspectorToAlien) return;
        if (info.Source != s.Rer) return;

        var qAgent = s.QerNO.GetComponent<PlayerConversationAgent>();
        if (qAgent != null) qAgent.RPC_ReceiveInspectorAnswer(sessionId, key, value);

        // 답변 기록 확인용 로그
        Debug.Log($"[Inspector] Alien answered {{key}} = {value} for Inspector {s.Qer}");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SubmitEnd(int sessionId, RpcInfo info = default)
    {
        if (!Object.HasStateAuthority) return;
        if (!_sessions.TryGetValue(sessionId, out var s) || !s.Active) return;

        if (info.Source != s.Qer && info.Source != s.Rer) return;

        EndSession_Server(s);
    }

    private void EndSession_Server(Session s)
    {
        s.Active = false;

        // 양측 UI 닫기
        var qAgent = s.QerNO.GetComponent<PlayerConversationAgent>();
        if (qAgent != null) qAgent.RPC_EndLocalDialogue(s.Id);

        var rAgent = s.RerNO.GetComponent<PlayerConversationAgent>();
        if (rAgent != null) rAgent.RPC_EndLocalDialogue(s.Id);

        _sessions.Remove(s.Id);
    }
}
