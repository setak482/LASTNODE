using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }
    
    [Header("Cursor Color Settings")]
    [SerializeField] private float colorRecoverySpeed = 5f; // 색상이 원래대로 돌아오는 속도
    [SerializeField] private float cursorPlaneHeight = 0f; // 커서를 찍을 평면의 Y 위치
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

        Ray ray = mainCamera.ScreenPointToRay(_screenMousePosition);
        Plane cursorPlane = new Plane(Vector3.up, Vector3.up * cursorPlaneHeight);

        if (cursorPlane.Raycast(ray, out float enterDistance))
        {
            _worldMousePosition = ray.GetPoint(enterDistance);
        }
        else
        {
            _worldMousePosition = mainCamera.transform.position + mainCamera.transform.forward * 10f;
        }

        if (currentCursorTransform != null)
        {
            currentCursorTransform.position = _worldMousePosition + Vector3.up * 0.2f;
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
