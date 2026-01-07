
using UnityEngine;
using Yarn.Unity;
using System.Collections;

public class InteractUI : MonoBehaviour
{
    [SerializeField] private GameObject image;
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private string startNode;

    private void Awake()
    {
        if (dialogueRunner == null) return;

        dialogueRunner.onDialogueStart.AddListener(() =>
        {
            Debug.Log("onDialogueStart fired (대화 시작됨)");
        });

        dialogueRunner.onDialogueComplete.AddListener(() =>
        {
            Debug.Log("onDialogueComplete fired (대화 끝남)");
        });
    }
    public void Show()
    {
        image.SetActive(true);
    }

    public void Hide()
    {
        image.SetActive(false);
    }
    public void Interact()
    {
        if (dialogueRunner == null || dialogueRunner.IsDialogueRunning) return;

        dialogueRunner.StartDialogue(startNode);
    }

}
