using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] protected uint view_distance;
    [SerializeField] protected float fov; // fov ca unghi
    [SerializeField] protected int ray_count; // ar trebui sa fie macar 1
    [SerializeField] protected Material fov_material;

    private Mesh fov_mesh;

    void Start()
    {
        fov_mesh = new();
        GetComponent<MeshFilter>().mesh = fov_mesh;
        GetComponent<MeshRenderer>().material = fov_material;
    }

    void Update()
    {
        Vector3 fov_origin = transform.position;

        ray_count = Mathf.Max(1, ray_count);
        float angle_step = fov / ray_count;
        float start_angle = -fov / 2f;

        Vector3[] vertices = new Vector3[ray_count + 2]; // vertice ("puncte" in scena)
        Vector2[] uv = new Vector2[vertices.Length]; // coordonate texturi (cum mapezi 2D => 3D)
        int[] triangles = new int[ray_count * 3]; // grupari de cate 3 vertice formand triunghiurile

        vertices[0] = fov_origin;

        for (int i = 0; i <= ray_count; i++)
        {
            float angle = start_angle + i * angle_step;
            float angleRad = angle * Mathf.Deg2Rad;

            Vector3 dir = new(Mathf.Cos(angleRad), Mathf.Sin(angleRad));  // vector trigonometric

            // verificam cu un ray cast daca am intersecta un alt obiect
            RaycastHit2D check_hit = Physics2D.Raycast(fov_origin, dir, view_distance);
            if (check_hit.collider == null) {
                // nu am lovit nimic, randam pana la "view distance"
                vertices[i + 1] = dir * view_distance;
            } else {
                // am lovit ceva, randam pana la obiect
                vertices[i + 1] = check_hit.point - (Vector2)fov_origin;
            }
            
        }

        for (int i = 0; i < ray_count; i++)
        {
            triangles[i * 3 + 0] = 0;  // punct central pt toate triunghiulrile
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        fov_mesh.vertices = vertices;
        fov_mesh.uv = uv;
        fov_mesh.triangles = triangles;
    }
}
