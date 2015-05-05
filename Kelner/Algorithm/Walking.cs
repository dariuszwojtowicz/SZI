namespace Kelner.Algorithm
{
    using Kelner.Model;

    public class Walking
    {
        public Section[,] Sections;

        public NTree<State> GenerateTree(State startState, State endState)
        {
            this.CreateSections();
            var tree = new NTree<State>(startState);

            var turnLeftState = this.TurnLeft(startState);
            if (turnLeftState != null)
            {
                tree.AddChild(turnLeftState);
            } 

            var turnRightState = this.TurnRight(startState);
            if (turnRightState != null)
            {
                tree.AddChild(turnRightState);
            }

            var goForwardState = this.GoForward(startState); 
            if (goForwardState != null)
            {
                tree.AddChild(goForwardState);
            }

            return tree;
        }

        private State TurnLeft(State state)
        {
            var newState = new State { X = state.X, Y = state.Y };
            switch (state.Direction)
            {
                case Direction.East:
                    newState.Direction = Direction.North;
                    break;
                case Direction.North:
                    newState.Direction = Direction.West;
                    break;
                case Direction.West:
                    newState.Direction = Direction.South;
                    break;
                case Direction.South:
                    newState.Direction = Direction.East;
                    break;
            }

            return newState;
        }

        private State TurnRight(State state)
        {
            var newState = new State { X = state.X, Y = state.Y };
            switch (state.Direction)
            {
                case Direction.East:
                    newState.Direction = Direction.South;
                    break;
                case Direction.South:
                    newState.Direction = Direction.West;
                    break;
                case Direction.West:
                    newState.Direction = Direction.North;
                    break;
                case Direction.North:
                    newState.Direction = Direction.East;
                    break;
            }

            return newState;
        }

        private State GoForward(State state)
        {
            var newState = new State { Direction = state.Direction };
            switch (state.Direction)
            {
                case Direction.East:
                    if (state.X + 1 <= Sections.GetLength(0))
                    {
                        if (Sections[state.X + 1, state.Y].CanEnter())
                        {
                            newState.X = state.X + 1;
                            newState.Y = state.Y;
                            return newState;
                        }
                    }
                    return null;
                case Direction.North:
                    if (state.Y - 1 >= 0)
                    {
                        if (Sections[state.X, state.Y - 1].CanEnter())
                        {
                            newState.X = state.X;
                            newState.Y = state.Y - 1;
                            return newState;
                        }
                    }
                    return null;
                case Direction.West:
                    if (state.X - 1 >= 0)
                    {
                        if (Sections[state.X - 1, state.Y].CanEnter())
                        {
                            newState.X = state.X - 1;
                            newState.Y = state.Y;
                            return newState;
                        }
                    }
                    return null;
                case Direction.South:
                    if (state.Y + 1 <= Sections.GetLength(0))
                    {
                        if (Sections[state.X, state.Y + 1].CanEnter())
                        {
                            newState.X = state.X;
                            newState.Y = state.Y + 1;
                            return newState;
                        }
                    }
                    return null;
            }

            return newState;
        }

        private void CreateSections()
        {
            this.Sections = new Section[9, 9];

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Section section;

                    if (i * j % 5 == 0)
                    {
                        section = new Chair { X = i * 25, Y = j * 25 };
                    }
                    else if (i * j % 12 == 0)
                    {
                        section = new TablePart { X = i * 25, Y = j * 25 };
                    }
                    else if (i * j == 81)
                    {
                        section = new Kitchen { X = i * 25, Y = j * 25 };
                    }
                    else
                    {
                        section = new Floor { X = i * 25, Y = j * 25 };
                    }

                    this.Sections[i, j] = section;
                }
            }
        }
    }
}
