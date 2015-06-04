/*
 W tym namespace można umieścić klasy zajmujące się typowo sztuczną inteligencją, czyli sieci neuronowe itp. 
 Potem można jakoś wyodrębnić w inne namespace jakby ktoś chciał, to tak póki co roboczo
 */

using System;
using System.Collections.Generic;
using System.Linq;

using Kelner.Model;

public enum StepDirection
{
    GoForward,
    GoBack,
    TurnLeft,
    TurnRight
}

public enum Direction
{
    North,
    East,
    West,
    South
}

public enum WaiterAction
{
    TurnLeft,
    TurnRight,
    GoForward
}

public class State
{
    public int X { get; set; }

    public int Y { get; set; }

    public Direction Direction { get; set; }
}

public class StateNode
{
    public State State { get; set; }

    public double CostValue { get; set; }

    public StepDirection StepDirection { get; set; }
}

public class WalkingAStarOld
{
    private List<State> historyOfStates;

    public WalkingAStarOld()
    {
        this.historyOfStates = new List<State>();
    }

    public List<WaiterAction> GetPath(State currentWaiterState, State targetState, Section[][] restaurantSections)
    {
        var actions = new List<WaiterAction>();
        var state = currentWaiterState;
        this.historyOfStates.Add(state);

        var counter = 0;

        while ((state.X != targetState.X || state.Y != targetState.Y) && counter < 30)
        {
            var newState = this.GetStateNode(state, targetState, restaurantSections);

            if (newState != null)
            {
                switch (newState.StepDirection)
                {
                    case StepDirection.TurnLeft:
                        actions.Add(WaiterAction.TurnLeft);
                        break;
                    case StepDirection.TurnRight:
                        actions.Add(WaiterAction.TurnRight);
                        break;
                    case StepDirection.GoBack:
                        actions.Add(WaiterAction.TurnRight);
                        actions.Add(WaiterAction.TurnRight);
                        break;
                }

                actions.Add(WaiterAction.GoForward);
            }
            else
            {
                return actions;
            }

            state = newState.State;
            counter++;
        }

        return actions;
    }

    private StateNode GetStateNode(State currentWaiterState, State targetState, Section[][] restaurantSections)
    {
        var possibleNodeStates = this.GetStateNodes(currentWaiterState, targetState, restaurantSections);

        return possibleNodeStates.OrderBy(s => s.CostValue).FirstOrDefault();
    }

    private List<StateNode> GetStateNodes(State currentWaiterState, State targetState, Section[][] restaurantSections)
    {
        var possibleNodeStates = new List<StateNode>();

        var goForwardState = this.GetGoForwardStateNode(currentWaiterState, targetState, restaurantSections);
        if (goForwardState != null)
        {
            possibleNodeStates.Add(goForwardState);
        }

        var turnLeftState = this.GetTurnLeftStateNode(currentWaiterState, targetState, restaurantSections);
        if (turnLeftState != null)
        {
            possibleNodeStates.Add(turnLeftState);
        }

        var turnRightState = this.GetTurnRightStateNode(currentWaiterState, targetState, restaurantSections);
        if (turnRightState != null)
        {
            possibleNodeStates.Add(turnRightState);
        }

        var goBackState = this.GetGoBackStateNode(currentWaiterState, targetState, restaurantSections);
        if (goBackState != null)
        {
            possibleNodeStates.Add(goBackState);
        }

        return possibleNodeStates;
    }  

