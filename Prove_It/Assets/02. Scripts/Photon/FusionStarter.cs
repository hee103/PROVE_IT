using System.Threading.Tasks;
using Fusion;
using UnityEngine;

public class FusionStarter : MonoBehaviour
{
    [SerializeField] private NetworkRunner runner;
    [SerializeField] private NetworkSceneManagerDefault sceneManager;

    [SerializeField] private string sessionName = "MVP_SESSION";
    [SerializeField] private GameMode gameMode = GameMode.AutoHostOrClient;

    private bool _starting;

    private async void Start()
    {
        await StartGame();
    }

    public async Task StartGame()
    {
        if (_starting) return;
        _starting = true;

        runner.ProvideInput = true;

        var agrs = new StartGameArgs
        {
            GameMode = gameMode,
            SessionName = sessionName,
            Scene = SceneRef.FromIndex(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex),
            SceneManager = sceneManager
        };

        var result = await runner.StartGame(agrs);
        if (!result.Ok)
        {
            Debug.LogError($"StartGame failed: {result.ShutdownReason}");
            _starting = false;
            return;
        }

        Debug.Log($"StartGame OK. Mode = {gameMode}, Session = {sessionName}");
    }
}
