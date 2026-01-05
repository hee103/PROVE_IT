using UnityEngine;
using System.IO;

public class PlanetDataTest : MonoBehaviour
{
    void Start()
    {
        string path = Path.Combine(
            Application.streamingAssetsPath,
            "planet_data.json"
        );

        string json = File.ReadAllText(path);
        Debug.Log(json);
    }
}
