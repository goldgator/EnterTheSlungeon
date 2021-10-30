using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixScale : MonoBehaviour
{
    [SerializeField]
    private Transform overloadTransform;

    private void Start()
    {
        if (overloadTransform == null) overloadTransform = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent == null) return;

        Vector3 newScale = new Vector3();
        newScale.x = 1f / overloadTransform.localScale.x;
        newScale.y = 1f / overloadTransform.localScale.y;
        newScale.z = 1f / overloadTransform.localScale.z;

        transform.localScale = newScale;
    }

    public void SetOverloadTransform(Transform newTransform)
    {
        overloadTransform = newTransform;
    }
}
