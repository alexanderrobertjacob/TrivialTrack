using UnityEngine;
using System.Collections;

public class State<owner_type>
{
    public const int STAY_IN_STATE = -1;
    public virtual void OnEnter(owner_type owner) { }
    public virtual int PerformStateAction(owner_type owner) { return STAY_IN_STATE;}
    public virtual void OnExit(owner_type owner) { }

    public Hashtable actionList = new Hashtable();

    public class StateAction
    {
        public delegate int TypeStateHandler(owner_type owner, params object[] arg);

        private TypeStateHandler handlerAction;

        public StateAction(TypeStateHandler action)
        {
            handlerAction = action;
        }

        public int performAction(owner_type owner, object[] arg)
        {
            return handlerAction(owner, arg);
        }
    }
}

public class StateMachine<owner_type>
{
    private State<owner_type>[] statesList;
    private int currentState;
    private owner_type owner;

    public StateMachine()
    {
        currentState = 0;
    }

    public StateMachine(int initialState, State<owner_type>[] initialStatesList, owner_type initialOwner)
    {
        currentState = initialState;
        owner = initialOwner;
        statesList = initialStatesList;
        statesList[currentState].OnEnter(owner);
    }

    public void sendActionMessage(string name, object[] arg)
    {
//        Debug.Log("Action message sent: " + name);
        if (statesList[currentState] != null)
        {
            State<owner_type>.StateAction actionPerformed = (State<owner_type>.StateAction)statesList[currentState].actionList[name];

            if (actionPerformed != null)
            {
                changeState(actionPerformed.performAction(owner, arg));
            }

        }

        return;
    }

    public void changeState(int ID)
    {
        statesList[currentState].OnExit(owner);
        currentState = ID;
        statesList[currentState].OnEnter(owner);
    }

    public void performStateAction()
    {
		var newState = statesList[currentState].PerformStateAction(owner);
		if (newState != currentState) changeState(newState);
    }
}
