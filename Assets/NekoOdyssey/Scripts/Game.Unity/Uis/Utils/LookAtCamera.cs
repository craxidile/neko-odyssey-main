using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public bool opposite;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var cam = Camera.main;

        var direction = cam.transform.forward;

        if (opposite)
        {
            direction = -direction;
        }

        transform.forward = direction;
    }
}
