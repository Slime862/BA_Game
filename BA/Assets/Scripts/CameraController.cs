using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("¸ú×ÙÄ¿±ê")]
    public Transform target;
    public Vector3 offset;


    private void LateUpdate()
    {
        if (target == null) return;
        transform.position = target.position-offset;
    }

    
}
