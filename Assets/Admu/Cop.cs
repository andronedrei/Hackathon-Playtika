using UnityEditor.Callbacks;
using UnityEngine;

public class Cop : MovableEntity
{
    protected override void HandleMovement()
    {
        Vector2 movement = Vector2.zero;

        // random movement for AI
        if (Random.value < 0.25f)
        {
            movement = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }

        // Apply the movement
        //rb.linearVelocity = movement * speed * Time.deltaTime;
        transform.Translate(movement * speed * Time.deltaTime);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = base_speed;
        TimeManager.Instance.SubscribeCop(this);
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }
}
