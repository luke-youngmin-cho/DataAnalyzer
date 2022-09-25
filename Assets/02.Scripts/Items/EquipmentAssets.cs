using UnityEngine;
using System.Collections.Generic;
public class EquipmentAssets : MonoBehaviour
{
    public static EquipmentAssets _instance;
    public static EquipmentAssets instance
    {
        get
        {
            if (_instance == null)
                _instance = Instantiate(Resources.Load<EquipmentAssets>("EquipmentAssets"));
            return _instance;
        }
    }

    private List<Equipment> weapons;
    private List<Equipment> heads;
    private List<Equipment> bodys;
    private List<Equipment> tops;
    private List<Equipment> bottoms;
    private List<Equipment> feets;

    // Prefabs
    //=================================================
    public GameObject GetWeaponPrefab(int code)
        => weapons.Find(x => x.code == code).gameObject;
    public GameObject GetHeadPrefab(int code)
        => heads.Find(x => x.code == code).gameObject;
    public GameObject GetBodyPrefab(int code)
        => bodys.Find(x => x.code == code).gameObject;
    public GameObject GetTopPrefab(int code)
        => tops.Find(x => x.code == code).gameObject;
    public GameObject GetBottomPrefab(int code)
        => bottoms.Find(x => x.code == code).gameObject;
    public GameObject GetFeetPrefab(int code)
        => feets.Find(x => x.code == code).gameObject;

    // Components
    //=================================================
    public Equipment GetWeapon(int code)
        => weapons.Find(x => x.code == code);
    public Equipment GetHead(int code)
        => heads.Find(x => x.code == code);
    public Equipment GetBody(int code)
        => bodys.Find(x => x.code == code);
    public Equipment GetTop(int code)
        => tops.Find(x => x.code == code);
    public Equipment GetBottom(int code)
        => bottoms.Find(x => x.code == code);
    public Equipment GetFeet(int code)
        => feets.Find(x => x.code == code);
}