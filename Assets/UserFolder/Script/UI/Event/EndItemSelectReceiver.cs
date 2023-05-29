using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndItemSelectReceiver : MonoBehaviour
{
    [SerializeField] private UnityEvent m_EndAnimation;
    private void EndAnimation() => m_EndAnimation?.Invoke();
}
