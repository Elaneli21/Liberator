using UnityEngine;

// Saya tempted buat namain nya Player aja abisan buat sekarang lebih gampang bikin behaviour nya
// disini aja, kalo ditaro di script laen kayak nya cuma nambah complexity padahal basic nya kita
// juga belom.
public class Player : MonoBehaviour
{
    public float speed = 3f;

    private Rigidbody2D rigid;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 moveInput = new Vector2();
    private bool inputAttack = false;
    private bool canMove = true;

    private int currentAtkSeq = 1;
    
    #region Animations

    // ini biar nama animation nya jadi lebih gampang(less error prone juga) nyari/ganti nama nya
    protected static class AnimationNames
    {
        // Cara pake nya tinggal tulis AnimationsNames. terus nanti mucul autocomplete(kalo punya) variable
        // yang isinya nama nama animation(ini yang dibawah ini). kalo mau tambahin ya gampang, tinggal
        // duplicate kalo males
        public static readonly string Run = "Run";
        public static readonly string Idle = "Idle";
        public static readonly string Attack = "Attack1";

        // Ini saya bikin class biar ga numpuk di class PlayerMovement nya, kalo saya gini in
        // nanti kan semua variable Run, Idle, Attack, etc jadi di dalem satu "container"
    }

    // ini versi simple nya (gapake class class an)
    //protected readonly string Run_AnimationName = "Player_Run";
    //protected readonly string Idle_AnimationName = "Player_Idle";

    #endregion

    void Start()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleInput();
        HandleAnimation();

        //Debug.Log(currentAtkSeq);
    }

    private void HandleInput()
    {
        //receive input for movement
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(inputX, inputY);

        if (Input.GetMouseButtonDown(0))
            inputAttack = true;
    }

    private void HandleAnimation()
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        bool playingAttack = currentState.IsTag("Attack");

        //Debug.Log(currentState);

        if (!playingAttack && inputAttack)
        {
            Debug.Log("attacking");
            canMove = false;
            Attack();
        }

        if (playingAttack)
        {
            if (currentState.normalizedTime >= 1f)
            {

                if (inputAttack)
                    Attack();
                else
                {
                    Debug.Log("asfasdfsegsdgsd" + currentState);
                    canMove = true;
                    animator.Play(AnimationNames.Idle);
                    currentAtkSeq = 1;
                }
            }
        }

        if (!playingAttack && canMove)
        {
            HandleMovementAnimation();
        }
    }

    void Attack()
    {
        string attackAnim;

        //combo system
        if (currentAtkSeq > 3)
        {
            currentAtkSeq = 1;
        }

        attackAnim = "Attack" + currentAtkSeq;
        currentAtkSeq++;

        animator.Play(attackAnim);
        inputAttack = false;
    }

    private void HandleMovementAnimation()
    {
        //flip sprite according to move direction
        if (moveInput.x != 0) spriteRenderer.flipX = moveInput.x < 0f;

        //play animation when doing action

        // yang lama saya tinggalin disini biar kalo bingung bisa but reference ato mau diapain setera lah
        //if (inputX != 0 || inputY != 0) anim.Play("Player_Run");

        // yang 0f bisa diganti jadi threshold kalo mau aman
        if (moveInput.sqrMagnitude > 0f) animator.Play(AnimationNames.Run);
        else animator.Play(AnimationNames.Idle);
    }

    private void FixedUpdate()
    {
        rigid.velocity = CalculateVelocity();
    }

    private Vector2 CalculateVelocity()
    {
        // Misal nya kalo kita lagi attacking, kita mau player nya engga digerakin
        // pake Rigidbody, jadi kita set velocity nya ke zero.
        // kalo mau lebih jelas nama method nya harus nya jadi CalculateRigibodyVelocity, cuma
        // ya kepanjangan, jadi saya potong aja nama nya
        if (!canMove) return Vector2.zero;

        // normalized maksud nya mastiin biar vector nya itu panjang nya selalu 1(kalo bukan 0),
        // jadi misal kalo dia input nya kanan atas(1, 1) ini kan panjang nya bukan 1, nanti pendekin
        // tapi arah nya tetep sama.
        // karna panjang nya udah pasti 1, nah kalo vector input(movement) nya dikaliin speed kan berarti
        // panjang nya jadi sepanjang speed nya dan arah nya tetep berdasarkan input player.
        return moveInput.normalized * speed;
    }
}
