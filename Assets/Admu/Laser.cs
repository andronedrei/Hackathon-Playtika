using UnityEngine;

public class Laser : MonoBehaviour, IFreezable
{
    protected Vector2 dir;
    [SerializeField] float min_angle;
    [SerializeField] float max_angle;
    [SerializeField] float default_rotation_speed;
    protected float cur_rotation_speed;

    //public void

    FieldOfView child_fov;

    public void OnFreeze()
    {
        throw new System.NotImplementedException();
    }

    public void OnUnfreeze()
    {
        throw new System.NotImplementedException();
    }

    protected void HandleRotaion() {

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        child_fov = GetComponentInChildren<FieldOfView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
