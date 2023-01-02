using Genshin.Planner.Demo.LinearProgrammingSolver;

namespace LpSolverTest
{
    public class LpSolverTest
    {
        public static void Main()
        {
            var lpSolver = new LpSolver();
            var v1 = lpSolver.CreateVariable("a");
            v1.ObjectiveCoefficient = 12.0;
            var v2 = lpSolver.CreateVariable("b");
            v2.ObjectiveCoefficient = 8.0;

            var c1 = lpSolver.CreateConstraint("1");
            c1.SetCoefficient("a", 5.0);
            c1.SetCoefficient("b", 2.0);
            c1.UpperBound = 120.0;

            var c2 = lpSolver.CreateConstraint("2");
            c2.SetCoefficient("a", 2.0);
            c2.SetCoefficient("b", 3.0);
            c2.UpperBound = 90.0;

            var c3 = lpSolver.CreateConstraint("3");
            c3.SetCoefficient("a", 4.0);
            c3.SetCoefficient("b", 2.0);
            c3.UpperBound = 100.0;

            lpSolver.Solve(LpSolver.ProblemType.Maximize);
            lpSolver.PrintSolution();
        }
    }
}