using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixScale : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        Vector3 newScale = new Vector3();
        newScale.x = 1f / transform.parent.localScale.x;
        newScale.y = 1f / transform.parent.localScale.y;
        newScale.z = 1f / transform.parent.localScale.z;

        transform.localScale = newScale;
    }
}
