using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeChildOf : MonoBehaviour
{
    public Transform futurParent;

    private void Start()
    {
        if(futurParent != null)
            transform.SetParent(futurParent);
    }
}
