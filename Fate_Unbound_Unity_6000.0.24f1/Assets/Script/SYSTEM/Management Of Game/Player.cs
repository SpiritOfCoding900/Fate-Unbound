using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SimpleSingleton<Player>
{
    // =========================
    // FSM
    // =========================
    public enum PlayerState
    {
        Idle,
        Move,
        Roll,
        Attack
    }

    [Header("FSM")]
    [SerializeField] private PlayerState state = PlayerState.Idle;

    // =========================
    // Stats
    // =========================
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

    [TextArea(3, 5)]
    public string description;

    public int ID;

    // =========================
    // Invincibility
    // =========================
    [Header("Player's Invincibility: ")]
    private bool isInvincible = false;
    public float invincibilityDuration = 1.5f;
    private float invincibilityTimer;

    // =========================
    // Movement
    // =========================
    private Rigidbody2D rb;

    [HideInInspector] public Vector2 moveDir;
    [HideInInspector] public Vector2 lastMovedVector;

    // =========================
    // Attack
    // =========================
    [Header("Player's Attack Settings")]
    public float attackSpeedRate = 0.15f;   // minimum time between starting attacks
    public float comboResetTime = 0.9f;     // if you wait too long, combo resets

    private float timeSinceAttack = 999f;
    private int currentAttack = 0;          // 0 = none, 1 = Attack1, 2 = Attack2
    private bool queuedNextAttack = false;  // click buffer during Attack1

    // =========================
    // Attack Range Direction
    // =========================
    [SerializeField] private Transform attackRange;          // drag your AttackRange here
    [SerializeField] private Transform[] attackSockets = new Transform[8]; // N,NE,E,SE,S,SW,W,NW
    [SerializeField] private bool rotateAttackRangeToSocket = true; // optional
    private int facingIndex = 4; // 0=N,1=NE,2=E,3=SE,4=S,5=SW,6=W,7=NW (default South)

    // =========================
    // Smash (ISmashable)
    // =========================
    [Header("Smash (ISmashable)")]
    [SerializeField] private bool smashClosestTarget = true;

    private readonly List<ISmashable> smashablesInRange = new List<ISmashable>();
    private readonly List<Collider2D> smashableCollidersInRange = new List<Collider2D>();

    // =========================
    // Roll
    // =========================
    [Header("Player's Roll Settings")]
    public float rollImpulse = 8f;
    public float rollCooldown = 0.35f;
    public bool invincibleWhileRolling = true;

    [Header("Roll Over Objects (IRollable)")]
    [SerializeField] private Collider2D playerSolidCollider; // your main NON-trigger collider
    private readonly HashSet<Collider2D> ignoredThisRoll = new HashSet<Collider2D>();

    private float rollCooldownTimer = 0f;
    private Vector2 rollDir = Vector2.down;


    private readonly List<IRollable> rollablesInRange = new List<IRollable>();
    private readonly List<Collider2D> rollableCollidersInRange = new List<Collider2D>();

    // =========================
    // Animation
    // =========================
    [Header("Animation")]
    public Animator anim;

    public string[] attack1Directions = {
        "Paladin_Attack1_N",
        "Paladin_Attack1_NE",
        "Paladin_Attack1_E",
        "Paladin_Attack1_SE",
        "Paladin_Attack1_S",
        "Paladin_Attack1_SW",
        "Paladin_Attack1_W",
        "Paladin_Attack1_NW"
    };
    public string[] attack2Directions = {
        "Paladin_Attack2_N",
        "Paladin_Attack2_NE",
        "Paladin_Attack2_E",
        "Paladin_Attack2_SE",
        "Paladin_Attack2_S",
        "Paladin_Attack2_SW",
        "Paladin_Attack2_W",
        "Paladin_Attack2_NW"
    };
    public string[] rollDirections = {
        "Paladin_Roll_N",
        "Paladin_Roll_NE",
        "Paladin_Roll_E",
        "Paladin_Roll_SE",
        "Paladin_Roll_S",
        "Paladin_Roll_SW",
        "Paladin_Roll_W",
        "Paladin_Roll_NW"
    };
    public string[] staticDirections = {
        "Paladin_Static_N",
        "Paladin_Static_NE",
        "Paladin_Static_E",
        "Paladin_Static_SE",
        "Paladin_Static_S",
        "Paladin_Static_SW",
        "Paladin_Static_W",
        "Paladin_Static_NW"
    };
    public string[] runDirections = {
        "Paladin_Run_N",
        "Paladin_Run_NE",
        "Paladin_Run_E",
        "Paladin_Run_SE",
        "Paladin_Run_S",
        "Paladin_Run_SW",
        "Paladin_Run_W",
        "Paladin_Run_NW"
    };
    private int lastDirection = 4; // default South-ish index if you want

    // =========================
    // Unity
    // =========================
    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (playerSolidCollider == null)
            playerSolidCollider = GetComponent<Collider2D>();

        if (playerSolidCollider == null)          Debug.LogError("Player: No Collider2D found on the Player root.");
        else if (playerSolidCollider.isTrigger)   Debug.LogError("Player: playerSolidCollider is a trigger. It should be a NON-trigger collider.");
    }

    void Start()
    {
        // Default facing (change if you want)
        lastMovedVector = new Vector2(0f, -1f);

        CurrentHP = MaxHP;
        CurrentMP = MaxMP;

        state = PlayerState.Idle;

        facingIndex = 4; // South
        if (attackRange != null && attackSockets != null && attackSockets.Length >= 8 && attackSockets[4] != null)
        {
            attackRange.position = attackSockets[4].position;
            if (rotateAttackRangeToSocket) attackRange.rotation = attackSockets[4].rotation;
        }
    }

    private void OnDisable()
    {
        EndRollIgnore();
    }

    void Update()
    {
        ReadMoveInput();
        UpdateAttackRangeSocket();
        PlayerInvincibility();

        // Timers
        timeSinceAttack += Time.deltaTime;
        if (rollCooldownTimer > 0f) rollCooldownTimer -= Time.deltaTime;

        // High priority inputs (only if not already doing an exclusive action)
        HandleRollInput();
        HandleAttackInput();

        // If we're in an action state, don't auto-switch to Move/Idle
        if (state == PlayerState.Roll || state == PlayerState.Attack)
            return;

        // Locomotion state update
        state = (moveDir.sqrMagnitude > 0.01f) ? PlayerState.Move : PlayerState.Idle;
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case PlayerState.Move:
                DoMove();
                break;

            case PlayerState.Idle:
                // Stop drift and play idle facing
                rb.linearVelocity = Vector2.zero;
                PlayLocomotionAnim(Vector2.zero);
                break;

            case PlayerState.Roll:
            case PlayerState.Attack:
                // Action coroutines control motion/animation
                break;
        }
    }

    // =========================
    // Input
    // =========================
    void ReadMoveInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDir = new Vector2(moveX, moveY).normalized;
    }

    // =========================
    // Locomotion
    // =========================
    void DoMove()
    {
        Vector2 vel = moveDir * moveSpeed;
        rb.linearVelocity = vel;
        PlayLocomotionAnim(vel);
    }

    void PlayLocomotionAnim(Vector2 velocity)
    {
        string[] directionArray;

        if (velocity.sqrMagnitude < 0.01f)
        {
            directionArray = staticDirections;
        }
        else
        {
            directionArray = runDirections;
            lastDirection = DirectionToIndex(velocity);
            lastMovedVector = velocity.normalized;
        }

        anim.Play(directionArray[lastDirection]);
    }

    // =========================
    // Roll
    // =========================
    void HandleRollInput()
    {
        if (state == PlayerState.Roll) return;
        if (state == PlayerState.Attack) return; // no roll during attack
        if (rollCooldownTimer > 0f) return;

        if (!Input.GetKeyDown(KeyCode.Space)) return;

        // Roll direction: input direction if any, else last facing
        rollDir = (moveDir.sqrMagnitude > 0.01f) ? moveDir.normalized : lastMovedVector.normalized;
        if (rollDir.sqrMagnitude < 0.01f) rollDir = Vector2.down;

        StartCoroutine(RollRoutineFSM());
    }

    IEnumerator RollRoutineFSM()
    {
        state = PlayerState.Roll;
        rollCooldownTimer = rollCooldown;

        lastMovedVector = rollDir;
        lastDirection = DirectionToIndex(rollDir);

        string rollClip = rollDirections[lastDirection];
        anim.Play(rollClip);

        if (invincibleWhileRolling)
        {
            isInvincible = true;
            invincibilityTimer = GetClipLength(rollClip);
        }

        rb.linearVelocity = Vector2.zero;

        BeginRollIgnore();

        try
        {
            rb.AddForce(rollDir * rollImpulse, ForceMode2D.Impulse);

            float animTime = GetClipLength(rollClip) / Mathf.Max(0.0001f, anim.speed);
            yield return new WaitForSeconds(animTime);

            rb.linearVelocity = Vector2.zero;
        }
        finally
        {
            EndRollIgnore(); // Always restores collisions
        }

        state = (moveDir.sqrMagnitude > 0.01f) ? PlayerState.Move : PlayerState.Idle;
    }

    public void RegisterRollable(Collider2D other)
    {
        // IRollable might be on the collider object OR its parent
        IRollable rollable = other.GetComponent<IRollable>() ?? other.GetComponentInParent<IRollable>();
        if (rollable == null) return;

        if (!rollableCollidersInRange.Contains(other))
        {
            rollableCollidersInRange.Add(other);
            rollablesInRange.Add(rollable);
        }
    }

    public void UnregisterRollable(Collider2D other)
    {
        int index = rollableCollidersInRange.IndexOf(other);
        if (index >= 0)
        {
            rollableCollidersInRange.RemoveAt(index);
            rollablesInRange.RemoveAt(index);
        }
    }

    private void BeginRollIgnore()
    {
        if (playerSolidCollider == null) return;

        ignoredThisRoll.Clear();

        // Ignore everything currently in range and REMEMBER it
        for (int i = 0; i < rollableCollidersInRange.Count; i++)
        {
            var c = rollableCollidersInRange[i];
            if (c == null) continue;

            Physics2D.IgnoreCollision(playerSolidCollider, c, true);
            ignoredThisRoll.Add(c);
        }

        // Let rollables react (splash, sound, etc)
        CallRollables();
    }
    private void EndRollIgnore()
    {
        if (playerSolidCollider == null) return;

        // Restore EVERYTHING we ignored, even if it left range mid-roll
        foreach (var c in ignoredThisRoll)
        {
            if (c == null) continue;
            Physics2D.IgnoreCollision(playerSolidCollider, c, false);
        }

        ignoredThisRoll.Clear();
    }

    private void CallRollables()
    {
        for (int i = 0; i < rollablesInRange.Count; i++)
            rollablesInRange[i]?.RollOverObject();
    }

    // =========================
    // Attack Range Socket
    // =========================
    private void UpdateAttackRangeSocket()
    {
        if (attackRange == null) return;
        if (attackSockets == null || attackSockets.Length < 8) return;

        // Pick facing from current move input, else last moved direction
        Vector2 dir = (moveDir.sqrMagnitude > 0.01f) ? moveDir : lastMovedVector;
        if (dir.sqrMagnitude < 0.01f) dir = Vector2.down;

        int newIndex = DirectionToIndex(dir);

        // If you want: only update when direction changes
        if (newIndex == facingIndex) return;
        facingIndex = newIndex;

        Transform socket = attackSockets[facingIndex];
        if (socket == null) return;

        // Snap AttackRange to socket position
        attackRange.position = socket.position;

        // Optional: match rotation (usually not needed for circle, useful for box)
        if (rotateAttackRangeToSocket)
            attackRange.rotation = socket.rotation;
    }

    // =========================
    // Smash Object
    // =========================
    public void RegisterSmashable(Collider2D other)
    {
        // ISmashable might be on collider object OR parent
        ISmashable smashable = other.GetComponent<ISmashable>() ?? other.GetComponentInParent<ISmashable>();
        if (smashable == null) return;

        // Optional: ignore trigger colliders (usually you want the solid one)
        if (other.isTrigger) return;

        if (!smashableCollidersInRange.Contains(other))
        {
            smashableCollidersInRange.Add(other);
            smashablesInRange.Add(smashable);
        }
    }
    public void UnregisterSmashable(Collider2D other)
    {
        int index = smashableCollidersInRange.IndexOf(other);
        if (index >= 0)
        {
            smashableCollidersInRange.RemoveAt(index);
            smashablesInRange.RemoveAt(index);
        }
    }

    // Called by Animation Event on the attack clip (the "hit" frame)
    public void SmashFrame()
    {
        if (smashablesInRange.Count == 0) return;

        // Copy so Destroy() / trigger exits won't mutate the list mid-loop
        var copy = new List<ISmashable>(smashablesInRange);

        for (int i = 0; i < copy.Count; i++)
        {
            if (copy[i] == null) continue;
            copy[i].SmashThisObject();
        }

        // Optional: clear lists immediately so next frame doesn't try to smash already-destroyed stuff
        smashablesInRange.Clear();
        smashableCollidersInRange.Clear();
    }

    // =========================
    // Attack (Attack1 -> Attack2, queued after Attack1 finishes)
    // =========================
    void HandleAttackInput()
    {
        if (state == PlayerState.Roll) return;

        // Reset combo if waited too long
        if (timeSinceAttack > comboResetTime)
            currentAttack = 0;

        if (!Input.GetMouseButtonDown(0)) return;

        // If already attacking, just queue the next hit
        if (state == PlayerState.Attack)
        {
            queuedNextAttack = true;
            return;
        }

        // Minimum time between starting attacks
        if (timeSinceAttack < attackSpeedRate) return;

        // Start at Attack1
        currentAttack = 1;
        queuedNextAttack = false;

        StartCoroutine(AttackRoutineFSM(currentAttack));
        timeSinceAttack = 0f;
    }

    IEnumerator AttackRoutineFSM(int attackNumber)
    {
        state = PlayerState.Attack;

        // Freeze immediately
        rb.linearVelocity = Vector2.zero;

        // Determine facing when attack starts
        Vector2 facing = (moveDir.sqrMagnitude > 0.01f) ? moveDir : lastMovedVector;
        if (facing.sqrMagnitude < 0.01f) facing = Vector2.down;

        lastMovedVector = facing.normalized;
        lastDirection = DirectionToIndex(lastMovedVector);

        // Pick and play the clip
        string clipName = (attackNumber == 1)
            ? attack1Directions[lastDirection]
            : attack2Directions[lastDirection];

        anim.Play(clipName);

        // Wait while keeping player frozen
        float animTime = GetClipLength(clipName) / Mathf.Max(0.0001f, anim.speed);
        float t = 0f;
        while (t < animTime)
        {
            rb.linearVelocity = Vector2.zero;
            t += Time.deltaTime;
            yield return null;
        }

        // Chain only after Attack1 finishes
        if (attackNumber == 1 && queuedNextAttack)
        {
            queuedNextAttack = false;
            timeSinceAttack = 0f;

            currentAttack = 2;
            yield return StartCoroutine(AttackRoutineFSM(2));
            yield break;
        }

        // Done attacking
        currentAttack = 0;
        state = (moveDir.sqrMagnitude > 0.01f) ? PlayerState.Move : PlayerState.Idle;
    }

    // =========================
    // Invincibility
    // =========================
    void PlayerInvincibility()
    {
        if (!isInvincible) return;

        invincibilityTimer -= Time.deltaTime;
        if (invincibilityTimer <= 0f)
        {
            isInvincible = false;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        CurrentHP -= amount;
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            Debug.Log("You're dead");
        }

        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
    }

    // =========================
    // Helpers
    // =========================
    float GetClipLength(string clipName)
    {
        if (anim == null || anim.runtimeAnimatorController == null) return 0.2f;

        foreach (var clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName) return clip.length;
        }
        return 0.2f;
    }

    private int DirectionToIndex(Vector2 _direction)
    {
        Vector2 norDir = _direction.normalized;
        float step = 360f / 8f;
        float offset = step / 2f;

        float angle = -Vector2.SignedAngle(Vector2.up, norDir);
        angle += offset;

        if (angle < 0) angle += 360f;

        float stepCount = angle / step;
        return Mathf.FloorToInt(stepCount) % 8;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        
    }
}