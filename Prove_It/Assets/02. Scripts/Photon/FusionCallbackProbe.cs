using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FusionCallbacksProbe : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("References")]
    [SerializeField] private NetworkRunner runner;
    [SerializeField] private NetworkSceneManagerDefault sceneManager;

    [Header("Session")]
    [SerializeField] private string sessionName = "MVP_SESSION";

    private async void Start()
    {
        if (runner == null) runner = gameObject.AddComponent<NetworkRunner>();
        if (sceneManager == null) sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();

        runner.AddCallbacks(this);                // (1) 콜백 등록
        runner.ProvideInput = true;

        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = sessionName,
            Scene = SceneRef.FromIndex(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex),
            SceneManager = sceneManager
        });

        Debug.Log($"StartGame Ok={result.Ok}, Reason={result.ShutdownReason}");
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log($"OnConnectedToServer IsServer={runner.IsServer} IsClient={runner.IsClient}");
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"OnPlayerJoined player={player} IsServer={runner.IsServer}");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"OnPlayerLeft player={player} IsServer={runner.IsServer}");
    }

    // 나머지 콜백은 MVP에선 비워둬도 됩니다.
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, System.Collections.Generic.List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, System.Collections.Generic.Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new System.NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new System.NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new System.NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new System.NotImplementedException();
    }
}
