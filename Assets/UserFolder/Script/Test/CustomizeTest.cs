using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeTest : MonoBehaviour
{
    NormalMonster normalMonster;
    Customization customization;
    private void OnValidate()
    {
        normalMonster = GetComponent<NormalMonster>();
        customization = FindObjectOfType<Customization>();
        customization.Customize(normalMonster);
    }
}
