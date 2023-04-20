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

        [SerializeField] private CrossHairDisplayer m_CrossHairDisplayer;   //옮겨야함

        [SerializeField] private ItemSlotDisplayer m_ItemSlotDisplayer;
        [SerializeField] private WeaponPropertyDisplayer m_WeaponPropertyDisplayer;
        [SerializeField] private DamageDisplayer m_DamageDisplayer;


        public void ChangeWeapon(int equipingWeaponType, int bulletType, int currentRemainBullet, int magazineRemainBullet, Sprite weaponImage)
        {
            m_WeaponPropertyDisplayer.ChangeWeapon(bulletType, currentRemainBullet, weaponImage);
            m_WeaponPropertyDisplayer.UpdateRemainBulletText(magazineRemainBullet);
            //equipingWeaponType은 슬롯 무늬 주기용
        }

        public void ChangeFireMode(int index)
        {
            m_WeaponPropertyDisplayer.UpdateFireMode(index);
        }

        public void RangeWeaponFire(int currentRemainBullet, bool isActive)
        {
            m_WeaponPropertyDisplayer.UpdateRemainBulletIcon(currentRemainBullet);
            m_WeaponPropertyDisplayer.DisplayReloadImage(isActive);
        }

        public void RangeWeaponReload(int currentRemainBullet, int magazineRemainBullet, bool isActive)
        {
            m_WeaponPropertyDisplayer.UpdateRemainBulletIcon(currentRemainBullet);
            m_WeaponPropertyDisplayer.UpdateRemainBulletText(magazineRemainBullet);
            m_WeaponPropertyDisplayer.DisplayReloadImage(isActive);
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

            m_PlayerStatDisplayer.UpdateHPImage(hpAmount);
        }

        public void UpdatePlayerHP(float hpAmount)
        {
            m_PlayerStatDisplayer.UpdateHPImage(hpAmount);

            if(hpAmount <= 500) m_HealDisplayer.DisplayHealNotification(true);
            else m_HealDisplayer.DisplayHealNotification(false);
        }

        public void UpdatePlayerMP(float mpAmount)
        {
            m_PlayerStatDisplayer.UpdateMPImage(mpAmount);
        }
    }
}
