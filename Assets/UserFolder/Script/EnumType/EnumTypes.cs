
namespace EnumType
{
    public enum GravityType
    {
        xUp = 0,
        xDown = 1,
        yUp = 2,
        yDown = 3,
        zUp = 4,
        zDown = 5
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
        RangedFlyingMonster = 0,
        MeleeFlyingMonster = 1
    }

    public enum Boss1
    {
        normalAttack = 0,   //가까이서 평타
        rushAttack = 1,     //돌진기
        grapAttack = 2,     //잡고 던지기
        rangeAttack = 3,    //멀리서 퉤 하기
        jumpAttack = 4,     //플레이어 위치로 점프해서 찍기
        specialPattern = 5  //본인 + 주변 몹 강화
    }

    public enum Boss2
    {

    }

    public enum Boss3
    {
        normalAttack = 0,   //본체 작은거 쏘기
        rangeAttack = 3,    //본체 큰거 쏘기
        swarmAttack1 = 4,   //짤들 잠깐 찍고 가기
        swarmAttack2 = 5,   //짤들 따라가기
        specialPattern = 6  //짤 소환
    }

    public enum AnimationStates
    {
        None = 0,
        Idle = 1,
        WalkForward = 2
    }
}
