using UnityEngine;

public class WaveScreenAnimScript : MonoBehaviour
{
    float stopWatch = 0;

    void Update()
    {
        stopWatch += Time.deltaTime;
        if(stopWatch > 1)
        {
            stopWatch = 0;
            gameObject.SetActive(false);
        }
    }
}
