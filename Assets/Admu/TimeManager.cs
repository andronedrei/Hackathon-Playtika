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
    private void Start()
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
    
        do
        {
            frozen_idx = (frozen_idx + 1) % nr_freeze_pools;
            
            if (GetCurrentPool().Count > 0)
            {
                break; // am gasit o categorie valida
            }

            if (frozen_idx == originalFrozenIdx)
            {
                Debug.LogWarning("All categories are empty. No valid category to freeze.");
                break;
            }

        } while (true);
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
        switch (frozen_idx)
        {
            case 0:
                foreach (var x in poolThiefs_1) x.OnFreeze();
                break;
            case 1:
                foreach (var x in poolThiefs_2) x.OnFreeze();
                break;
            case 2:
                foreach (var x in poolCops) x.OnFreeze();
                break;
            default:
                Debug.LogError("Error in Freeze function: invalid frozen_idx");
                break;
        }
    }

    // da unfreeze la "categoria" curenta si o seteaza pe urmatoarea
    public void Unfreeze()
    {
        switch (frozen_idx)
        {
            case 0:
                foreach (var x in poolThiefs_1) x.OnUnfreeze();
                break;
            case 1:
                foreach (var x in poolThiefs_2) x.OnUnfreeze();
                break;
            case 2:
                foreach (var x in poolCops) x.OnUnfreeze();
                break;
            default:
                Debug.LogError("Error in Unfreeze function: invalid frozen_idx");
                break;
        }

        ChangeFreezeEntity(); // mergi la urmatoarea categorie
    }
}
