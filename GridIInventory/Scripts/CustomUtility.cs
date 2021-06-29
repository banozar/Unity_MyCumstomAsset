using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomUtility : MonoBehaviour
{
    public static void OverLap_UI_Swap(GameObject Target)
    {
        GameObject Group = Target.transform.parent.gameObject;
        for (int i = 0; i < Group.transform.childCount; i++)   Group.transform.GetChild(i).gameObject.SetActive(false);
        Target.SetActive(true);
    }
    public static float Rotate_Value(Transform Ori, Vector3 Target)
    {
        Vector3 a = Ori.transform.forward;
        Vector3 b = Target - Ori.transform.position;
        b = b.normalized;
        float rot = 0.0f;

        rot = Vector3.Dot(a, b);

        rot = Mathf.Acos(rot);
        rot = (rot * 180) / Mathf.PI;
        Vector3 right = Vector3.Cross(Vector3.up, a);
        right.Normalize();
        if (Vector3.Dot(right, b) < 0.0f)
            rot *= -1;

        if (float.IsNaN(rot)) rot = 0;

        return rot;
    }
}
