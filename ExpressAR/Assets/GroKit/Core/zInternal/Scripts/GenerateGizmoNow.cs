using UnityEngine;

public class GenerateGizmoNow : MonoBehaviour
{
    public float radius;
    public static void MakeGizmo(Vector3 postion, float radius)
    {
        GameObject holder = Instantiate(new GameObject("GizmoShphere"), postion, Quaternion.identity);
        holder.AddComponent<GenerateGizmoNow>().radius = radius;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
