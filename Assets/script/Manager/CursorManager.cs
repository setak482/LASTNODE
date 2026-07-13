using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }
    
    [Header("Cursor Color Settings")]
    [SerializeField] private float colorRecoverySpeed = 5f; // 색상이 원래대로 돌아오는 속도
    private SpriteRenderer _cursorSpriteRenderer;
    private Color _targetColor = Color.white; // 최종 목표 색상 (평소 색상)

    [Header("References")]
    [SerializeField] private Camera mainCamera; 
    [SerializeField] private Transform currentCursorTransform;

    private PlayerInput _inputActions;
    private Vector2 _screenMousePosition;
    private Vector3 _worldMousePosition;

    private void Start(){
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _inputActions = new PlayerInput();

        if (currentCursorTransform != null){
            _cursorSpriteRenderer = currentCursorTransform.GetComponent<SpriteRenderer>();
        }
    }

    private void OnEnable()
    {
        _inputActions.Cursor.Enable();
        _inputActions.Cursor.Mouse.performed += ctx => _screenMousePosition = ctx.ReadValue<Vector2>();
        _inputActions.Cursor.Mouse.canceled += ctx => _screenMousePosition = ctx.ReadValue<Vector2>();
    }

    private void OnDisable()
    {
        _inputActions.Cursor.Disable();
    }

    void Update()
    {
        if (mainCamera == null) return;

        // 1. Orthographic 월드 좌표 변환
        Vector3 tempPos = mainCamera.ScreenToWorldPoint(new Vector3(
            _screenMousePosition.x, 
            _screenMousePosition.y, 
            0f 
        ));

        // 2. [핵심 수정] 커서의 Y축 높이를 카메라보다 약간만 낮은 위치(예: Y = 9.0f)로 잡습니다.
        // 카메라가 Y=10에 있다면, 9.0f는 캡슐(높이 2)이나 벽보다 훨씬 높은 공중입니다.
        // Orthographic 뷰 특성상 공중에 떠 있어도 바닥을 정확히 조준하는 것처럼 보입니다.
        float renderHeight = mainCamera.transform.position.y - 1f;

        _worldMousePosition = new Vector3(tempPos.x, renderHeight, tempPos.z);

        // 3. 커서 오브젝트 위치 적용
        if (currentCursorTransform != null)
        {
            currentCursorTransform.position = _worldMousePosition;
        }
    }

    void LateUpdate()
    {
        if (_cursorSpriteRenderer == null) return;

        // 현재 커서 색상을 타겟 색상(흰색)으로 부드럽게 보간(Lerp) 처리
        // 우클릭 시 파란색이 되었다가, 손을 떼거나 시간이 지나면 자연스럽게 흰색으로 슥 돌아옵니다.
        _cursorSpriteRenderer.color = Color.Lerp(_cursorSpriteRenderer.color, _targetColor, Time.deltaTime * colorRecoverySpeed);
    }
    // 플레이어가 대시 방향 등을 계산할 때 가져다 쓸 월드 좌표
    public Vector3 GetMouseWorldPosition()
    {
        return _worldMousePosition;
    }

    // 스프라이트 색 바꾸기 (임시 확인용)
    public void ChangeCursorColor(Color newColor)
    {
        if (_cursorSpriteRenderer != null)
        {
            _cursorSpriteRenderer.color = newColor;
        }
    }
}
