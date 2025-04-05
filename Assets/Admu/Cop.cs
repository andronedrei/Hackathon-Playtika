using System;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class Cop : MovableEntity
{
    // Lista cu punctele intre care patruleaza un politist
    [SerializeField] protected List<Vector2> patrol_points = new(); // list of points in between which cop can patrol
    protected int next_patrol_point = 0;
    const float delta_patrol_check = 0.1f;
    private void ChangePatrolPoint() {
        next_patrol_point = (next_patrol_point + 1) % patrol_points.Count;
    }

    private void AdjustDirection() {
        movement_dir = (patrol_points[next_patrol_point] - (Vector2)transform.position).normalized;
    }

    private void CheckPatrolPointReached() {
        // Daca distanta este suficient de mica se considera punctul atins si se patruleaza spre urmatorul
        if ((patrol_points[next_patrol_point] - (Vector2)transform.position).magnitude < delta_patrol_check) {
            ChangePatrolPoint();
        }
    }

    protected override void HandleMovement()
    {
        CheckPatrolPointReached();
        AdjustDirection();
        TranslateEntity();
    }

    void Start()
    {
        movement_dir = Vector2.zero;
        speed = base_speed;
        TimeManager.Instance.SubscribeCop(this);     
    }

    void Update()
    {
        HandleMovement();
    }
}
