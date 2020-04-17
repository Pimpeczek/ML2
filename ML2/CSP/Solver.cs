using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML2.CSP
{
    public class Solver
    {
        protected Variable[] variables;
        protected Constraint[] constraints;
        protected Stack<State> stateStack;
        protected ICSPProblem cspProblem;
        public State FinalState;
        protected int curVar;
        public int counter = 0;
        public double PossibleCombinations { get; protected set; }
        public Solver(ICSPProblem problem)
        {
            cspProblem = problem;
            this.variables = problem.PrepareVariables();
            PossibleCombinations = variables[0].DomainSize;
            for (int i = 1; i < variables.Length; i++)
            {
                PossibleCombinations *= variables[i].DomainSize;
            }
            this.constraints = problem.PrepareConstraints();
            stateStack = new Stack<State>();
            stateStack.Push(problem.PrepareState());
            curVar = -1;

        }

        public void Calculate()
        {
            counter = 0;
            Try();
            stateStack.Clear();

        }

        protected bool Try()
        {
            counter++;
            curVar++;
            if (curVar >= variables.Length)
                return false;
            State newState = stateStack.Peek().MakeClone();
            stateStack.Push(newState);
            variables[curVar].Set = true;
            for (int d = variables[curVar].DomainSize - 1; d >= 0; d--)
            {
                if (variables[curVar].Domain.Mask[d])
                {
                    
                    newState.Values[variables[curVar].R][variables[curVar].C] = variables[curVar].Domain.Values[d];
                    counter++;
                    if (curVar < variables.Length-1 && !cspProblem.ShrinkDomain(newState.Values, variables[curVar + 1]))
                    {
                        variables[curVar + 1].Domain.ResetMask();
                    }
                    else if (CheckConstraints(newState))
                    {
                        if (curVar == variables.Length-1)
                        {
                            FinalState = stateStack.Pop();
                            stateStack.Clear();
                            return true;
                        }
                        if (Try())
                        {
                            return true;
                        }
                        else
                        {
                            stateStack.Pop();
                        }
                        curVar--;
                    }
                    if (curVar < variables.Length - 1)
                    {
                        variables[curVar + 1].Domain.ResetMask();
                    }
                }

            }
            variables[curVar].Set = false;
            return false;
        }

        protected bool CheckConstraints(State state)
        {
            for(int c = 0; c < constraints.Length; c++)
            {
                if (!constraints[c].Check(state.Values))
                    return false;
            }
            return true;
        }

        public void ResetDomains()
        {
            for (int i = 0; i < variables.Length; i++)
            {
                variables[i].Domain.ResetMask();
            }
        }
        public void WriteState()
        {
            if (stateStack.Count > 0)
                Console.WriteLine($"{stateStack.Peek()}\n");
            else
                Console.WriteLine($"{FinalState}\n");
        }

    }
}
