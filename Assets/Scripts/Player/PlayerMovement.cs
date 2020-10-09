using UnityEngine;

// Saya tempted buat namain nya Player aja abisan buat sekarang lebih gampang bikin behaviour nya
// disini aja, kalo ditaro di script laen kayak nya cuma nambah complexity padahal basic nya kita
// juga belom.
public class PlayerMovement : MonoBehaviour
{
    public float speed = 3f;

    private Rigidbody2D rigid;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 moveInput = new Vector2();
    private bool attack = false;
    private bool canMove = true;

    // ini saya comment gara gara ada warning
    //private float moveHor;
    //private float moveVer;
    //private float moveLimit = 0.7f;

    #region Animations

    // ini biar nama animation nya jadi lebih gampang(less error prone juga) nyari/ganti nama nya
    protected static class AnimationNames
    {
        // Cara pake nya tinggal tulis AnimationsNames. terus nanti mucul autocomplete(kalo punya) variable
        // yang isinya nama nama animation(ini yang dibawah ini). kalo mau tambahin ya gampang, tinggal
        // duplicate kalo males
        public static readonly string Run = "Player_Run";
        public static readonly string Idle = "Player_Idle";

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
        // Saya pindahin ke function masing masing biar
        // 1. Fungsionalitas nya mereka bedua emang distinct
        // 2. Rapih aja
        // 3. Kalo pengen di pindah pindah ke script laen etc, jadi lebih gampang

        HandleInput();
        HandleAnimation();
    }

    private void HandleInput()
    {
        //receive input for movement
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(inputX, inputY);

        if (Input.GetMouseButtonDown(0))
            attack = true;
    }

    private void HandleAnimation()
    {
        //flip sprite according to move direction
        if (moveInput.x != 0) spriteRenderer.flipX = moveInput.x < 0f;

        //play animation when doing action

        // yang lama saya tinggalin disini biar kalo bingung bisa but reference ato mau diapain setera lah
        //if (inputX != 0 || inputY != 0) anim.Play("Player_Run");

        // yang 0f bisa diganti jadi threshold kalo mau aman
        if (moveInput.sqrMagnitude > 0f) animator.Play(AnimationNames.Run);
        else animator.Play(AnimationNames.Run);
    }

    private void FixedUpdate()
    {
        // Ini ga saya pindah ke function sendiri 'cause ya, cuma 2 line, mo gimana?
        // comment nya doang yang panjang nya minta ampun

        // saya pake placeholder variable biar moveInput nya gausa diubah ubah,
        // 'cause move input kan representasi player input nya, bukan velocity.
        // normalized maksud nya mastiin biar vector nya itu panjang nya selalu 1(kalo bukan 0),
        // jadi misal kalo dia input nya kanan atas(1, 1) ini kan panjang nya bukan 1, nanti pendekin
        // tapi arah nya tetep sama.
        // karna panjang nya udah pasti 1, nah kalo vector input(movement) nya dikaliin speed kan berarti
        // panjang nya jadi sepanjang speed nya dan arah nya tetep berdasarkan input player.
        Vector2 velocity = moveInput.normalized * speed;

        // ini maksud nya kan Vector2D itu arah, nah magnitude(sqrMagnitude sama aja cuma lebih performant aja)
        // itu panjang vector/arah nya itu. ini kita ngecek aja kalo vector nya ini panjang nya lebih dari 0
        // apa engga, kalo iya berarti player nya ada input
        //if (moveInput.sqrMagnitude > 0f)
        //{
        //    moveHor *= moveLimit;
        //    moveVer *= moveLimit;
        //}
        // ini kalo saya pikir pikir kayak nya gausa deh, emang mau buat apaan kode diatas ini yang saya comment?

        rigid.velocity = velocity;
    }

}
