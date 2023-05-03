
namespace EnumType
{
    public enum GravityType
    {
        xDown = 0,
        xUp = 1,
        yDown = 2,
        yUp = 3,
        zDown = 4,
        zUp = 5
    }
    public enum GravityDirection
    {
        X, Y, Z
    }

    public enum NoramlMonsterType
    {
        UrbanZombie = 0,
        OldManZombie = 1,
        WomenZombie = 2,
        BigZombie = 3,
        GiantZombie = 4
    }

    public enum FlyingMonsterType
    {
        BoidMonster = 0,
        RangedFlyingMonster = 1,
        MeleeFlyingMonster = 2
    }

    public enum SpecialMonsterType
    {
        Horridomorph = 0,
        SP2 = 1,
        SP3 = 2
    }

    public enum Boss1
    {
        normalAttack = 0,   //�����̼� ��Ÿ
        rushAttack = 1,     //������
        grapAttack = 2,     //��� ������
        rangeAttack = 3,    //�ָ��� ơ �ϱ�
        jumpAttack = 4,     //�÷��̾� ��ġ�� �����ؼ� ���
        specialPattern = 5  //���� + �ֺ� �� ��ȭ
    }

    public enum Boss2
    {

    }

    public enum Boss3
    {
        normalAttack = 0,   //��ü ������ ���
        rangeAttack = 3,    //��ü ū�� ���
        swarmAttack1 = 4,   //©�� ��� ��� ����
        swarmAttack2 = 5,   //©�� ���󰡱�
        specialPattern = 6  //© ��ȯ
    }

    public enum AnimationStates
    {
        None = 0,
        Idle = 1,
        WalkForward = 2
    }

    public enum SurfaceType
    {
        Concrete = 0,
        Metal = 1,
        Wood = 2
    }
}
