using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    private GameObject TracerObject;

    [HideInInspector] public Vector3 Offset = Vector3.zero;
    Vector3 TargetPosition;

    Transform BarrelTopPos;
    Transform BarrelBotPos;
    Transform BarrelStartPos;
    int Gauge = 6;

    float speed = 2;

    ParticleSystem Particles;
    Rigidbody2D rb;
    Transform player;

    MovementController playerMControllerScript;

    int LayerMask = 209;//10011001 = 153 //10001011 = 209
    Vector2 midBarrel;
    Vector2 Direction;

    float StopWatch = 3;

    GameObject GhostDying;

    EnemyManager EnemyHolder;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").transform;
        playerMControllerScript = player.GetComponent<MovementController>();
        EnemyHolder = GameObject.Find("EnemyHolder").gameObject.GetComponent<EnemyManager>();


        Transform Shotgun = transform.Find("Shotgun");

        BarrelStartPos = Shotgun.transform.Find("BarrelStart");
        BarrelTopPos = Shotgun.transform.Find("BarrelLimitTop");
        BarrelBotPos = Shotgun.transform.Find("BarrelLimitBot");
        Particles = Shotgun.Find("Gun Shell Particles").gameObject.GetComponent<ParticleSystem>();

        GhostDying = Resources.Load("GhostDying") as GameObject;
        TracerObject = Resources.Load("Tracer") as GameObject;
    }

    void Update()
    {
        if (!playerMControllerScript.isStealthing)
            TargetPosition = player.position + Offset;

        //Move to target
        rb.velocity = (TargetPosition - transform.position).normalized * speed;

        //Rotate to target
        float AngleZ = AngleBetweenTwoPoints(transform.position, TargetPosition);
        transform.rotation =
            Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, AngleZ - 90), 0.5f);

        //Start checking visual contact when close to target
        if (Vector2.Distance(transform.position, TargetPosition) < 4)
        {
            StopWatch += Time.deltaTime;
            if (StopWatch > 2)//shoot every two second
            {
                midBarrel = (BarrelTopPos.position + BarrelBotPos.position) / 2;
                Direction = midBarrel - (Vector2)BarrelStartPos.position;

                RaycastHit2D hit = Physics2D.Raycast(transform.position, Direction, 5, LayerMask);

                if (hit)
                {
                    if (hit.transform.name == "Player")
                    {
                        StopWatch = 0;
                        ShootToKill();
                    }
                }
                else
                {
                    StopWatch = 0;
                    ShootToKill();
                }
            }
        }
    }

    float AngleBetweenTwoPoints(Vector2 a, Vector2 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    public void killYourself()
    {
        EnemyHolder.EnemyDied();
        Instantiate(GhostDying, transform.position, transform.rotation);
        player.GetComponent<HpHolder>().EnemyKilled++;
        Destroy(gameObject);
    }

    void ShootToKill()
    {
        for (int i = 0; i < Gauge; i++)
        {
            switch (i)
            {
                case 0:
                    //Direction = Direction;
                    break;
                case 1:
                    Direction = BarrelTopPos.position - BarrelStartPos.position;
                    break;
                case 2:
                    Direction = BarrelBotPos.position - BarrelStartPos.position;
                    break;
                default:
                    Direction = new Vector2(
                        Random.Range(BarrelTopPos.position.x, BarrelBotPos.position.x),
                        Random.Range(BarrelTopPos.position.y, BarrelBotPos.position.y)) -
                        (Vector2)BarrelStartPos.position;
                    break;
            }

            RaycastHit2D hit;
            hit = Physics2D.Raycast(transform.position, Direction, 5, LayerMask);

            if (hit)
            {
                if (hit.transform.name == "Player")
                {
                    if (playerMControllerScript.isDashing)
                    {
                        CreateTracer(BarrelStartPos.position, (Vector2)BarrelStartPos.position + (Direction * 20));
                    }
                    else
                    {
                        hit.transform.gameObject.GetComponent<HpHolder>().ChangeHp(-0.01f);
                        CreateTracer(BarrelStartPos.position, hit.point);
                    }
                }
            }
            else
            {
                CreateTracer(BarrelStartPos.position, (Vector2)BarrelStartPos.position + (Direction * 20));
            }
        }

        Particles.Play();
    }

    //Creates a sprite tracer between two given points
    void CreateTracer(Vector2 FirstPos, Vector2 SecondPos)
    {
        float AngularZ = AngleBetweenTwoPoints(FirstPos, SecondPos);
        GameObject Tracer = Instantiate(TracerObject, (FirstPos + SecondPos) / 2, Quaternion.Euler(0, 0, AngularZ));
        Tracer.transform.localScale = new Vector2(Vector2.Distance(FirstPos, SecondPos), 0.025f);
    }
}