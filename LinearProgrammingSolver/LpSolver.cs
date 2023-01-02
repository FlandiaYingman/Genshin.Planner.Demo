namespace Genshin.Planner.Demo.LinearProgrammingSolver
{
    public class LpSolver
    {
        public enum ProblemType
        {
            Minimize,
            Maximize
        }

        public enum ProblemResult
        {
            Infeasible,
            Optimal,
            Unbounded,
        }

        internal Dictionary<string, LpVariable> VariableMap = new();
        internal List<LpVariable> VariableList = new();
        internal Dictionary<string, LpConstraint> ConstraintMap = new();
        internal List<LpConstraint> ConstraintList = new();

        public double OptimumValue { get; set; }

        public LpVariable CreateVariable(string name)
        {
            var variable = new LpVariable(name);
            VariableList.Add(variable);
            VariableMap.Add(name, variable);
            return variable;
        }

        public LpConstraint CreateConstraint(string name)
        {
            var constraint = new LpConstraint(this, name);
            ConstraintList.Add(constraint);
            ConstraintMap.Add(name, constraint);
            return constraint;
        }

        public ProblemResult Solve(ProblemType problemType)
        {
            var constraintsCoefficientMatrix = CreateConstraintsCoefficientMatrix();
            var constraintsConstantVector = CreateConstraintsConstantVector();
            var maximizeObjectiveVariableCoefficientsVector = CreateObjectiveVariableCoefficientsVector(problemType);
            var variablesLowerBoundVector = CreateVariablesLowerBoundVector();
            var variablesUpperBoundVector = CreateVariablesUpperBoundVector();
            var constraintsNumber = GetConstraintsNumber();
            var variablesNumber = GetVariablesNumber();
            var solution = Simplex.Solve(
                ToJaggedArray(constraintsCoefficientMatrix),
                constraintsConstantVector,
                maximizeObjectiveVariableCoefficientsVector,
                variablesLowerBoundVector,
                variablesUpperBoundVector,
                constraintsNumber,
                variablesNumber,
                out var status
            );

            for (int i = 0; i < VariableList.Count; i++)
            {
                VariableList[i].OptimizerValue = solution[i];
            }

            OptimumValue = problemType switch
            {
                ProblemType.Minimize => +solution[^1],
                ProblemType.Maximize => -solution[^1],
                _ => throw new ArgumentOutOfRangeException(nameof(problemType), problemType, null)
            };

            return status switch
            {
                Simplex.INFEASIBLE => ProblemResult.Infeasible,
                Simplex.OPTIMAL => ProblemResult.Optimal,
                Simplex.UNBOUNDED => ProblemResult.Unbounded,
                _ => throw new Exception($"status should not be {status}!")
            };
        }

        public void PrintSolution()
        {
            Console.WriteLine($"Optimum Value: ${OptimumValue}");
            VariableList.ForEach(x => Console.WriteLine($"{x.Name} Optimizer Value: {x.OptimizerValue}"));
        }


        private double[,] CreateConstraintsCoefficientMatrix()
        {
            // constraintList.Count * 2 是因为我们需要两个等式来表达两个不等式，即：下界 <= x <= 上界。
            // variableList.Count + constraintList.Count * 2 是因为我们需要同等于约束个数的松弛变量。
            var mat = new double[GetConstraintsNumber(), GetVariablesNumber()];
            var rowIndex = 0;
            var colIndex = 0;
            foreach (var constraint in ConstraintList)
            {
                if (!double.IsNegativeInfinity(constraint.LowerBound))
                {
                    mat[rowIndex, VariableList.Count + colIndex] = -1.0;
                    for (var vi = 0; vi < VariableList.Count; vi++)
                    {
                        var variable = VariableList[vi];
                        if (!constraint.Coefficients.ContainsKey(variable)) continue;
                        mat[rowIndex, vi] = constraint.Coefficients[variable];
                    }
                    rowIndex++;
                    colIndex++;
                }
                if (!double.IsPositiveInfinity(constraint.UpperBound))
                {
                    mat[rowIndex, VariableList.Count + colIndex] = 1.0;
                    for (var vi = 0; vi < VariableList.Count; vi++)
                    {
                        var variable = VariableList[vi];
                        if (!constraint.Coefficients.ContainsKey(variable)) continue;
                        mat[rowIndex, vi] = constraint.Coefficients[variable];
                    }
                    rowIndex++;
                    colIndex++;
                }
            }

            return mat;
        }

        private double[] CreateConstraintsConstantVector()
        {
            // constraintList.Count * 2 是因为我们需要两个等式来表达两个不等式，即：下界 <= x <= 上界。
            var vec = new double[GetConstraintsNumber()];
            var rowIndex = 0;
            foreach (var constraint in ConstraintList)
            {
                if (!double.IsNegativeInfinity(constraint.LowerBound))
                {
                    vec[rowIndex] = constraint.LowerBound;
                    rowIndex++;
                }
                if (!double.IsPositiveInfinity(constraint.UpperBound))
                {
                    vec[rowIndex] = constraint.UpperBound;
                    rowIndex++;
                }
            }

            return vec;
        }

        private double[] CreateObjectiveVariableCoefficientsVector(ProblemType problemType)
        {
            // variableList.Count + constraintList.Count * 2 是因为我们需要同等于约束个数的松弛变量。
            var mat = new double[VariableList.Count + ConstraintList.Count * 2];
            for (var vi = 0; vi < VariableList.Count; vi++)
            {
                var variable = VariableList[vi];
                mat[vi] = problemType switch
                {
                    ProblemType.Minimize => +variable.ObjectiveCoefficient,
                    ProblemType.Maximize => -variable.ObjectiveCoefficient,
                    _ => throw new ArgumentOutOfRangeException(nameof(problemType), problemType, null)
                };
            }

            return mat;
        }

        private double[] CreateVariablesLowerBoundVector()
        {
            // variableList.Count + constraintList.Count * 2 是因为我们需要同等于约束个数的松弛变量。
            var mat = new double[VariableList.Count + ConstraintList.Count * 2];
            Array.Fill(mat, 0.0);
            for (var vi = 0; vi < VariableList.Count; vi++)
            {
                var variable = VariableList[vi];
                mat[vi] = variable.LowerBound;
            }

            return mat;
        }

        private double[] CreateVariablesUpperBoundVector()
        {
            // variableList.Count + constraintList.Count * 2 是因为我们需要同等于约束个数的松弛变量。
            var mat = new double[VariableList.Count + ConstraintList.Count * 2];
            Array.Fill(mat, double.PositiveInfinity);
            for (var vi = 0; vi < VariableList.Count; vi++)
            {
                var variable = VariableList[vi];
                mat[vi] = variable.UpperBound;
            }

            return mat;
        }

        private int GetConstraintsNumber()
        {
            return ConstraintList.Count * 2 - ConstraintList.Count(x => double.IsNegativeInfinity(x.LowerBound)) - ConstraintList.Count(x => double.IsPositiveInfinity(x.UpperBound));
        }

        private int GetVariablesNumber()
        {
            return VariableList.Count + GetConstraintsNumber();
        }

        internal static T[][] ToJaggedArray<T>(T[,] twoDimensionalArray)
        {
            int rowsFirstIndex = twoDimensionalArray.GetLowerBound(0);
            int rowsLastIndex = twoDimensionalArray.GetUpperBound(0);
            int numberOfRows = rowsLastIndex + 1;

            int columnsFirstIndex = twoDimensionalArray.GetLowerBound(1);
            int columnsLastIndex = twoDimensionalArray.GetUpperBound(1);
            int numberOfColumns = columnsLastIndex + 1;

            T[][] jaggedArray = new T[numberOfRows][];
            for (int i = rowsFirstIndex; i <= rowsLastIndex; i++)
            {
                jaggedArray[i] = new T[numberOfColumns];

                for (int j = columnsFirstIndex; j <= columnsLastIndex; j++)
                {
                    jaggedArray[i][j] = twoDimensionalArray[i, j];
                }
            }

            return jaggedArray;
        }

        public class LpVariable
        {
            public string Name { get; }
            public double LowerBound { get; set; } = 0.0;
            public double UpperBound { get; set; } = double.PositiveInfinity;
            public double ObjectiveCoefficient { get; set; }

            public double OptimizerValue { get; internal set; }

            internal LpVariable(string name)
            {
                Name = name;
            }
        }

        public class LpConstraint
        {
            private readonly LpSolver _solver;
            public string Name { get; }
            public double LowerBound { get; set; }
            public double UpperBound { get; set; }
            public Dictionary<LpVariable, double> Coefficients { get; } = new();

            internal LpConstraint(LpSolver solver, string name)
            {
                _solver = solver;
                Name = name;
            }

            public void SetCoefficient(string variableName, double coefficient)
            {
                Coefficients[_solver.VariableMap[variableName]] = coefficient;
            }
        }
    }
}