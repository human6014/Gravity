using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSkillReceiver : MonoBehaviour
{
    [SerializeField] private UnityEvent<int, int> m_GetWeaponEvent;
    [SerializeField] private UnityEvent<int, int> m_GetSupplyEvent;

    [SerializeField] private UnityEvent<float>[] m_AttackEvents;
    [SerializeField] private UnityEvent<int>[] m_DefenseEvents;
    [SerializeField] private UnityEvent<int>[] m_SupportEvents;

    public void GetWeaponEvent(int slotNumber, int index)
        => m_GetWeaponEvent?.Invoke(slotNumber,index);
    
    public void GetSupplyEvent(int slotNumber, int amount)
        => m_GetSupplyEvent?.Invoke(slotNumber, amount);
    
    public void AttackSkillEvent(UI.Event.AttackEventType eventType, float amount)
    {
        switch (eventType)
        {
            case UI.Event.AttackEventType.AttackDamageUp:
                //WeaponStatScriptable

                //float         amount
                break;
            case UI.Event.AttackEventType.AttackSpeedUp:
                //WeaponStatScriptable

                //float         amount
                break;
            case UI.Event.AttackEventType.MaxBulletUp:
                //WeaponInfo

                //int(float)    amount
                break;
            case UI.Event.AttackEventType.ReloadSpeedUp:
                //Reloadable

                //float         amount
                break;
        }

        m_AttackEvents[(int)eventType]?.Invoke(amount);
    }

    public void DefenseSkillEvent(UI.Event.DefenseEventType eventType, int amount)
    {
        switch (eventType)
        {
            case UI.Event.DefenseEventType.GetHealKit:
                //Inventory

                //int   amount
                break;
            case UI.Event.DefenseEventType.HealKitRateUp:
                //PlayerData
                
                //int   amount
                break;
            case UI.Event.DefenseEventType.HealthRecoverUp:
                //PlayerData
                
                //int   amount
                break;
            case UI.Event.DefenseEventType.MaxHealthUp:
                //PlayerData

                //int   amount
                break;
        }

        m_DefenseEvents[(int)eventType]?.Invoke(amount);
    }

    public void SupportSkillEvent(UI.Event.SupportEventType eventType, int amount)
    {
        switch (eventType)
        {
            case UI.Event.SupportEventType.MaxStaminaUp:
                //PlayerData

                //int   amount
                break;
            case UI.Event.SupportEventType.MoveSpeedUp:
                //FirstPersonController

                //int?float   amount
                break;
            case UI.Event.SupportEventType.StaminaConsumeDown:
                //PlayerData

                //int   amount
                break;
            case UI.Event.SupportEventType.StaminaRecoverUp:
                //PlayerData

                //int   amount
                break;
        }
        m_SupportEvents[(int)eventType]?.Invoke(amount);
    }

    public void SpecificSkillEvent()
    {

    }

    public void SpecialSkillEvent()
    {

    }
}
