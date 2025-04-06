using UnityEngine;

public class Laser : MonoBehaviour, IFreezable
{
    protected Vector2 dir;
    [SerializeField] float min_angle; // AICI SUNT IN GRADE
    [SerializeField] float max_angle;
    [SerializeField] float default_rotation_speed;
    protected float cur_angle;
    protected float cur_rotation_speed;
    protected bool does_rotate_forward = true;

    FieldOfView child_fov;

    void IFreezable.OnFreeze()
    {
        cur_rotation_speed = 0;
    }

    void IFreezable.OnUnfreeze()
    {
        cur_rotation_speed = default_rotation_speed;
    }

    bool IFreezable.IsFreezed()
    {
        return cur_rotation_speed == 0;
    }

    protected void HandleRotaion() {
        float delta_angle = cur_rotation_speed * Time.deltaTime;

        if (does_rotate_forward)
        {
            cur_angle += delta_angle;
            if (cur_angle >= max_angle)
            {
                cur_angle = max_angle;
                does_rotate_forward = false;
            }
        } else {
            cur_angle -= delta_angle;
            if (cur_angle <= min_angle)
            {
                cur_angle = min_angle;
                does_rotate_forward = true;
            }
        }

        //transform.rotation = Quaternion.Euler(0, 0, cur_angle); // eventual roteste si laserul

        float angleRad = cur_angle * Mathf.Deg2Rad;
        dir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cur_rotation_speed = default_rotation_speed;
        TimeManager.Instance.SubscribeCop(this);

        child_fov = GetComponentInChildren<FieldOfView>();
        does_rotate_forward = true;
    }

    // Update is called once per frame
    void Update()
    {
        child_fov.UpdateDir(dir);
    }

    void FixedUpdate()
    {
        HandleRotaion();
    }
}
