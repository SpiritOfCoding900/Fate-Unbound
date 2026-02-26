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

    [Header("Player's Attack Settings")]
    public float attackSpeedRate = 0.15f;   // minimum time between clicks
    public float comboResetTime = 0.9f;     // if you wait too long, combo resets
    public bool lockMovementWhileAttacking = true;

    private float timeSinceAttack = 999f;
    private int currentAttack = 0;
    private bool isAttacking = false;
    private bool queuedNextAttack = false;
    private int queuedAttackNumber = 0;

    [Header("Player's Roll Settings")]
    public float rollImpulse = 8f;      // tweak this
    public float rollDuration = 0.25f;  // tweak this
    public float rollCooldown = 0.35f;  // tweak this
    public bool invincibleWhileRolling = true;

    private bool isRolling = false;
    private float rollCooldownTimer = 0f;
    private Vector2 rollDir = Vector2.down;

    ///This is for the animation - Joycelyn
    public Animator anim;

    public string[] attack1Directions = {
    "Paladin_Attack1_N",
    "Paladin_Attack1_NE",
    "Paladin_Attack1_E",
    "Paladin_Attack1_SE",
    "Paladin_Attack1_S",
    "Paladin_Attack1_SW",
    "Paladin_Attack1_W",
    "Paladin_Attack1_NW" };

    public string[] attack2Directions = {
    "Paladin_Attack2_N",
    "Paladin_Attack2_NE",
    "Paladin_Attack2_E",
    "Paladin_Attack2_SE",
    "Paladin_Attack2_S",
    "Paladin_Attack2_SW",
    "Paladin_Attack2_W",
    "Paladin_Attack2_NW" };

    public string[] rollDirections = {
    "Paladin_Roll_N",
    "Paladin_Roll_NE",
    "Paladin_Roll_E",
    "Paladin_Roll_SE",
    "Paladin_Roll_S",
    "Paladin_Roll_SW",
    "Paladin_Roll_W",
    "Paladin_Roll_NW" };
    
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
        Roll();
        Attack();
        PlayerInvincibility();

        if (rollCooldownTimer > 0f)
            rollCooldownTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (!isRolling && !(lockMovementWhileAttacking && isAttacking))
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
        if (isRolling || isAttacking) return;
        string[] directionArray = null;

        if(_direction.magnitude < 0.01)
        {
            directionArray = staticDirections;
        }
        else
        {
            directionArray = runDirections;
            lastDirection = DirectionToIndex(_direction);
            lastMovedVector = _direction.normalized;
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

    float GetClipLength(string clipName)
    {
        foreach (var clip in anim.runtimeAnimatorController.animationClips)
            if (clip.name == clipName) return clip.length;

        return 0.2f; // fallback
    }

    void Roll()
    {
        if (isRolling) return;
        if (rollCooldownTimer > 0f) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Choose roll direction:
            // If player is pressing a direction, roll that way.
            // Otherwise roll toward last moved direction.
            Vector2 inputDir = moveDir;
            rollDir = (inputDir.sqrMagnitude > 0.01f) ? inputDir.normalized : lastMovedVector.normalized;

            // Safety fallback (in case lastMovedVector is zero somehow)
            if (rollDir.sqrMagnitude < 0.01f)
                rollDir = Vector2.down;

            StartCoroutine(RollRoutine());
        }
    }

    IEnumerator RollRoutine()
    {
        isRolling = true;
        rollCooldownTimer = rollCooldown;

        // Cache + update lastMovedVector so your facing is correct after roll
        lastMovedVector = rollDir;
        lastDirection = DirectionToIndex(rollDir);

        // Play roll animation (8-direction)
        anim.Play(rollDirections[lastDirection]);

        // Optional: invincible while rolling
        bool prevInv = isInvincible;
        float prevInvTimer = invincibilityTimer;

        if (invincibleWhileRolling)
        {
            isInvincible = true;
            invincibilityTimer = rollDuration; // will tick down in PlayerInvincibility
        }

        // Clear current velocity so impulse feels crisp
        rb.linearVelocity = Vector2.zero;

        // AddForce impulse (instant push)
        rb.AddForce(rollDir * rollImpulse, ForceMode2D.Impulse);

        // Wait for the ACTUAL roll animation to finish
        float clipLen = GetClipLength(rollDirections[lastDirection]);

        // If animator speed isn't 1, account for it
        float animTime = clipLen / Mathf.Max(0.0001f, anim.speed);

        yield return new WaitForSeconds(animTime);

        // Stop momentum at the end (prevents sliding forever)
        rb.linearVelocity = Vector2.zero;

        // Restore invincibility state if you don't want roll to overwrite other i-frames
        if (!invincibleWhileRolling)
        {
            // do nothing
        }
        else
        {
            // If you prefer: keep whatever invincibility system decides based on timer
            // Leaving it alone is fine because PlayerInvincibility() handles timer.
        }

        isRolling = false;
    }

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;

        if (isRolling) return;

        // If we waited too long, reset combo
        if (timeSinceAttack > comboResetTime)
            currentAttack = 0;

        // Click?
        if (!Input.GetMouseButtonDown(0)) return;

        // If currently attacking, queue the next hit instead of playing immediately
        if (isAttacking)
        {
            // Only queue if within combo window (optional)
            if (timeSinceAttack <= comboResetTime)
            {
                queuedNextAttack = true;
                queuedAttackNumber = Mathf.Clamp(currentAttack + 1, 1, 2); // next is 2 if current is 1
                if (queuedAttackNumber > 2) queuedAttackNumber = 1;
            }
            return;
        }

        // Enforce minimum time between starting attacks
        if (timeSinceAttack < attackSpeedRate) return;

        // Start the next attack
        currentAttack++;
        if (currentAttack > 2) currentAttack = 1;

        StartCoroutine(AttackRoutine(currentAttack));
        timeSinceAttack = 0f;
    }

    IEnumerator AttackRoutine(int attackNumber)
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero; // stop immediately

        // Choose facing direction when attack starts
        Vector2 facing = (moveDir.sqrMagnitude > 0.01f) ? moveDir : lastMovedVector;
        if (facing.sqrMagnitude < 0.01f) facing = Vector2.down;

        lastMovedVector = facing.normalized;
        lastDirection = DirectionToIndex(lastMovedVector);

        // Pick clip
        string clipName = (attackNumber == 1)
            ? attack1Directions[lastDirection]
            : attack2Directions[lastDirection];

        // Play clip
        anim.Play(clipName);

        // Wait for clip to finish
        float clipLen = GetClipLength(clipName);
        float animTime = clipLen / Mathf.Max(0.0001f, anim.speed);
        
        float t = 0f;
        while (t < animTime)
        {
            rb.linearVelocity = Vector2.zero; // ✅ keep frozen
            t += Time.deltaTime;
            yield return null;
        }

        // If player clicked during the attack, chain to next attack NOW
        if (queuedNextAttack)
        {
            queuedNextAttack = false;
            currentAttack = (attackNumber == 1) ? 2 : 1;
            timeSinceAttack = 0f;

            yield return StartCoroutine(AttackRoutine(currentAttack));
            yield break;
        }

        // Done attacking
        isAttacking = false;
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