using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

// Clasa de tip Singleton pt gestionarea timpului si freez-ului
public class TimeManager : MonoBehaviour
{
    [SerializeField] protected int nr_freeze_pools = 2; // trb sa fie 2 sau 3
    [SerializeField] protected float freeze_time = 1f;
    [SerializeField] protected float wait_freeze_time = 5f;
    //[SerializeField] public CountdownTimer countdown_timer; // child class of time manager
    public CountdownTimer countdown_timer; // child class of time manager
    private bool freezed;
    public static TimeManager Instance = null;

    // Design Pattern de tip "Observer" cu liste intre care se va face freeze alternativ
    private List<IFreezable> poolThiefs_1;
    private List<IFreezable> poolCops;
    private List<IFreezable> poolThiefs_2;

    private int frozen_idx;

    // public static TimeManager Instance
    // {
    //     get
    //     {
    //         if (_instance == null)
    //         {
    //             GameObject obj = new GameObject("TimeManager");
    //             _instance = obj.AddComponent<TimeManager>();
    //             DontDestroyOnLoad(obj);
    //         }
    //         return _instance;
    //     }
    // }

    // ruleaza inainte de primul frame
    private void Awake()
    {
        // if (_instance == null)
        // {
        //     _instance = this; // se asigura ca exista o onstanta
        //     DontDestroyOnLoad(gameObject);
        // }
        // else
        // {
        //     Destroy(gameObject);
        // }
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;

        poolThiefs_1 = new();
        poolCops = new();
        poolThiefs_2 = new();
    }

    void Start()
    {
        // asigura-te ca e un nr corespunzator
        if (nr_freeze_pools < 2 || nr_freeze_pools > 3) {
            nr_freeze_pools = 2;
        }

        freezed = false;
        frozen_idx = 0;

        TtriggerNextAction();

        Debug.Log($"[TimeManager] Initialized with {nr_freeze_pools} pools.");
    }

    // returneaza "pool-ul" curent de entitati pe baza indexului
    private List<IFreezable> GetCurrentPool()
    {
        return frozen_idx switch
        {
            0 => poolThiefs_1,
            1 => poolCops,
            2 => poolThiefs_2,
            _ => null,
        };
    }

    // functie alternare entitate inghetata
    private void ChangeFreezeEntity()
    {
        int originalFrozenIdx = frozen_idx;
    
        // Loop cautare urmatorul "pool" empty
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

    // functii de adaugare in pool-uri (practic "suncribe" la serviciul de freeze)
    public void SubscribeThief_1(IFreezable element) => poolThiefs_1.Add(element);
    public void SubscribeThief_2(IFreezable element) => poolThiefs_2.Add(element);
    public void SubscribeCop(IFreezable element) => poolCops.Add(element);

    // functii de eliminare din pool-uri (practic "unsubcribe" la serviciul de freeze)
    public void UnsubscribeThief_1(IFreezable element) => poolThiefs_1.Remove(element);
    public void UnsubscribeThief_2(IFreezable element) => poolThiefs_2.Remove(element);
    public void UnsubscribeCop(IFreezable element) => poolCops.Remove(element);

    // da freeze la pool-ul focusat in prezent
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

    // da unfreeze si schimba pool-ul focusat
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

    public void TtriggerNextAction() {
        if (!freezed) {
            Freeze();
            countdown_timer.SetTimer(freeze_time, false);
        } else {
            Unfreeze();
            countdown_timer.SetTimer(wait_freeze_time, true);
        }
        freezed = !freezed;
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
