using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;

    private Rigidbody rb;
    private Vector2 moveInput;

    // 직접 만든 Input Action 클래스 참조
    private PlayerInput controls; 

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // 클래스 인스턴스화
        controls = new PlayerInput();
    }

    // Input System을 사용할 때 가장 중요한 활성화/비활성화 처리
    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        // 직접 정의한 Action Map과 Action 이름을 그대로 호출 (자동완성 지원)
        // 예: Action Map 이름이 Player, Action 이름이 Move일 경우
        moveInput = controls.Player.Move.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        Vector3 moveVelocity = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
        rb.linearVelocity = moveVelocity;
    }
}