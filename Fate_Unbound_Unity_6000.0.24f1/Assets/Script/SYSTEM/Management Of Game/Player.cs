using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SimpleSingleton<Player>
{
    [Header("Player's Current Stats: ")]
    public string className;

    public float MaxHP = 10;
    public float CurrentHP;

    public float MaxMP = 10;
    public float CurrentMP;

    public float ATK = 5;
    public float DEF = 5;

    public float dodgeRate = 0;
    public float moveSpeed = 10;

    [TextArea(3,5)]
    public string description;

    public int ID;

    [Header("Player's Invincibility: ")]
    private bool isInvincible = false;
    public float invincibilityDuration = 1.5f; // seconds
    private float invincibilityTimer;

    private Rigidbody2D rb;
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 moveDir;
    [HideInInspector]
    public Vector2 lastMovedVector;

    ///This is for the animation - Joycelyn
    public Animator anim;
    public string[] staticDirections = {
    "Paladin_Static_N",
    "Paladin_Static_NE",
    "Paladin_Static_E",
    "Paladin_Static_SE",
    "Paladin_Static_S",
    "Paladin_Static_SW",
    "Paladin_Static_W",
    "Paladin_Static_NW" };

    public string[] runDirections = {
    "Paladin_Run_N",
    "Paladin_Run_NE",
    "Paladin_Run_E",
    "Paladin_Run_SE",
    "Paladin_Run_S",
    "Paladin_Run_SW",
    "Paladin_Run_W",
    "Paladin_Run_NW" };

    int lastDirection;

    /// This is for checking which class is selected  - Joycelyn
    UIPlayerSelection playerSelection;
    Player player;


    private void Awake()
    {
        anim = GetComponent<Animator>();

        float result1 = Vector2.SignedAngle(Vector2.up, Vector2.right);
        print("R1 " + result1);
        float result2 = Vector2.SignedAngle(Vector2.up, Vector2.left);
        print("R1 " + result2);
        float result3 = Vector2.SignedAngle(Vector2.up, Vector2.down);
        print("R1 " + result3);
        //float result4 = Vector2.SignedAngle(Vector2.up, Vector2.right);
        //print("R1 " + result4);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(1, 0f);
        CurrentHP = MaxHP;
        CurrentMP = MaxMP;
    }

    void Update()
    {
        Inputmanagement();
        PlayerInvincibility();
    }

    void FixedUpdate()
    {
        Move();
    }

    ///To select the correct class sprites - Jsoycelyn
    //void ClassChecker()
    //{
    //    if (ID == 0)
    //    {
    //        anim.SetBool("isBarb", true);
    //        anim.SetBool("isRog", false);
    //        anim.SetBool("isAlc", false);
    //    }
    //    if (ID == 1)
    //    {

    //        anim.SetBool("isRog", true);
    //        anim.SetBool("isBarb", false);
    //        anim.SetBool("isAlc", false);
    //    }
    //    if (ID == 2)
    //    {
    //        anim.SetBool("isAlc", true);
    //        anim.SetBool("isBarb", false);
    //        anim.SetBool("isRog", false);
    //    }
    //}

    void Inputmanagement()
    {
        ///To select the correct class sprites - Jsoycelyn
        //ClassChecker();

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveX, moveY).normalized;
    }

    void PlayerInvincibility()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
                Debug.Log("Player is no longer invincible.");
            }
        }
    }

    public void SetDirection(Vector2 _direction)
    {
        string[] directionArray = null;

        if(_direction.magnitude < 0.01)
        {
            directionArray = staticDirections;
        }
        else
        {
            directionArray = runDirections;
            lastDirection = DirectionToIndex(_direction);
        }

        anim.Play(directionArray[lastDirection]);
    }

    private int DirectionToIndex(Vector2 _direction)
    {
        Vector2 norDir = _direction.normalized;
        float step = 360 / 8;
        float offset = step / 2;
        float angle = -Vector2.SignedAngle(Vector2.up, norDir);

        angle += offset;
        if (angle < 0)
        {
            angle += 360;
        }
        float stepCount = angle / step;
        return Mathf.FloorToInt(stepCount);
    }
    
    void Move()
    {
        float moveH = moveDir.x * moveSpeed;
        float moveV = moveDir.y * moveSpeed;
        rb.linearVelocity = new Vector2(moveH, moveV);
        Vector2 direction = new Vector2(moveH, moveV);
        SetDirection(direction);
    }

    void Roll()
    {

    }

    void Attack()
    {

    }

    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        if (CurrentHP <= 0)
        {
            Debug.Log("You're dead");
        }
        
        CurrentHP -= amount;

        // Activate temporary invincibility
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        Debug.Log("Player is now invincible.");
    }
}