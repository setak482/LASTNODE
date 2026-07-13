using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int maxHP = 100;
    public int currentHP = 100;
    public int attackPower = 10;
    public float moveSpeed = 5f;

    public void ResetData()
    {
        currentHP = maxHP;
    }
}
