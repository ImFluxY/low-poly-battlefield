using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTranform : MonoBehaviour
{
    [SerializeField]
    private Transform copyThis;

    private void Update()
    {
        if(copyThis)
        {
            transform.position = copyThis.position;
            transform.rotation = copyThis.rotation;
        }
    }
}
