using Yarn.Unity;
using UnityEngine;
using System;

public class ConversationController : MonoBehaviour
{
    [SerializeField] private PlayerModeController mode;
    [SerializeField] private DialogueRunner runner;

    private AlienInfo currentAlien;

    private void Awake()
    {
        runner.AddCommandHandler<string, string>("LearnInfo", OnLearnInfo);
        runner.onDialogueComplete.AddListener(OnDialogueComplete);
    }

    public void Begin(string startNode, AlienInfo targetAlien)
    {
        if (runner.IsDialogueRunning) return;

        currentAlien = targetAlien;

        mode.EnterConversationMode();
        UIManager.Instance.EnterDialogueMode();

        runner.StartDialogue(startNode);
    }

    private void OnDialogueComplete()
    {
        currentAlien = null;

        UIManager.Instance.ExitDialogueMode();
        mode.ExitConversationMode();
    }

    public void OnCancel()
    {
        if (!runner.IsDialogueRunning) return;
        runner.Stop();
    }

    public void OnLearnInfo(string keyStr, string value)
    {
        if (currentAlien == null) return;

        if (!Enum.TryParse<InfoKey>(keyStr, true, out var key)) return;

        currentAlien.LearnInfo(key, value);
    }
}
