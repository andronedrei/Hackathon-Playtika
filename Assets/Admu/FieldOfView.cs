using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] protected Material fov_material;
    private Mesh fov_mesh;

    protected uint view_distance = 5;
    protected float fov = 30; // fov ca unghi
    protected int ray_count = 20; // ar trebui sa fie macar 1

    private float angle_offset = 0f;

    void Start()
    {
        fov_mesh = new();
        GetComponent<MeshFilter>().mesh = fov_mesh;
        GetComponent<MeshRenderer>().material = fov_material;
    }

    void Update()
    {
        ray_count = Mathf.Max(1, ray_count);
        float angle_step = fov / ray_count;
        float start_angle = -fov / 2f + angle_offset;

        Vector3[] vertices = new Vector3[ray_count + 2]; // vertice ("puncte" in scena)
        Vector2[] uv = new Vector2[vertices.Length]; // coordonate texturi (cum mapezi 2D => 3D)
        int[] triangles = new int[ray_count * 3]; // grupari de cate 3 vertice formand triunghiurile

        vertices[0] = Vector3.zero;

        for (int i = 0; i <= ray_count; i++)
        {
            float angle = start_angle + i * angle_step;
            float angleRad = angle * Mathf.Deg2Rad;

            Vector3 dir = new(Mathf.Cos(angleRad), Mathf.Sin(angleRad));  // vector trigonometric

            // verificam cu un ray cast daca am intersecta un alt obiect
            RaycastHit2D check_hit = Physics2D.Raycast(transform.position, dir, view_distance);
            if (check_hit.collider == null) {
                // nu am lovit nimic, randam pana la "view distance"
                vertices[i + 1] = dir * view_distance;
            } else {
                // am lovit ceva, randam pana la obiect
                vertices[i + 1] = check_hit.point - (Vector2)transform.position;
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

    // fa campul vizual sa fie indreptat catre o directie
    public void SetDirection(Vector2 dir) {
        angle_offset = Mathf.Atan2(dir.y, dir.x);
    }

    public void SetOrigin(Vector2 point) {
        //transform.position = (Vector2)point;
    }
}
