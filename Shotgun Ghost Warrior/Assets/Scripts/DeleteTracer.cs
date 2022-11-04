using UnityEngine;

public class DeleteTracer : MonoBehaviour
{
    float DeathTimer = 0;
    SpriteRenderer spriteRenderer;
    Color RayColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        RayColor = spriteRenderer.color;
    }

    //Tracer do fade away and gets deleted eventually
    void Update()
    {
        DeathTimer += Time.deltaTime;

        Color a = RayColor;
        a.a = 1 - (DeathTimer / 0.6f);
        spriteRenderer.color = a;

        if (DeathTimer > 0.6f)
            Destroy(gameObject);
    }
}