using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Player;

namespace UI.Manager
{
    public class PlayerUIManager : MonoBehaviour
    {
        [SerializeField] private PlayerStatDisplayer m_PlayerStatDisplayer;
        [SerializeField] private HealDisplayer m_HealDisplayer;
        [SerializeField] private AttackCrossHairDisplayer m_AttackCrossHairDisplayer;
        [SerializeField] private ItemSlotDisplayer m_ItemSlotDisplayer;
        [SerializeField] private WeaponPropertyDisplayer m_WeaponPropertyDisplayer;
        [SerializeField] private DamageDisplayer m_DamageDisplayer;

        public void Init(int playerMaxHP, int playerMaxMP, int amountToRealConst, float realToAmountConst, int havingHealKit)
        {
            m_PlayerStatDisplayer.Init(playerMaxHP, playerMaxMP, realToAmountConst);
            m_HealDisplayer.Init(havingHealKit);
            m_WeaponPropertyDisplayer.Init();
            m_DamageDisplayer.Init(playerMaxHP, amountToRealConst);
        }

        public void ChangeWeapon(int equipingWeaponType, int bulletType, int fireMode, int currentRemainBullet, int magazineRemainBullet, Sprite weaponImage)
        {
            m_WeaponPropertyDisplayer.ChangeWeapon(bulletType, currentRemainBullet);
            m_WeaponPropertyDisplayer.UpdateWeaponIcon(weaponImage);
            m_WeaponPropertyDisplayer.UpdateRemainBulletText(magazineRemainBullet);
            m_WeaponPropertyDisplayer.UpdateFireMode(fireMode);

            m_ItemSlotDisplayer.UpdateFocusSlot(equipingWeaponType);
        }

        public void DisplayReloadImage(bool isActive)
        {
            m_WeaponPropertyDisplayer.DisplayReloadImage(isActive);
        }

        public void ChangeFireMode(int index)
        {
            m_WeaponPropertyDisplayer.UpdateFireMode(index);
        }

        public void HitEnemy()
        {
            m_AttackCrossHairDisplayer.AttackCrossHairActive();
        }

        public void RangeWeaponFire(int currentRemainBullet, bool isActive)
        {
            m_WeaponPropertyDisplayer.UpdateRemainBulletIcon(currentRemainBullet);
            m_WeaponPropertyDisplayer.DisplayReloadImage(isActive);
        }

        public void RangeWeaponReload(int currentRemainBullet, int magazineRemainBullet, bool isActive)
        {
            m_WeaponPropertyDisplayer.UpdateRemainBulletText(magazineRemainBullet);
            RangeWeaponFire(currentRemainBullet, isActive);
        }

        public void UpdateWeaponSlot(int equipingWeaponType, Sprite sprite)
        {
            m_ItemSlotDisplayer.UpdateWeaponSlotIcon(equipingWeaponType, sprite);
        }

        public void UpdateRemainThrowingWeapon(int remainThrowingWeapon)
        {

        }

        public void UsingHealKit(int value, float hpAmount)
        {
            m_HealDisplayer.UpdateRemainHeal(value);
        }

        public void UpdatePlayerHP(float hpAmount)
        {
            m_PlayerStatDisplayer.UpdateHPImage(hpAmount);
            m_DamageDisplayer.DisplayBloodScreen(hpAmount);
            DisplayHealImage(hpAmount);
        }

        public void DisplayHitDirection(Transform target)
        {
            m_DamageDisplayer.DisplayHitDirection(target);
        }

        public void UpdatePlayerMP(float mpAmount)
        {
            m_PlayerStatDisplayer.UpdateMPImage(mpAmount);
        }

        private void DisplayHealImage(float hpAmount)
        {
            if (hpAmount <= 0.5f) m_HealDisplayer.DisplayHealNotification(true);
            else m_HealDisplayer.DisplayHealNotification(false);
        }
    }
}
