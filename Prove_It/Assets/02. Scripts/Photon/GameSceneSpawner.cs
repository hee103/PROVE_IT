using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEditor;
using UnityEngine;

public class GameSceneSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Config")]
    [SerializeField] private int gameSceneBuildIndex = 1;

    [Header("Prefabs")]
    [SerializeField] private NetworkPrefabRef playerPrefab;

    private Dictionary<PlayerRef, NetworkObject> _spawned = new();

    private void Awake()
    {
        var runner = FindFirstObjectByType<NetworkRunner>();
        if(runner == null)
        {
            Debug.Log("[GameScene] NetworkRunner not found.");
            return;
        }

        runner.AddCallbacks(this);
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != gameSceneBuildIndex)
            return;

        if (!runner.IsServer) return;

        SpawnAllPlayerIfNeeded(runner);
    }

    private void SpawnAllPlayerIfNeeded(NetworkRunner runner)
    {

        int i = 0;
        foreach (var player in runner.ActivePlayers)
        {
            if (_spawned.ContainsKey(player)) continue;

            var pos = new Vector3(i * 2.5f, 0f, 0f);
            var obj = runner.Spawn(playerPrefab, pos, Quaternion.identity, player);
            _spawned[player] = obj;

            runner.SetPlayerObject(player, obj);

            var role = obj.GetComponent<PlayerRole>();
            var sessionState = runner.GetComponent<SessionState>();

            if(sessionState != null && role != null && sessionState.AssignedRoles.TryGetValue(player, out var _role))
            {
                role.Server_SetRole(_role);
                Debug.Log($"[GameRoleInject] {{PlayerRef}} -> {_role}");
            }
            else
            {
                Debug.LogWarning($"[GameRoleInject] missing. sessionState = {sessionState != null}, role = {role != null} \n , hasRole = {sessionState != null && sessionState.AssignedRoles.ContainsKey(player)}");
            }

                i++;
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new System.NotImplementedException();
    }
}
