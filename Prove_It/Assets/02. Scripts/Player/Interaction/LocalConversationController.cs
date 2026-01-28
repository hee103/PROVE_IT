using Yarn.Unity;
using UnityEngine;
using Fusion;

public enum ConversationKind
{
    InspectorToAlien,
    AlienToNpc,
    AlienToAlien
}

public enum ConversationPhase
{
    None = 0,
    ResponderOnly = 10,
    Asking = 20,
    Answering = 21
}
public class LocalConversationController : MonoBehaviour
{
    [SerializeField] private PlayerModeController mode;
    [SerializeField] private DialogueRunner runner;

    private ConversationSessionManager _mgr;
    private ConversationSessionManager Mgr
    {
        get 
        {
            if (_mgr == null)
                _mgr = ConversationSessionManager.Instance_LocalOrFind();
              return _mgr; 
        }
    }
    private int _sessionId;
    private NetworkObject _otherPlayer;

    private void Awake()
    {
        runner.AddCommandHandler<string>("Ask", OnAskCommand);
        runner.AddCommandHandler<string, string>("Answer", OnAnswerCommand);
        runner.AddCommandHandler("EndConversation", OnEndCommand);
        runner.AddCommandHandler<string, string>("TellInfo", OnTellInfoCommand);
        runner.AddCommandHandler<string, string>("LearnInfo", OnLearnInfoCommand);
        runner.onDialogueComplete.AddListener(OnDialogueComplete);
    }

    public void BeginSession(int sessionId, string nodeName, NetworkObject other)
    {
        if (runner.IsDialogueRunning) return;
        if (_sessionId != 0) return;

        _sessionId = sessionId;
        _otherPlayer = other;

        UIManager.Instance.ShowDialogueUI(true);
        UIManager.Instance.SetMainUIInteractable(false);
        mode.EnterConversationMode();

        runner.StartDialogue(nodeName);
    }

    public void EndSession(int sessionId)
    {
        if (_sessionId != sessionId) return;

        if(runner != null && runner.IsDialogueRunning) 
            runner.Stop();

        UIManager.Instance.HideDialogueUI();
        UIManager.Instance.SetMainUIInteractable(true);
        mode.ExitConversationMode();

        _sessionId = 0;
        _otherPlayer = null;
    }

    private void OnDialogueComplete()
    {
        _otherPlayer = null;

        UIManager.Instance.ExitDialogueMode();
        mode.ExitConversationMode();
    }

    // ========== Yarn Commnad ==========
    private void OnAskCommand(string keyStr)
    {
        if(_sessionId == 0) return;
        if (!System.Enum.TryParse<InfoKey>(keyStr, true, out var key)) return;

        Mgr?.SubmitAsk(_sessionId, key);
    }

    private void OnAnswerCommand(string keyStr, string value)
    {
        if (_sessionId == 0) return;
        if (!System.Enum.TryParse<InfoKey>(keyStr, out var key)) return;

        Mgr?.SubmitAnswer(_sessionId, key, value);
    }

    private void OnLearnInfoCommand(string keyStr, string value)
    {
        if (_sessionId == 0) return;
        if (!System.Enum.TryParse<InfoKey>(keyStr, true, out var key)) return;
        Mgr.SubmitLearnInfo(_sessionId, key, value);
    }

    private void OnTellInfoCommand(string keyStr, string value)
    {
        if(_sessionId == 0) return;
        if (!System.Enum.TryParse<InfoKey>(keyStr, true, out var key)) return;
        Mgr?.SubmitTellInfo(_sessionId, key, value);
    }
    private void OnEndCommand()
    {
        if (_sessionId == 0) return;
        Mgr?.SubmitEnd(_sessionId);
    }
}