    private StateNode GetGoForwardStateNode(State currentWaiterState, State targetState, Section[][] restaurantSections)
    {
        var newState = new State { Direction = currentWaiterState.Direction };
        switch (newState.Direction)
        {
            case Direction.East:
                if (currentWaiterState.X + 1 < restaurantSections.GetLength(0))
                {
                    if (restaurantSections[currentWaiterState.X + 1][currentWaiterState.Y].CanEnter())
                    {
                        newState.X = currentWaiterState.X + 1;
                        newState.Y = currentWaiterState.Y;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                break;

            case Direction.North:
                if (currentWaiterState.Y - 1 >= 0)
                {
                    if (restaurantSections[currentWaiterState.X][currentWaiterState.Y - 1].CanEnter())
                    {
                        newState.X = currentWaiterState.X;
                        newState.Y = currentWaiterState.Y - 1;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                break;

            case Direction.West:
                if (currentWaiterState.X - 1 >= 0)
                {
                    if (restaurantSections[currentWaiterState.X - 1][currentWaiterState.Y].CanEnter())
                    {
                        newState.X = currentWaiterState.X - 1;
                        newState.Y = currentWaiterState.Y;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                break;

            case Direction.South:
                if (currentWaiterState.Y + 1 < restaurantSections.GetLength(0))
                {
                    if (restaurantSections[currentWaiterState.X][currentWaiterState.Y + 1].CanEnter())
                    {
                        newState.X = currentWaiterState.X;
                        newState.Y = currentWaiterState.Y + 1;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                break;
        }

        if (this.IsHistoryState(newState))
        {
            return null;
        }

        var newStateNode = new StateNode
                               {
                                   State = newState,
                                   StepDirection = StepDirection.GoForward,
                                   CostValue = this.CountCostValue(newState, targetState, StepDirection.GoForward)
                               };

        return newStateNode;
    }

    private StateNode GetTurnLeftStateNode(State currentWaiterState, State targetState, Section[][] restaurantSections)
    {
        var newState = new State();
        switch (currentWaiterState.Direction)
        {
            case Direction.East:
                if (currentWaiterState.Y - 1 >= 0)
                {
                    if (restaurantSections[currentWaiterState.X][currentWaiterState.Y - 1].CanEnter())
                    {
                        newState.X = currentWaiterState.X;
                        newState.Y = currentWaiterState.Y - 1;
                        newState.Direction = Direction.North;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                break;

            case Direction.North:
                if (currentWaiterState.X - 1 >= 0)
                {
                    if (restaurantSections[currentWaiterState.X - 1][currentWaiterState.Y].CanEnter())
                    {
                        newState.X = currentWaiterState.X - 1;
                        newState.Y = currentWaiterState.Y;
                        newState.Direction = Direction.West;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                break;

            case Direction.West:
                if (currentWaiterState.Y + 1 < restaurantSections.GetLength(0))
                {
                    if (restaurantSections[currentWaiterState.X][currentWaiterState.Y + 1].CanEnter())
                    {
                        newState.X = currentWaiterState.X;
                        newState.Y = currentWaiterState.Y + 1;
                        newState.Direction = Direction.South;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                break;

            case Direction.South:
                if (currentWaiterState.X + 1 < restaurantSections.GetLength(0))
                {
                    if (restaurantSections[currentWaiterState.X + 1][currentWaiterState.Y].CanEnter())
                    {
                        newState.X = currentWaiterState.X + 1;
                        newState.Y = currentWaiterState.Y;
                        newState.Direction = Direction.East;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                break;
        }

        if (this.IsHistoryState(newState))
        {
            return null;
        }

        var newStateNode = new StateNode
        {
            State = newState,
            StepDirection = StepDirection.TurnLeft,
            CostValue = this.CountCostValue(newState, targetState, StepDirection.TurnLeft)
        };

        return newStateNode;
    }

    private StateNode GetTurnRightStateNode(State currentWaiterState, State targetState, Section[][] restaurantSections)
    {
        var newState = new State();
        switch (currentWaiterState.Direction)
        {
            case Direction.East:
                if (currentWaiterState.Y + 1 < restaurantSections.Length)
                {
                    if (restaurantSections[currentWaiterState.X][currentWaiterState.Y + 1].CanEnter())
                    {
                        newState.X = currentWaiterState.X;
                        newState.Y = currentWaiterState.Y + 1;
                        newState.Direction = Direction.South;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                break;

            case Direction.North:
                if (currentWaiterState.X + 1 < restaurantSections.Length)
                {
                    if (restaurantSections[currentWaiterState.X + 1][currentWaiterState.Y].CanEnter())
                    {
                        newState.X = currentWaiterState.X + 1;
                        newState.Y = currentWaiterState.Y;
                        newState.Direction = Direction.East;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                break;

            case Direction.West:
                if (currentWaiterState.Y - 1 >= 0)
                {
                    if (restaurantSections[currentWaiterState.X][currentWaiterState.Y - 1].CanEnter())
                    {
                        newState.X = currentWaiterState.X;
                        newState.Y = currentWaiterState.Y - 1;
                        newState.Direction = Direction.North;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                break;

            case Direction.South:
                if (currentWaiterState.X - 1 >= 0)
                {
                    if (restaurantSections[currentWaiterState.X - 1][currentWaiterState.Y].CanEnter())
                    {
                        newState.X = currentWaiterState.X - 1;
                        newState.Y = currentWaiterState.Y;
                        newState.Direction = Direction.West;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                break;
        }

        if (this.IsHistoryState(newState))
        {
            return null;
        }

        var newStateNode = new StateNode
        {
            State = newState,
            StepDirection = StepDirection.TurnRight,
            CostValue = this.CountCostValue(newState, targetState, StepDirection.TurnRight)
        };

        return newStateNode;
    }

    private StateNode GetGoBackStateNode(State currentWaiterState, State targetState, Section[][] restaurantSections)
    {
        var newState = new State();
        switch (currentWaiterState.Direction)
        {
            case Direction.East:
                if (currentWaiterState.Y - 1 >= 0)
                {
                    if (restaurantSections[currentWaiterState.X][currentWaiterState.Y - 1].CanEnter())
                    {
                        newState.X = currentWaiterState.X;
                        newState.Y = currentWaiterState.Y - 1;
                        newState.Direction = Direction.West;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                break;

            case Direction.North:
                if (currentWaiterState.Y + 1 < restaurantSections.Length)
                {
                    if (restaurantSections[currentWaiterState.X][currentWaiterState.Y + 1].CanEnter())
                    {
                        newState.X = currentWaiterState.X;
                        newState.Y = currentWaiterState.Y + 1;
                        newState.Direction = Direction.South;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                break;

            case Direction.West:
                if (currentWaiterState.X + 1 < restaurantSections.Length)
                {
                    if (restaurantSections[currentWaiterState.X + 1][currentWaiterState.Y].CanEnter())
                    {
                        newState.X = currentWaiterState.X + 1;
                        newState.Y = currentWaiterState.Y;
                        newState.Direction = Direction.East;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                break;

            case Direction.South:
                if (currentWaiterState.Y - 1 >= 0)
                {
                    if (restaurantSections[currentWaiterState.X][currentWaiterState.Y - 1].CanEnter())
                    {
                        newState.X = currentWaiterState.X;
                        newState.Y = currentWaiterState.Y - 1;
                        newState.Direction = Direction.North;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                break;
        }

        if (this.IsHistoryState(newState))
        {
            return null;
        }

        var newStateNode = new StateNode
        {
            State = newState,
            StepDirection = StepDirection.GoBack,
            CostValue = this.CountCostValue(newState, targetState, StepDirection.GoBack)
        };

        return newStateNode;
    }

    private double CountCostValue(State newState, State targetState, StepDirection stepDirection)
    {
        double g = 0;
        switch (stepDirection)
        {
            case StepDirection.GoForward:
                g = 0;
                break;
            case StepDirection.GoBack:
                g = 0.25 * 2;
                break;
            case StepDirection.TurnLeft:
            case StepDirection.TurnRight:
                g = 0.25;
                break;
        }

        double h = (Math.Abs(newState.X - targetState.X) + Math.Abs(newState.Y - targetState.Y)) / 2;

        return g + h;
    }

    private bool IsHistoryState(State newState)
    {
        return this.historyOfStates.Any(s => s.X == newState.X && s.Y == newState.Y);
    }
}