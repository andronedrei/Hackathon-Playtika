using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Rendering;

public class TimeManager : MonoBehaviour
{
    private static int nr_freeze_pools = 3;
    private static TimeManager _instance;

    private List<Freezable> poolThiefs_1 = new();
    private List<Freezable> poolThiefs_2 = new();
    private List<Freezable> poolCops = new();

    private int frozen_idx = 0;

    public static TimeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("TimeManager");
                _instance = obj.AddComponent<TimeManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    // ruleaza inainte de primul frame
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this; // se asigura ca exista o onstanta
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Get the current pool based on frozen_idx
    private List<Freezable> GetCurrentPool()
    {
        switch (frozen_idx)
        {
            case 0: return poolThiefs_1;
            case 1: return poolThiefs_2;
            case 2: return poolCops;
            default: return null;
        }
    }

    // functie alternare entitate inghetata
    private void ChangeFreezeEntity()
    {
        // continua in loop pana dai de o categorie cu elemente
        int originalFrozenIdx = frozen_idx;
    
        for (int i = 0; i < nr_freeze_pools; i++)
        {
            frozen_idx = (frozen_idx + 1) % nr_freeze_pools;
            
            if (GetCurrentPool().Count > 0)
            {
                return; // am gasit o categorie valida
            }

        }

        frozen_idx = originalFrozenIdx;
        Debug.LogError("No category with items");
    }

    // adauga elemente in listele de gestiune a freezului
    public void SubscribeThief_1(Freezable element) => poolThiefs_1.Add(element);
    public void SubscribeThief_2(Freezable element) => poolThiefs_2.Add(element);
    public void SubscribeCop(Freezable element) => poolCops.Add(element);

    // scoate elemente din listele de gestiune ale freezului
    public void UnsubscribeThief_1(Freezable element) => poolThiefs_1.Remove(element);
    public void UnsubscribeThief_2(Freezable element) => poolThiefs_2.Remove(element);
    public void UnsubscribeCop(Freezable element) => poolCops.Remove(element);

    // da freeze la "categoria" curenta
    public void Freeze()
    {
        var pool = GetCurrentPool();
        if (pool == null)
        {
            Debug.LogWarning("Freeze(): Current pool is null.");
            return;
        }

        foreach (var x in pool)
        {
            x.OnFreeze();
        }
    }

    // da unfreeze la "categoria" curenta si o seteaza pe urmatoarea
    public void Unfreeze()
    {
        var pool = GetCurrentPool();
        if (pool == null)
        {
            Debug.LogWarning("UnFreeze(): Current pool is null.");
            return;
        }

        foreach (var x in pool)
        {
            x.OnUnfreeze();
        }

        ChangeFreezeEntity(); // mergi la urmatoarea categorie
    }

    public void MyDebug()
    {
        Debug.Log($"[TimeManager] ======= DEBUG INFO =======");
        Debug.Log($"Frozen Index: {frozen_idx}");
        Debug.Log($"Thiefs_1 Pool Count: {poolThiefs_1.Count}");
        Debug.Log($"Thiefs_2 Pool Count: {poolThiefs_2.Count}");
        Debug.Log($"Cops Pool Count: {poolCops.Count}");

        for (int i = 0; i < nr_freeze_pools; i++)
        {
            var pool = i switch
            {
                0 => poolThiefs_1,
                1 => poolThiefs_2,
                2 => poolCops,
                _ => null
            };

            if (pool != null)
            {
                Debug.Log($"Pool {i}: {pool.Count} elements");
                for (int j = 0; j < pool.Count; j++)
                {
                    Debug.Log($"    [{j}] {pool[j].GetType()}");
                }
            }
        }
        Debug.Log($"[TimeManager] =========================");
    }
}
