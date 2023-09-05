using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UI.Event;

public class PlayerSkillReceiver : MonoBehaviour
{
    [SerializeField] private UnityEvent<int, int> m_GetWeaponEvent;
    [SerializeField] private UnityEvent<int, int> m_GetSupplyEvent;

    [SerializeField] private UnityEvent<float>[] m_AttackEvents;
    [SerializeField] private UnityEvent<int>[] m_DefenseEvents;
    [SerializeField] private UnityEvent<int>[] m_SupportEvents;

    [SerializeField] private UnityEvent[] m_SpecificEvents;
    [SerializeField] private UnityEvent<int>[] m_SpecialEvents;

    public void GetWeaponEvent(int slotNumber, int index)
        => m_GetWeaponEvent?.Invoke(slotNumber,index);
    
    public void GetSupplyEvent(int slotNumber, int amount)
        => m_GetSupplyEvent?.Invoke(slotNumber, amount);
    

    public void AttackSkillEvent(AttackEventType eventType, float amount)
        => m_AttackEvents[(int)eventType]?.Invoke(amount);
    
    public void DefenseSkillEvent(DefenseEventType eventType, int amount)
        => m_DefenseEvents[(int)eventType]?.Invoke(amount);
    
    public void SupportSkillEvent(SupportEventType eventType, int amount)
        => m_SupportEvents[(int)eventType]?.Invoke(amount);
    

    public void SpecificSkillEvent(SpecificEventType eventType, int amount)
        => m_SpecificEvents[(int)eventType]?.Invoke();
    
    public void SpecialSkillEvent(SpeciaEventType eventType, int amount)
        => m_SpecialEvents[(int)eventType]?.Invoke(amount);
}
