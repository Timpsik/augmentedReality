using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PortalControl : MonoBehaviour
{
    public Material[] materials;

    void Start()
    {
        foreach (var mat in materials)
        {
            mat.SetInt("_StencilTest", (int)CompareFunction.Equal);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.name != "AR Camera")
        {
            return;
        }

        if (transform.position.z > other.transform.position.z)
        {
            Debug.Log("Outside portal");
            foreach (var mat in materials)
            {
                mat.SetInt("_StencilTest", (int)CompareFunction.Equal);
            }
        }
        else
        {
            Debug.Log("Inside portal");
            foreach (var mat in materials)
            {
                mat.SetInt("_StencilTest", (int)CompareFunction.NotEqual);
            }
        }

    }

}
