using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerModeController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private MonoBehaviour[] disableOnConversation;

    [SerializeField] private string playerMap = "Player";
    [SerializeField] private string conversationMap = "Conversation";

    public void EnterConversationMode()
    {
        foreach (var b in disableOnConversation) b.enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (playerInput != null) playerInput.SwitchCurrentActionMap(conversationMap);
    }

    public void ExitConversationMode()
    {
        if(playerInput != null) playerInput.SwitchCurrentActionMap(playerMap);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        foreach (var b in disableOnConversation) b.enabled = true;
    }
}
