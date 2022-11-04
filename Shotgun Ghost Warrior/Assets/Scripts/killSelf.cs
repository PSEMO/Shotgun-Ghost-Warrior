using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killSelf : MonoBehaviour
{
    float sw = 0;

    // Update is called once per frame
    void Update()
    {
        sw += Time.deltaTime;
        if(sw > 0.8f)
        {
            Destroy(gameObject);
        }
    }
}
