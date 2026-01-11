using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyNetController : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Fusion")]
    [SerializeField] private NetworkRunner runnerPrefab;
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private string sessionName = "MVP_SESSION";
    [SerializeField] private int gameSceneBuildIndex = 1;

    private NetworkRunner runner;
    private NetworkSceneManagerDefault sceneManager;

    [Header("UGUI")]
    [SerializeField] private TMP_InputField nicknameInput;
    [SerializeField] private Toggle randomToggle;
    [SerializeField] private Toggle inspectorToggle;
    [SerializeField] private Toggle alienToggle;
    [SerializeField] private Button matchButton;
    [SerializeField] private TMP_Text statusText;

    private Dictionary<PlayerRef, NetworkObject> _spawned = new();
    private bool _rolesAssigned;
    private bool _startRequested;

    private void Awake()
    {
        if (matchButton != null)
            matchButton.onClick.AddListener(() => _ = StartMatchAsync());

        SetStatus("대기 중");
    }

    public async Task StartMatchAsync()
    {
        if (_startRequested) return;
        _startRequested = true;

        matchButton.interactable = false;
        SetStatus("매칭 시작...");

        await DisposeRunnerAsync();

        runner = Instantiate(runnerPrefab);
        runner.name = "NetworkRunner";
        runner.ProvideInput = true;

        sceneManager = runner.GetComponent<NetworkSceneManagerDefault>();
        if (sceneManager == null)
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        runner.AddCallbacks(this);

        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = sessionName,
            Scene = SceneRef.FromIndex(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex),
            SceneManager = sceneManager
        });

        if (!result.Ok)
        {
            if (result.ShutdownReason == ShutdownReason.OperationCanceled) SetStatus("매칭 취소됨");
            else
            {
                Debug.Log($"StartGame 실패: {result.ShutdownReason}");
                SetStatus($"매칭 실패: {result.ShutdownReason}");
            }

            await DisposeRunnerAsync();
            matchButton.interactable = true;
            _startRequested = false;
            return;
        }

        SetStatus("세션 참가 완료. 상태 대기 중...");
        matchButton.interactable = false;
        _startRequested = false;
    }

    public async Task CancelMatchAsync()
    {
        SetStatus("매칭 취소 중...");
        await DisposeRunnerAsync();
        matchButton.interactable = true;
        _startRequested = false;
        SetStatus("대기 중");
    }
    
    private async Task DisposeRunnerAsync()
    {
        _rolesAssigned = false;
        _spawned.Clear();

        if (runner == null) return;

        runner.RemoveCallbacks(this);

        try { await runner.Shutdown(); }

        finally
        {
            Destroy(runner.gameObject);
            runner = null;
            sceneManager = null;
        }
    }

    private RolePreference GetSelectedPreference()
    {
        if (inspectorToggle != null && inspectorToggle.isOn) return RolePreference.Inspector;
        if (alienToggle != null && alienToggle.isOn) return RolePreference.Alien;
        return RolePreference.Random;
    }

    private string GetNickname()
    {
        return nicknameInput != null ? nicknameInput.text : "Player";
    }

    private void TrySubmitMyProfile()
    {
        foreach(var kv in _spawned)
        {
            var obj = kv.Value;
            if(obj == null) continue;

            if (obj.HasInputAuthority)
            {
                var np = obj.GetComponent<NetworkPlayer>();
                if(np == null) continue;

                np.Rpc_SubmitProfile(GetNickname(), GetSelectedPreference());
                return;
            }
        }
    }

    private void TryAssignRolesAndStartGame()
    {
        if (!runner.IsServer) return;
        if (_rolesAssigned) return;
        if (_spawned.Count < 2) return;

        //역할 확정
        RoleAssigner.AssignRoles_Server(_spawned);
        _rolesAssigned = true;

        SetStatus("역할 확정. 게임 씬 로딩...");

        if (runner.IsSceneAuthority)
            runner.LoadScene(SceneRef.FromIndex(gameSceneBuildIndex));
    }

    private void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
    }

    ///INetworkRunnerCallbakcs
    ///
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            var pos = new Vector3(_spawned.Count * 2f, 0f, 0f);
            var obj = runner.Spawn(playerPrefab, pos, Quaternion.identity, player);
            _spawned[player] = obj;

            var np = obj.GetComponent<NetworkPlayer>();
            np?.Server_InitDefults();
        }

        TrySubmitMyProfile();

        TryAssignRolesAndStartGame();
        SetStatus(runner.IsServer ? "플레이어 참가(Host)" : "플레이어 참가(Client)");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        TrySubmitMyProfile();

        TryAssignRolesAndStartGame();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if(runner.IsServer && _spawned.TryGetValue(player, out var obj))
        {
            runner.Despawn(obj);
            _spawned.Remove(player);
            _rolesAssigned = false;
        }

        SetStatus("상대가 나갔습니다. 대기 상태로 복귀");
        matchButton.interactable = true;
        _startRequested = false;
    }

    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    // Fusion 버전에 따라 AOI 콜백이 필수일 수 있음(당신이 겪은 것)
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new System.NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new System.NotImplementedException();
    }
}
