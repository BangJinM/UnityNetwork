using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

//状态机调用切换  ，返回值 类型 Animator Transition
//状态 调用切换，返回值 类型  Animator State Transition

public class MyCreateStateMachine : MonoBehaviour
{

    static AnimatorController animController;
    static AnimatorStateMachine smA;
    static AnimatorStateMachine smB;
    static AnimatorStateMachine smC;


    [MenuItem("UnitySupport/CreateMyStateMachine")]
    static void Create()
    {
        animController = AnimatorController.CreateAnimatorControllerAtPath("Assets/Game/Resources/Anim.controller");
        AddParameters();
        CreateRootStateMachine();

        CreateStateMachineA();
        CreateStateMachineB();
        CreateStateMachineC();
    }

    static void AddParameters()
    {
        animController.AddParameter("TransitionNow", AnimatorControllerParameterType.Trigger);
        animController.AddParameter("Reset", AnimatorControllerParameterType.Trigger);
        animController.AddParameter("GotoB1", AnimatorControllerParameterType.Trigger);
        animController.AddParameter("GotoC", AnimatorControllerParameterType.Trigger);
    }

    static void CreateRootStateMachine()
    {
        AnimatorStateMachine rootStateMachine = animController.layers[0].stateMachine;
        smA = rootStateMachine.AddStateMachine("smA");
        smB = rootStateMachine.AddStateMachine("smB");
        smC = rootStateMachine.AddStateMachine("smC");

        AnimatorTransition stateMachineTransition = rootStateMachine.AddStateMachineTransition(smA, smC);    // 状态机之间的切换
        stateMachineTransition.AddCondition(AnimatorConditionMode.If, 0, "GotoC");

        rootStateMachine.AddStateMachineTransition(smA, smB);
    }

    static void CreateStateMachineA()
    {
        AnimatorState stateA1 = smA.AddState("stateA1");

        AnimatorStateTransition exitTransition = stateA1.AddExitTransition();           //xx state Transition
        exitTransition.AddCondition(AnimatorConditionMode.If, 0, "TransitionNow");
        exitTransition.duration = 0;
    }

    static void CreateStateMachineB()
    {
        AnimatorState stateB1 = smB.AddState("stateB1");        //添加的先后顺序决定了默认状态
        AnimatorState stateB2 = smB.AddState("stateB2");

        AnimatorTransition transitionB1 = smB.AddEntryTransition(stateB1);  // Entry --- B1     xx Transition
        transitionB1.AddCondition(AnimatorConditionMode.If, 0, "GotoB1");

        AnimatorTransition transitionB2 = smB.AddEntryTransition(stateB2);   // Entry --- B2

    }

    static void CreateStateMachineC()
    {
        AnimatorState stateC1 = smC.AddState("stateC1");
        AnimatorState stateC2 = smC.AddState("stateC2");

        //通过这里指定默认状态，所以上边即使先写stateC1也没用，默认状态为 stateC2
        //默认状态，如果不需要指定 entry 到这个默认状态的条件，那么这句就可以达到切换的效果
        smC.defaultState = stateC2;

        // smC.AddEntryTransition(stateC2);      //如果加了这句，就会有三个箭头的黄线

        AnimatorStateTransition exitTransitionC2 = stateC2.AddExitTransition();     //  xx State Transition
        exitTransitionC2.AddCondition(AnimatorConditionMode.If, 0, "TransitionNow");
        exitTransitionC2.duration = 0;
    }


}