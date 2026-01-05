using UnityEngine;
using UnityEngine.InputSystem;

public class CctvController : MonoBehaviour
{
    public enum ViewMode { Fps, Cctv }

    [Header("카메라")]
    [SerializeField] private Camera fpsCamera;
    [SerializeField] private Camera cctvCamera;

    [Header("FPS Look 스크립트")]
    [SerializeField] private MonoBehaviour fpsLookScript;

    [Header("커서")]
    [SerializeField] private bool showCursor = true;

    public ViewMode CurrentMode { get; private set; } = ViewMode.Fps;

    private void Reset()
    {
        fpsCamera = Camera.main;
    }

    private void Awake()
    {
        if(fpsCamera == null) fpsCamera = Camera.main;

        if(fpsCamera == null || cctvCamera == null)
        {
            Debug.Log("fpsCamera, cctvCamera x");
        }

        ApplyMode(ViewMode.Fps);
    }

    public void OnToggleCctv(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        ApplyMode(CurrentMode == ViewMode.Fps ? ViewMode.Cctv : ViewMode.Fps);
    }

    private void ApplyMode(ViewMode mode)
    {
        CurrentMode = mode;

        bool isCctv = (mode == ViewMode.Cctv);
        //카메라 활성
        if (fpsCamera != null) fpsCamera.enabled = !isCctv;
        if (cctvCamera != null) cctvCamera.enabled = isCctv;

        //fps 시점 회전 방지
        if (fpsLookScript != null) fpsLookScript.enabled = !isCctv;

        //커서 처리
        if (showCursor)
        {
            Cursor.lockState = isCctv ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isCctv;
        }

    }
}
