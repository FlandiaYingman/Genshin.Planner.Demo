// ReSharper disable All

namespace Genshin.Planner.Demo.LinearProgrammingSolver
{
    class Simplex
    {
        public const int INFEASIBLE = 0;
        public const int OPTIMAL = 1;
        public const int UNBOUNDED = 2;

        public const double MAXITERATIONS = double.PositiveInfinity;

        public static double[] Solve(
            double[][] constraintsCoefficientMatrix,
            double[] constraintsConstantVector,
            double[] objectiveVariableCoefficientsVector,
            double[] variablesLowerBoundVector,
            double[] variablesUpperBoundVector,
            int constraintsNumber,
            int variablesNumber,
            out int status // status flag
        )
        {
            double[] x;
            status = -1; // status flag

            int i, j, k;
            double[] varStatus = new double[variablesNumber + constraintsNumber];
            int[] basicVars = new int[constraintsNumber];
            double[][] Binv = new double[constraintsNumber][];
            for (i = 0; i < constraintsNumber; i++)
            {
                Binv[i] = new double[constraintsNumber];
            }

            double[] cBT = new double[constraintsNumber];
            double[] pi = new double[constraintsNumber];
            double[] rc = new double[variablesNumber];
            double[] BinvAs = new double[constraintsNumber];

            // Some useful constants
            double BASIC = 0f, NONBASIC_L = +1f, NONBASIC_U = -1f;
            double TOL = 0.000001f;

            // The solution
            x = new double[variablesNumber + constraintsNumber];


            // Create the initial solution to Phase 1
            // - Real variables
            for (i = 0; i < variablesNumber; i++)
            {
                var absLB = Math.Abs(variablesLowerBoundVector[i]);
                var absUB = Math.Abs(variablesUpperBoundVector[i]);
                x[i] = (absLB < absUB) ? variablesLowerBoundVector[i] : variablesUpperBoundVector[i];
                varStatus[i] = (absLB < absUB) ? NONBASIC_L : NONBASIC_U;
            }

            // - Artificial variables
            for (i = 0; i < constraintsNumber; i++)
            {
                x[i + variablesNumber] = constraintsConstantVector[i];
                // Some of the real variables might be non-zero, so need
                // to reduce x[artificials] accordingly
                for (j = 0; j < variablesNumber; j++)
                {
                    x[i + variablesNumber] -= constraintsCoefficientMatrix[i][j] * x[j];
                }

                varStatus[i + variablesNumber] = BASIC;
                basicVars[i] = i + variablesNumber;
            }

            // - Basis
            for (i = 0; i < constraintsNumber; i++)
            {
                cBT[i] = +1.0;
            }

            for (i = 0; i < constraintsNumber; i++)
            {
                for (j = 0; j < constraintsNumber; j++)
                {
                    Binv[i][j] = (i == j) ? 1.0 : 0.0;
                }
            }

            // Being simplex iterations
            bool phaseOne = true;
            int iter = 0;
            var z = 0.0;
            while (true)
            {
                iter++;
                if (iter >= MAXITERATIONS)
                {
                    z = 0.0;
                    for (i = 0; i < variablesNumber; i++) z += objectiveVariableCoefficientsVector[i] * x[i];
                    break;
                }

                //---------------------------------------------------------------------
                // Step 1. Duals and reduced Costs

                for (i = 0; i < constraintsNumber; i++)
                {
                    pi[i] = 0.0;
                    for (j = 0; j < constraintsNumber; j++)
                    {
                        pi[i] += cBT[j] * Binv[j][i];
                    }
                }

                for (j = 0; j < variablesNumber; j++)
                {
                    rc[j] = phaseOne ? 0.0 : objectiveVariableCoefficientsVector[j];
                    for (i = 0; i < constraintsNumber; i++)
                    {
                        rc[j] -= pi[i] * constraintsCoefficientMatrix[i][j];
                    }
                }

                //---------------------------------------------------------------------

                //---------------------------------------------------------------------
                // Step 2. Check optimality and pick entering variable
                var minRC = -TOL;
                int s = -1;
                for (i = 0; i < variablesNumber; i++)
                {
                    // If NONBASIC_L (= +1), rc[i] must be negative (< 0) -> +rc[i] < -TOL
                    // If NONBASIC_U (= -1), rc[i] must be positive (> 0) -> -rc[i] < -TOL
                    //                                                      -> +rc[i] > +TOL
                    // If BASIC    (= 0), can't use this rc -> 0 * rc[i] < -LPG_TOL -> alway FALSE
                    // Then, by setting initial value of minRC to -TOL, can collapse this
                    // check and the check for a better RC into 1 IF statement!
                    if (varStatus[i] * rc[i] < minRC)
                    {
                        minRC = varStatus[i] * rc[i];
                        s = i;
                    }
                }

                // If no entering variable
                if (s == -1)
                {
                    if (phaseOne)
                    {
                        z = 0.0;
                        for (i = 0; i < constraintsNumber; i++) z += cBT[i] * x[basicVars[i]];
                        if (z > TOL)
                        {
                            status = INFEASIBLE;
                            break;
                        }
                        else
                        {
                            phaseOne = false;
                            for (i = 0; i < constraintsNumber; i++)
                            {
                                cBT[i] = (basicVars[i] < variablesNumber) ? (objectiveVariableCoefficientsVector[basicVars[i]]) : (0.0);
                            }

                            continue;
                        }
                    }
                    else
                    {
                        status = OPTIMAL;
                        z = 0.0;
                        for (i = 0; i < variablesNumber; i++)
                        {
                            z += objectiveVariableCoefficientsVector[i] * x[i];
                        }

                        break;
                    }
                }
                //---------------------------------------------------------------------

                //---------------------------------------------------------------------
                // Step 3. Calculate BinvAs
                for (i = 0; i < constraintsNumber; i++)
                {
                    BinvAs[i] = 0.0;
                    for (k = 0; k < constraintsNumber; k++) BinvAs[i] += Binv[i][k] * constraintsCoefficientMatrix[k][s];
                }

                //---------------------------------------------------------------------

                //---------------------------------------------------------------------
                // Step 4. Ratio test
                double minRatio = double.PositiveInfinity, ratio = 0.0;
                int r = -1;
                var rIsEV = false;
                // If EV is...
                // NBL, -> rc[s] < 0 -> want to INCREASE x[s]
                // NBU, -> rc[s] > 0 -> want to DECREASE x[s]
                // Option 1: Degenerate iteration
                ratio = variablesUpperBoundVector[s] - variablesLowerBoundVector[s];
                if (ratio <= minRatio)
                {
                    minRatio = ratio;
                    r = -1;
                    rIsEV = true;
                }

                // Option 2: Basic variables leaving basis
                for (i = 0; i < constraintsNumber; i++)
                {
                    j = basicVars[i];
                    var jLB = (j >= variablesNumber) ? 0.0 : variablesLowerBoundVector[j];
                    var jUB = (j >= variablesNumber) ? double.PositiveInfinity : variablesUpperBoundVector[j];
                    if (-1 * varStatus[s] * BinvAs[i] > +TOL)
                    {
                        // NBL: BinvAs[i] < 0, NBU: BinvAs[i] > 0
                        ratio = (x[j] - jUB) / (varStatus[s] * BinvAs[i]);
                        if (ratio <= minRatio)
                        {
                            minRatio = ratio;
                            r = i;
                            rIsEV = false;
                        }
                    }

                    if (+1 * varStatus[s] * BinvAs[i] > +TOL)
                    {
                        // NBL: BinvAs[i] > 0, NBU: BinvAs[i] < 0
                        ratio = (x[j] - jLB) / (varStatus[s] * BinvAs[i]);
                        if (ratio <= minRatio)
                        {
                            minRatio = ratio;
                            r = i;
                            rIsEV = false;
                        }
                    }
                }

                // Check ratio
                if (minRatio >= double.PositiveInfinity)
                {
                    if (phaseOne)
                    {
                        // nothing good!

                        break;
                    }
                    else
                    {
                        // PHASE 2: Unbounded!
                        status = UNBOUNDED;

                        break;
                    }
                }
                //---------------------------------------------------------------------

                //---------------------------------------------------------------------
                // Step 5. Update solution and basis
                x[s] += varStatus[s] * minRatio;
                for (i = 0; i < constraintsNumber; i++) x[basicVars[i]] -= varStatus[s] * minRatio * BinvAs[i];

                if (!rIsEV)
                {
                    // Basis change! Update Binv, flags
                    // RSM tableau: [Binv B | Binv | Binv As]
                    // -> GJ pivot on the BinvAs column, rth row
                    var erBinvAs = BinvAs[r];
                    // All non-r rows
                    for (i = 0; i < constraintsNumber; i++)
                    {
                        if (i != r)
                        {
                            var eiBinvAsOvererBinvAs = BinvAs[i] / erBinvAs;
                            for (j = 0; j < constraintsNumber; j++)
                            {
                                Binv[i][j] -= eiBinvAsOvererBinvAs * Binv[r][j];
                            }
                        }
                    }

                    // rth row
                    for (j = 0; j < constraintsNumber; j++) Binv[r][j] /= erBinvAs;

                    // Update status flags
                    varStatus[s] = BASIC;
                    if (basicVars[r] < variablesNumber)
                    {
                        if (Math.Abs(x[basicVars[r]] - variablesLowerBoundVector[basicVars[r]]) < TOL) varStatus[basicVars[r]] = NONBASIC_L;
                        if (Math.Abs(x[basicVars[r]] - variablesUpperBoundVector[basicVars[r]]) < TOL) varStatus[basicVars[r]] = NONBASIC_U;
                    }
                    else
                    {
                        if (Math.Abs(x[basicVars[r]] - 0.00000) < TOL) varStatus[basicVars[r]] = NONBASIC_L;
                        if (Math.Abs(x[basicVars[r]] - double.PositiveInfinity) < TOL) varStatus[basicVars[r]] = NONBASIC_U;
                    }

                    cBT[r] = phaseOne ? 0.0 : objectiveVariableCoefficientsVector[s];
                    basicVars[r] = s;
                }
                else
                {
                    // Degenerate iteration
                    if (varStatus[s] == NONBASIC_L)
                    {
                        varStatus[s] = NONBASIC_U;
                    }
                    else
                    {
                        varStatus[s] = NONBASIC_L;
                    }
                }
                //---------------------------------------------------------------------
            }

            var solution = new double[variablesNumber + 1];
            solution[^1] = z;
            Array.Copy(x, solution, variablesNumber);

            return solution;
        }
    }
}