using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 각종 상수들과 열거형들을 정리해놓은 static 클래스
public static class Constants {

    public const int TurnStop = 0;
    public const int TurnRight = 1;
    public const int TurnLeft = 2;

    public const int MoveStop = 0;
    public const int MoveWalk = 1;
    public const int MoveRun = 2;

    public const int MoveFront = 1;
    public const int MoveRight = 2;
    public const int MoveLeft = 3;
    public const int MoveBack = 4;

    public const int JumpEnd = -1;
    public const int JumpUp = 1;
    public const int JumpVer = 2;
    public const int JumpHor = 3;

    public const float JumpDistValue = 0.6f;

    public const int InputNum1 = 0;
    public const int InputNum2 = 1;
    public const int InputNum3 = 2;
    public const int InputNum4 = 3;
    public const int InputNum5 = 4;
    public const int InputNum6 = 5;
    public const int InputQ = 6;
    public const int InputE = 7;
    public const int InputR = 8;
    public const int InputT = 9;
    public const int InputMouseR = 10;
    public const int InputLCtrl = 11;
    public const int InputTab = 12;

    public const int HitStop = 0;
    public const int HitAnim1 = 1;
    public const int HitAnim2 = 2;
    
    public const int SkillEnd = 0;
    public const float NormalSpeed = 1f;
    
    public const bool FindState = true;
    public const bool NotFindState = false;

    public const int FailUseSkill = 0;
    public const int AtkSkill = 1;
    public const int NotAtkSkill = 2;

    public const bool AtkState = true;
    public const bool NotAtkState = false;

    public const float CantUseSkill = 0.01f;

    public const int LayerForce1 = 8;
    public const int LayerForce2 = 9;
    public const int LayerForce3 = 10;
    public const int LayerForce4 = 11;
    public const int LayerForce5 = 12;
    public const int LayerPlayer = 13;

    public const int DeathFront = 1;
    public const int DeathBack = 2;
    public const int DeathEnd = 0;

    public const bool Idle = true;
    public const bool IdleStop = false;

    public const int DropCharBrute = 0;
    public const int DropCharZombie = 1;
    public const int DropForce1 = 0;
    public const int DropForce2 = 1;
    public const int DropForce3 = 2;
    public const int DropOrderPatrol = 0;
    public const int DropOrderMove = 1;
    public const int DropLocationFar = 0;
    public const int DropLocationNear = 1;

    public const int BlockingHit = 1;

    public const float DefaultHitRecoveryValue = 5f;

    public enum CurrentState
    {
        Dead = -1,
        Idle,
        Move,
        Jump,
        ModeChange,
        Skill,
        Hit,
        Length
    }

    public enum KeyInput
	{
		num1, num2, num3, num4, num5, num6,
		Q, E, R, T,
		MouseL, MouseR,
		Length
	}

	public enum ComboType
	{
		Single, Combo, Finish, Blocking
	}
    
    public enum SpawnType
    {

    }

	//public enum SkillType
	//{
	//	Blocking = 0,
	//	Tumbling,
	//	Melee,
	//	Range,
	//	Missile,
	//	Buff,
	//}
    
}
