using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class ThrowingWeapon : Weapon
    {
        protected override void Awake()
        {
            base.Awake();
            AssignKeyAction(); //AwakeÀÚ¸® ¾Æ´Ô
        }

        public override void Init()
        {
            base.Init();
            AssignKeyAction();
        }

        private void AssignKeyAction()
        {
            m_PlayerInputController.SemiFire += LongThrow;
            m_PlayerInputController.HeavyFire += ShortThrow;
        }

        private void LongThrow()
        {

        }

        private void ShortThrow()
        {

        }

        public override void Dispose()
        {
            base.Dispose();
            DischargeKeyAction();
        }

        private void DischargeKeyAction()
        {
            m_PlayerInputController.SemiFire -= LongThrow;
            m_PlayerInputController.HeavyFire -= ShortThrow;
        }
    }
}
