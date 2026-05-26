using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    public GameObject[] weapons;

    int currentWeapon = 0;

    void Start()
    {
        SelectWeapon(currentWeapon);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeapon = 0;
            SelectWeapon(currentWeapon);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeapon = 1;
            SelectWeapon(currentWeapon);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentWeapon = 2;
            SelectWeapon(currentWeapon);
        }
    }

    void SelectWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == index);
        }
    }
}