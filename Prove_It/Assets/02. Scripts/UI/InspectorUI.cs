using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InspectorUI : MonoBehaviour
{
    [SerializeField] private TMP_Text logText;
    [SerializeField] private int maxLines = 9;

    private Queue<string> _lines = new();

    public void AddLine(string line)
    {
        if (logText == null) return;

        _lines.Enqueue(line);
        while(_lines.Count > maxLines) _lines.Dequeue();

        logText.text = string.Join("\n", _lines);
    }
}
