using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;

    [Header("Dash Setting")]
    // [추가] 대시 제어용 변수
    private bool isDashing = false;
    private Vector3 dashDirection;
    private float dashTimer = 0f;
    public float dashDistance = 5f;  // 목표 거리
    public float dashDuration = 0.2f; // 목표 시간


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

        // [추가] 우클릭(RightClick) 액션이 수행되었을 때 실행할 함수 연결
        controls.Player.Dash.performed += ctx => OnRightClickAction();
    }

    // Input System을 사용할 때 가장 중요한 활성화/비활성화 처리
    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
        
        // [추가] 오브젝트가 꺼질 때 이벤트 해제 (메모리 누수 방지 예방)
        if (controls != null)
        {
            controls.Player.Dash.performed -= ctx => OnRightClickAction();
        }
    }

    void Update()
    {
        // 직접 정의한 Action Map과 Action 이름을 그대로 호출 (자동완성 지원)
        if (!isDashing) // [변경] 대시 중이 아닐 때만 입력 허용
        {
            moveInput = controls.Player.Move.ReadValue<Vector2>();
        }
    }

    void FixedUpdate()
    {
        if (isDashing) // [추가] 대시 상태 연산
        {
            dashTimer += Time.fixedDeltaTime;
            
            if (dashTimer >= dashDuration)
            {
                isDashing = false;
                rb.linearVelocity = Vector3.zero; // 대시 종료 후 정지
            }
            else
            {
                // Sin 함수를 이용해 초반엔 빠르고 끝엔 부드럽게 멈추는 속도 배율 계산
                float progress = dashTimer / dashDuration; 
                float speedMultiplier = Mathf.Sin(progress * Mathf.PI * 0.5f + Mathf.PI * 0.5f); 
                
                // 평균 이동 속도에 삼각함수 배율을 곱해 속도 강제 지정
                float targetSpeed = (dashDistance / dashDuration) * speedMultiplier;
                rb.linearVelocity = dashDirection * targetSpeed;
            }
        }
        else // [기존 평소 이동 코드를 else로 감싸기]
        {
            Vector3 moveVelocity = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
            rb.linearVelocity = moveVelocity; 
        }
    }


    /// <summary>
    /// [추가] 마우스 우클릭 시 호출되는 빈 함수
    /// </summary>
    private void OnRightClickAction()
    {
        if (isDashing) return;

        if (CursorManager.Instance != null)
            CursorManager.Instance.ChangeCursorColor(Color.blue);

        // [수정] 이동 입력 방향을 보지 않고, '무조건' 마우스 커서가 있는 방향으로 대시 방향을 확정합니다.
        if (CursorManager.Instance != null)
        {
            Vector3 mouseWorldPos = CursorManager.Instance.GetMouseWorldPosition();
            dashDirection = (mouseWorldPos - transform.position).normalized;
            dashDirection.y = 0; // 3D 공간이지만 위아래로 날아가지 않게 Y축 힘은 차단
        }
        else
        {
            // 혹시 모를 예외 상황(커서 매니저 누락 등)에만 캡슐이 바라보는 앞방향(기본값)으로 대시
            dashDirection = transform.forward;
        }

        isDashing = true;
        dashTimer = 0f;
    }

}
