using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public enum InfoKey { Planet, Greeting, Current, Code, Color}

public class AlienInfo: MonoBehaviour
{
    [SerializeField] private float suspicionDelta = 10f;

    public float Suspicion { get; private set; }
    public event Action<float> OnSuspicionChanged;

    private Dictionary<InfoKey, string> knownInfo = new();

    public void LearnInfo(InfoKey key, string value)
    {
        if(knownInfo.TryGetValue(key, out var existing))
        {
            if(!string.Equals(existing, value, StringComparison.Ordinal))
            {
                Suspicion = Mathf.Clamp(Suspicion + suspicionDelta, 0f, 100f);
                OnSuspicionChanged?.Invoke(Suspicion);
            }
            return;
        }

        knownInfo[key] = value;
    }
    public bool TryGetInfo(InfoKey key, out string value) => knownInfo.TryGetValue(key, out value);
}
public class AlienKnowledge
{
    private HashSet<string> _keys = new();
    public IEnumerable<string> Keys => _keys;

    public bool Has(string key) => _keys.Contains(key);
    public bool Add(string key) => _keys.Add(key);
}

public class AlienSuspicion
{
    public float Value { get; private set; }
    public event Action<float> OnChanged;

    public void Add(float delta)
    {
        if (delta <= 0f) return;
        Value = Mathf.Clamp(Value + delta, 0f, 100f);
        OnChanged?.Invoke(Value);
    }
}


