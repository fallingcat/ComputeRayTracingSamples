using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointLight : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(200.0f * UnityEngine.Mathf.Cos(Time.time), 120.0f, 200.0f * UnityEngine.Mathf.Sin(Time.time));
    }
}
