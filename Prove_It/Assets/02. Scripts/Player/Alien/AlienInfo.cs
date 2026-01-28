using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public enum InfoKey { Planet, Greeting, Current, Code, Color}
public enum A2APhase { None, Asking, Answering }

public class AlienInfo: NetworkBehaviour
{
    [SerializeField] private float suspicionDelta = 10f;

    [Networked] public float Suspicion { get; private set; }

    [Networked, Capacity(8)] public NetworkString<_32> Planet { get; private set; }
    [Networked, Capacity(8)] public NetworkString<_32> Greeting { get; private set; }
    [Networked, Capacity(8)] public NetworkString<_32> Current { get; private set; }
    [Networked, Capacity(8)] public NetworkString<_32> Code { get; private set; }
    [Networked, Capacity(8)] public NetworkString<_32> Color { get; private set; }
    
    public event Action<float> OnSuspicionChanged;

    private Dictionary<InfoKey, string> knownInfo = new();

    public void Server_LearnInfo(InfoKey key, string value)
    {
        if (!Object.HasStateAuthority) return;

        var old = GetValue(key);
        if (!string.IsNullOrEmpty(old) && old != value) Suspicion++;

        SetValue(key, value);
        
    }

    private string GetValue(InfoKey key) => key switch
    {
        InfoKey.Planet => Planet.ToString(),
        InfoKey.Greeting => Greeting.ToString(),
        InfoKey.Current => Current.ToString(),
        InfoKey.Code => Code.ToString(),
        InfoKey.Color => Color.ToString(),
        _ => ""
    };

    private void SetValue(InfoKey key, string value)
    {
        switch (key)
        {
            case InfoKey.Planet: Planet = value; break;
            case InfoKey.Greeting: Greeting = value; break;
            case InfoKey.Current: Current = value; break;
            case InfoKey.Code: Code = value; break;
            case InfoKey.Color: Color = value; break;
        }
    }

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


