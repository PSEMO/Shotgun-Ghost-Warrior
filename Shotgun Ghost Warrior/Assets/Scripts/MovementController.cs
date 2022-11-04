using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour
{
    private GameObject TracerObject;

    float ReloadCoolDown = 1;
    float ReloadStopWatch;
    int BulletNumber = 2;
    int Gauge = 12;

    int BulletRange = 35;

    CapsuleCollider2D capsuleCollider2D;

    float Vertical = 0;
    float Horizontal = 0;

    float Speed = 10;
    float DashMultiplier = 5;

    Camera MainCam;

    Vector3 MouseDirection;

    Transform Legs;

    readonly int StealthDuration = 2;
    readonly int StealthCoolDown = 7;
    float StealthStopWatch = 0;
    bool CanStealth = true;
    [HideInInspector] public bool isStealthing = false;

    readonly float DashDuration = 0.15f;
    readonly int DashCoolDown = 4;
    float DashStopWatch = 0;
    bool CanDash = true;
    [HideInInspector] public bool isDashing = false;

    float ShakeStopWatch = 0;
    readonly float ShakeDuration = 0.2f;
    bool Shake = false;

    bool CanMove = true;
    bool isUntouchable = false;

    Color DefaultCoolDownColor;

    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;

    GameObject Canvas;
    GameObject DashCoolDownUI;
    GameObject DashUIBlock;
    GameObject AutoAttack;
    GameObject AutoAttackCoolDownUI;
    GameObject StealthUIBlock;
    GameObject StealthCoolDownUI;

    GameObject Shotgun;
    Transform BarrelStartPos;
    Transform BarrelTopPos;
    Transform BarrelBotPos;
    Transform BarrelFirePos;

    ParticleSystem Particles;

    GameObject MiniMapCam;

    Animator PlayerAnimator;

    GameObject FireAnim;
    GameObject ActiveFireAnim;

    int LimitY = 31;
    int LimitX = 63;

    private void Awake()
    {
        Time.timeScale = 1;
    }

    void Start()
    {
        Canvas = GameObject.Find("Canvas");

        MiniMapCam = GameObject.Find("Minimap Camera");

        AutoAttack = Canvas.transform.Find("AutoAttack").gameObject;
        AutoAttackCoolDownUI = AutoAttack.transform.Find("AutoAttackCooldown").gameObject;
        StealthUIBlock = Canvas.transform.Find("StealthSkill").gameObject;
        StealthCoolDownUI = StealthUIBlock.transform.Find("GskillCooldown").gameObject;

        DashUIBlock = Canvas.transform.Find("DashSkill").gameObject;
        DashCoolDownUI = DashUIBlock.transform.Find("DashSkillCooldown").gameObject;

        Shotgun = transform.Find("Shotgun").gameObject;

        BarrelStartPos = Shotgun.transform.Find("BarrelStart");
        BarrelTopPos = Shotgun.transform.Find("BarrelLimitTop");
        BarrelBotPos = Shotgun.transform.Find("BarrelLimitBot");
        BarrelFirePos = Shotgun.transform.Find("BarrelFirePos");

        Particles = Shotgun.transform.Find("Gun Shell Particles").gameObject.GetComponent<ParticleSystem>();
        TracerObject = Resources.Load("Tracer") as GameObject;
        FireAnim = Resources.Load("Fire") as GameObject;

        MainCam = Camera.main;

        PlayerAnimator = GetComponent<Animator>();
        capsuleCollider2D = gameObject.GetComponent<CapsuleCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        Legs = transform.Find("Legs");

        DefaultCoolDownColor = Color.gray;
    }

    void Update()
    {
        #region Leg animation
        if (rb.velocity.magnitude > 0)
            Legs.gameObject.SetActive(true);
        else
            Legs.gameObject.SetActive(false);
        #endregion

        #region Movement
        if (CanMove)
        {
            if (Input.GetKey(KeyCode.D))
            {
                if (Input.GetKey(KeyCode.A))
                    Vertical = 0;
                else
                    Vertical = Speed;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                Vertical = -Speed;
            }
            else
            {
                Vertical = 0;
            }

            if (Input.GetKey(KeyCode.W))
            {
                if (Input.GetKey(KeyCode.S))
                    Horizontal = 0;
                else
                    Horizontal = Speed;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                Horizontal = -Speed;
            }
            else
            {
                Horizontal = 0;
            }

            rb.velocity = new Vector3(Vertical, Horizontal, 0);
        }
        else //Dashing
        {
            rb.velocity = new Vector3(Vertical, Horizontal, 0) * DashMultiplier;
        }

        //Position limiters
        {
            if (transform.position.x > LimitX)
                transform.position = new Vector3(LimitX, transform.position.y, 0);
            else if (transform.position.x < -LimitX)
                transform.position = new Vector3(-LimitX, transform.position.y, 0);

            if (transform.position.y > LimitY)
                transform.position = new Vector3(transform.position.x, LimitY, 0);
            else if (transform.position.y < -LimitY)
                transform.position = new Vector3(transform.position.x, -LimitY, 0);
        }
        #endregion

        Vector3 mousePosition = MainCam.ScreenToWorldPoint(Input.mousePosition);
        MouseDirection = mousePosition - transform.position;

        #region Rotate
        float angle = AngleBetweenTwoPoints(transform.position, mousePosition);
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle - 90));
        #endregion

        #region Stealth Mod
        if (Input.GetKeyDown(KeyCode.LeftShift))
            StealthMod();

        if (!CanStealth)
        {
            StealthStopWatch += Time.deltaTime;

            float RemainingTime = StealthCoolDown - StealthStopWatch;
            StealthCoolDownUI.GetComponent<Text>().text = RemainingTime.ToString("f1");

            if (StealthStopWatch > StealthDuration)
            {
                StealthModCancel();
            }
            if (StealthStopWatch > StealthCoolDown)
            {
                StealthUIBlock.GetComponent<RawImage>().color = Color.white;
                CanStealth = true;
                StealthStopWatch = 0;

                StealthCoolDownUI.GetComponent<Text>().text = "";
            }
        }
        #endregion

        #region Dash
        if (Input.GetKeyDown(KeyCode.Space))
            Dash();

        if (!CanDash)
        {
            DashStopWatch += Time.deltaTime;

            float RemainingTime = DashCoolDown - DashStopWatch;
            DashCoolDownUI.GetComponent<Text>().text = RemainingTime.ToString("f1");

            if (DashStopWatch > DashDuration)
            {
                DashCancel();
            }
            if (DashStopWatch > DashCoolDown)
            {
                DashUIBlock.GetComponent<RawImage>().color = Color.white;
                CanDash = true;
                DashStopWatch = 0;

                DashCoolDownUI.GetComponent<Text>().text = "";
            }
        }
        #endregion

        #region Move/Shake Camera and ReFire

        MiniMapCam.transform.position = transform.position +
            (new Vector3(0, 0, -10) + (MouseDirection / 2));

        if (!Shake)
        {
            Vector3 CamPos = transform.position + (MouseDirection / 3.5f);
            MainCam.transform.position = new Vector3(CamPos.x, CamPos.y, -10);

            AutoAttackCoolDownUI.GetComponent<Text>().text = "";
        }
        else //Shaking
        {
            Vector3 CamPos = transform.position + (MouseDirection / 4f);
            MainCam.transform.position = new Vector3(CamPos.x, CamPos.y, -10) +
                new Vector3(Random.Range(-0.45f, 0.45f), Random.Range(-0.45f, 0.45f), 0);

            transform.position += Time.deltaTime * -6f * MouseDirection.normalized;

            float ColorRatio = ShakeStopWatch / ShakeDuration;
            AutoAttack.GetComponent<RawImage>().color = new Color(ColorRatio, ColorRatio, ColorRatio);

            ShakeStopWatch += Time.deltaTime;
            if (ShakeStopWatch > ShakeDuration)
            {
                ShakeStopWatch = 0;
                Shake = false;
            }

            AutoAttackCoolDownUI.GetComponent<Text>().text = (ShakeDuration - ShakeStopWatch).ToString("f2").Remove(0, 2) + "<size=75>ms</size>";
        }
        #endregion

        #region Fire!
        if (Input.GetMouseButtonDown(0) && !Shake && BulletNumber > 0)
        {
            if (ActiveFireAnim != null)
                Destroy(ActiveFireAnim);


            ActiveFireAnim = Instantiate(FireAnim, BarrelFirePos.position, transform.rotation);

            BulletNumber--;

            Shake = true;
            ShakeStopWatch = 0;

            for (int i = 0; i < Gauge; i++)
            {
                Vector2 Direction;
                switch (i)
                {
                    case 0:
                        Vector2 midBarrel = (BarrelTopPos.position + BarrelBotPos.position) / 2;
                        Direction = midBarrel - (Vector2)BarrelStartPos.position;
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

                int LayerMask = 153;//10011001 = 153 //1000101 = 81

                RaycastHit2D hit;
                hit = Physics2D.Raycast(transform.position, Direction, BulletRange, LayerMask);

                if (hit)
                {
                    if (hit.transform.CompareTag("enemy"))
                    {
                        Color HitObjColor = hit.transform.gameObject.GetComponent<SpriteRenderer>().color;
                        HitObjColor.a = HitObjColor.a / 2;
                        hit.transform.gameObject.GetComponent<SpriteRenderer>().color = HitObjColor;

                        hit.transform.gameObject.GetComponent<EnemyMover>().killYourself();
                    }
                    CreateTracer(BarrelStartPos.position, hit.point);
                }
                else
                {
                    CreateTracer(BarrelStartPos.position, (Vector2)BarrelStartPos.position + (Direction * BulletRange));
                }
            }

            Particles.Play();
        }
        #endregion

        #region Reload
        if (BulletNumber == 0)
        {
            PlayerAnimator.SetBool("isReloading", true);

            float ColorRatio = ReloadStopWatch / ReloadCoolDown;
            AutoAttack.GetComponent<RawImage>().color = new Color(ColorRatio, ColorRatio, ColorRatio);

            //start.reload anim
            ReloadStopWatch += Time.deltaTime;
            if (ReloadStopWatch > ReloadCoolDown)
            {
                ReloadStopWatch = 0;
                BulletNumber = 2;

                PlayerAnimator.SetBool("isReloading", false);
            }

            AutoAttackCoolDownUI.GetComponent<Text>().text = (ReloadCoolDown - ReloadStopWatch).ToString("f2").Remove(0, 2) + "<size=75>ms</size>";
        }
        #endregion
    }

    //Creates a sprite tracer between two given points
    void CreateTracer(Vector2 FirstPos, Vector2 SecondPos)
    {
        float AngularZ = AngleBetweenTwoPoints(FirstPos, SecondPos);
        GameObject Tracer = Instantiate(TracerObject, (FirstPos + SecondPos) / 2, Quaternion.Euler(0, 0, AngularZ));
        Tracer.transform.localScale = new Vector2(Vector2.Distance(FirstPos, SecondPos), 0.025f);
    }

    //Calculates the angle between two vector2 points in degrees.
    float AngleBetweenTwoPoints(Vector2 a, Vector2 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    //Activates dash's boost, color change and activates relevant cooldown.
    public void Dash()
    {
        if (CanDash)
        {
            CanMove = false;

            isDashing = true;
            UntouchableManager();

            DashUIBlock.GetComponent<RawImage>().color = DefaultCoolDownColor;
            CanDash = false;
        }
    }
    //Cancels dash's boost.
    void DashCancel()
    {
        isDashing = false;
        UntouchableManager();

        CanMove = true;
    }

    //Activates Stealth mod, color change and activates relevant cooldown.
    public void StealthMod()
    {
        if (CanStealth)
        {
            StealthUIBlock.GetComponent<RawImage>().color = DefaultCoolDownColor;
            CanStealth = false;

            isStealthing = true;
            UntouchableManager();

            Color TempForTransparant = Color.white;
            TempForTransparant.a = 0.35f;
            GetComponent<SpriteRenderer>().color = TempForTransparant;
            Legs.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = TempForTransparant;
            Legs.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = TempForTransparant;

            TempForTransparant = Color.black;
            TempForTransparant.a = 0.35f;
        }
    }
    //Cancels stealth colors.
    void StealthModCancel()
    {
        isStealthing = false;
        UntouchableManager();

        GetComponent<SpriteRenderer>().color = Color.white;
        Legs.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        Legs.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    //is called when stealth or dash state has been changed.
    void UntouchableManager()
    {
        if (isStealthing || isDashing)
            isUntouchable = true;
        else
            isUntouchable = false;
        //the roads we cross to avoid putting few '{}'...
        if (isUntouchable == true)
            capsuleCollider2D.isTrigger = true;
        else
            capsuleCollider2D.isTrigger = false;
    }
}