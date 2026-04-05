using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int HP;
    [SerializeField]
    private int MAXHP;

    private void Start()
    {
        HP = MAXHP;
    }
    public void HitPlayer(int attake)
    {
        HP -= attake;
        Debug.Log("受击伤害"+attake+"目前生命"+HP);
    }
}
