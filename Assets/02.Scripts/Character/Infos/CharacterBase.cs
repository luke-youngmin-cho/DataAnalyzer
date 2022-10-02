using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    public string Name = "";
    public float JumpForce = 5.0f;
    public float MoveSpeed = 3.0f;
    public float DashSpeed = 6.0f;
    public float MoveSpeedMax = 10.0f;
    public float ATK = 10.0f;
    public float DEF = 10.0f;

    public Stats BaseStats;
    public Stats CurrentStats;
    public bool WeaponEquiped;
}