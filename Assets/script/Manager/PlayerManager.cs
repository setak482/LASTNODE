using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform spawnPoint; // 인스펙터에서 빈 오브젝트를 하나 만들어 위치 지정

    void Awake()
    {
        // 씬 시작 시 플레이어 생성
        Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
    }
}