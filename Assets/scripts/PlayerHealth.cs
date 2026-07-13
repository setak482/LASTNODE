using UnityEngine;
using UnityEngine.UI;
using System;


public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;

    [Header("Date")]
    public PlayerData data = new PlayerData();

    [Header("UI")]
    public Slider hpSlider;

    [Header("피격 설정")]
    public float invincibleTime = 0.5f;
    public bool isInvincible = false;
    public bool isDead = false;

    private float displayedHp;

// 다른 스크립트에서 체력 변화 감지할 수 있도록 이벤트 제공
    public event Action<int, int> OnHpChanged; // (currentHP, maxHP)
    public event Action OnPlayerDeath;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        data.ResetDate();
        displayedHp = data.currentHP;
        UpdateSlider();
        OnHpChanged?.Invoke(data.currentHP, data.maxHP);
    }

    // Update is called once per frame
    void Update()
    {
        HanbleHpBar();
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible) return;
        if(damage <= 0) return;

        data.currentHP -= damage;
        data.currentHP = Mathf.Clamp(data.currentHP, 0, data.maxHP);

        OnHpChanged?.Invoke(data.currentHP, data.maxHP);

        if(data.currentHP <= 0)
        {
            Die();
        }
        else
        {
            //StartCoroutine(invincibleRoutine());
        }
    }

    public void Heal(int amount)
    {
        if(isDead || amount <= 0) return;

        data.currentHP += amount;
        data.currentHP = Mathf.Clamp(data.currentHP, 0 , data.maxHP);
        OnHpChanged?.Invoke(data.currentHP, data.maxHP);
    }

    /*System.Collections.IEnumerator InvincibleRoutine()
    {
        isInvincible = false;
        //이펙트나 애니메이션 필요시 구현
    
    }*/

    void Die()
    {
        isDead = true;
        OnPlayerDeath?.Invoke();
        //gameOver시 이벤트
    }

    void HanbleHpBar()
    {
        if (hpSlider == null) return;
        displayedHp = Mathf.Lerp(displayedHp, data.currentHP, Time.deltaTime * 5f);
        hpSlider.value = displayedHp / data.maxHP;
    }

    void UpdateSlider()
    {
        if(hpSlider == null) return;
        hpSlider.value = (float) data.currentHP / data.maxHP;
    }
}
