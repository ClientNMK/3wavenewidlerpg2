using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;


public enum State
{
    Idle,
    Move,
    Dash,
    Attack,
    Skill,
    Hit,
    Victory,

    Pause,
    Dead,
}

public class FSM
{
    protected CharacterBase parent = null;
    public FSM() { }
    public FSM(CharacterBase character)
    {
        parent = character;
    }
    public virtual void Enter()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void Exit()
    {

    }
}
