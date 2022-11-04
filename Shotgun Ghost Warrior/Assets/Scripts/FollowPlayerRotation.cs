using UnityEngine;

public class FollowPlayerRotation : MonoBehaviour
{
    ParticleSystem m_ParticleSystem;
    Transform player;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        m_ParticleSystem = GetComponent<ParticleSystem>();
    }

    [System.Obsolete]
    void FixedUpdate()
    {
        m_ParticleSystem.startRotation = player.rotation.eulerAngles.z;
    }
}
