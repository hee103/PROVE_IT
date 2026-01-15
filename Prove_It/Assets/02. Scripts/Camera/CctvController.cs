using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CctvController : MonoBehaviour
{
    public enum ViewMode { Fps, Cctv }

    [Header("카메라")]
    [SerializeField] private Camera fpsCamera;
    [SerializeField] private Camera cctvCamera;

    [Header("게임 씬 설정")]
    [SerializeField] private int gameSceneBuildIndex = 1;

    [Header("FPS Look 스크립트")]
    [SerializeField] private MonoBehaviour fpsLookScript;

    [Header("커서")]
    [SerializeField] private bool showCursor = true;

    public ViewMode CurrentMode { get; private set; } = ViewMode.Fps;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        TryBind();
        ForceStartFps();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void Awake()
    {
        //if(fpsCamera == null) fpsCamera = Camera.main;

        if(fpsCamera == null)
        {
            Debug.Log("[CctvController] fpsCamera x");
            enabled = false;
            return;
        }

        ApplyMode(ViewMode.Fps);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryBind();
        ForceStartFps();
    }

    private void TryBind()
    {
        var active = SceneManager.GetActiveScene();
        if (active.buildIndex != gameSceneBuildIndex) return;
        if(fpsCamera == null)
        {
            fpsCamera = GetComponentInChildren<Camera>(true);
        }
        if(cctvCamera == null)
        {
            var go = GameObject.FindGameObjectWithTag("CCTV");
            if(go != null) cctvCamera = go.GetComponent<Camera>();
        }
        Debug.Log($"[CctvController] Bind fps={fpsCamera != null}, cctv={cctvCamera != null} scene={active.buildIndex}");
    }
    public void OnToggleCctv(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        ApplyMode(CurrentMode == ViewMode.Fps ? ViewMode.Cctv : ViewMode.Fps);
    }

    private void ForceStartFps()
    {
        if (fpsCamera == null) return;

        fpsCamera.enabled = true;

        if (cctvCamera != null) cctvCamera.enabled = false;

        ApplyMode(ViewMode.Fps);
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
