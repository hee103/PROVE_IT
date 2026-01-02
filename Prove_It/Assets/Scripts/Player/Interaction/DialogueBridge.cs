using System;
using Yarn.Unity;
using UnityEngine;

public class DialogueBridge : MonoBehaviour
{
    private AlienInfo currentAlien;

    public void SetCurrentAlien(AlienInfo alien) => currentAlien = alien;

    [YarnCommand("learnInfo")]
    public void LearnInfo(string keyStr, string value)
    {
        if (currentAlien == null) return;

        if (!Enum.TryParse<InfoKey>(keyStr, true, out var key)) return;

        currentAlien.LearnInfo(key, value);
    }
}
