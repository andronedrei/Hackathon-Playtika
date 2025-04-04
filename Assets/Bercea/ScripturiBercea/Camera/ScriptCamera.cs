using UnityEngine;


[RequireComponent(typeof(Camera))]
public class StaticLevelCamera : MonoBehaviour
{
    [Header("Level View Settings")]
    [Tooltip("Adjust this value to change the zoom level. Smaller values zoom IN, larger values zoom OUT.")]
    public float levelOrthographicSize = 10f; 

    private Camera cam;

    void Awake()
    {
        
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        ApplyOrthographicSize();
    }

    void ApplyOrthographicSize()
    {
        if (cam != null)
        {
            cam.orthographicSize = levelOrthographicSize;
        }
        else
        {
            Debug.LogError("StaticLevelCamera: Camera component not found!");
        }
    }

  
    #if UNITY_EDITOR
    void OnValidate()
    {
       
        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }
        ApplyOrthographicSize();
    }
    #endif
}