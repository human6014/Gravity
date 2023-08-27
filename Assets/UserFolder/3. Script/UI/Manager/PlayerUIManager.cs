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
        [SerializeField] private VerticalBarDisplayer m_GravityEnergeDisplayer;
        [SerializeField] private VerticalBarDisplayer m_TimeEnergeDisplayer;

        public void Init(int playerMaxHP, int playerMaxMP, int amountToRealConst, float realToAmountConst, int havingHealKit)
        {
            m_PlayerStatDisplayer.Init(playerMaxHP, playerMaxMP, realToAmountConst);
            m_HealDisplayer.Init(havingHealKit);
            m_WeaponPropertyDisplayer.Init();
            m_DamageDisplayer.Init(playerMaxHP, amountToRealConst);
        }

        /// <summary>
        /// ���� ����� UI ����
        /// </summary>
        /// <param name="equipingWeaponType">�������� ���� ���� ��ȣ(Ÿ��)</param>
        /// <param name="bulletType">�������� ������ �Ѿ� ����(Ÿ��)</param>
        /// <param name="fireMode">��� ���</param>
        /// <param name="currentRemainBullet">������ ������ ���� �Ѿ� ����</param>
        /// <param name="magazineRemainBullet">������ ������ ���� �Ѿ� ����</param>
        /// <param name="weaponImage">���� �̹��� ������</param>
        public void ChangeWeapon(int equipingWeaponType, int bulletType, int fireMode, int currentRemainBullet, int magazineRemainBullet, Sprite weaponImage)
        {
            m_WeaponPropertyDisplayer.ChangeWeapon(bulletType, currentRemainBullet);
            m_WeaponPropertyDisplayer.UpdateWeaponIcon(weaponImage);
            m_WeaponPropertyDisplayer.UpdateRemainBulletText(magazineRemainBullet);
            m_WeaponPropertyDisplayer.UpdateFireMode(fireMode);

            m_ItemSlotDisplayer.UpdateFocusSlot(equipingWeaponType);
        }
        public void UpdatePlayerHP(float hpAmount)
        {
            m_PlayerStatDisplayer.UpdateHPImage(hpAmount);
            m_DamageDisplayer.DisplayBloodScreen(hpAmount);
            m_HealDisplayer.DisplayHealNotification(hpAmount <= 0.5f);
        }

        public void RangeWeaponFire(int currentRemainBullet, bool isActive)
        {
            m_WeaponPropertyDisplayer.UpdateRemainBulletIcon(currentRemainBullet);
            m_WeaponPropertyDisplayer.DisplayReloadImage(isActive);
        }
        public void DisplayReloadImage(bool isActive)
            => m_WeaponPropertyDisplayer.DisplayReloadImage(isActive);

        public void ChangeFireMode(int index)
            => m_WeaponPropertyDisplayer.UpdateFireMode(index);

        public void RangeWeaponReload(int magazineRemainBullet)
            => m_WeaponPropertyDisplayer.UpdateRemainBulletText(magazineRemainBullet);

        public void HitEnemy()
            => m_AttackCrossHairDisplayer.AttackCrossHairActive();

        public void UpdateWeaponSlot(int equipingWeaponType, Sprite sprite)
            => m_ItemSlotDisplayer.UpdateWeaponSlotIcon(equipingWeaponType, sprite);

        public void UsingHealKit(int value)
            => m_HealDisplayer.UpdateRemainHeal(value);

        public void DisplayHitDirection(Transform target)
            => m_DamageDisplayer.DisplayHitDirection(target);

        public void UpdatePlayerMaxHP(int playerMaxHP, int amountToRealConst)
            => m_DamageDisplayer.ReCalcScreenAlpha(playerMaxHP, amountToRealConst);

        public void UpdatePlayerMP(float mpAmount)
            => m_PlayerStatDisplayer.UpdateMPImage(mpAmount);

        public void UpdatePlayerGE(float geAmount)
            => m_GravityEnergeDisplayer.UpdateBarImage(geAmount);

        public void UpdatePlayerTE(float teAmount)
            => m_TimeEnergeDisplayer.UpdateBarImage(teAmount);
    }
}
