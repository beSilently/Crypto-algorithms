using System;
namespace CryptoAlg
{
    public class Matrix : IEquatable<Matrix>
    {
        private double[,] _array;

        public Matrix(double[,] array)
        {
            _array = array;
        }

        public Matrix(int rowNumber, int columnNumber)
        {
            _array = new double[rowNumber, columnNumber];
        }

        public Vector this[int index]
        {
            get
            {
                var result = new Vector(_array.GetLength(1));

                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = _array[index, i];
                }

                return result;
            }
            set
            {
                var vectorLength = value.Length;
                if (_array.GetLength(1) != vectorLength)
                {
                    throw new ArgumentException("The length of the vector is not equal to the number of columns.");
                }
                for (int i = 0; i < vectorLength; i++)
                {
                    _array[index, i] = value[i];

                }
            }
        }

        public double this[int row, int col]
        {
            get
            {
                return _array[row, col];
            }
            set
            {
                _array[row, col] = value;
            }
        }

        public bool Equals(Matrix other)
        {
            if(other.GetLength(0) != this.GetLength(0) || other.GetLength(1) != this.GetLength(1))
            {
                return false;
            }

            for (int i = 0; i < other.GetLength(0); i++)
            {
                for (int j = 0; j < other.GetLength(1); j++)
                {
                    double difference = Math.Abs(this[i, j] * .00001);

                    if (Math.Abs(this[i, j] - other[i, j]) > difference)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public int GetLength(int dimension)
        {
            return _array.GetLength(dimension);
        }

        public bool IsSquare()
        {
            return GetLength(0) == GetLength(1);
        }

        public void TransposeRows(int rowInd1, int rowInd2)
        {
            var temp = this[rowInd1];

            this[rowInd1] = this[rowInd2];
            this[rowInd2] = temp;
        }

        public Matrix MatMul(Matrix other)
        { 
            var thisMatrixRowCount = this.GetLength(0);
            var thisMatrixColCount = this.GetLength(1);
            var otherMatrixRowCount = other.GetLength(0);
            var otherMatrixColCount = other.GetLength(1);

            if (thisMatrixColCount != otherMatrixRowCount)
                throw new InvalidOperationException
                  ("Product is undefined. n columns of the first matrix must equal to n rows of the second matrix");

            Matrix product = new Matrix(thisMatrixRowCount, otherMatrixColCount);

            for (int matrix1_row = 0; matrix1_row < thisMatrixRowCount; matrix1_row++)
            {
                for (int matrix2_col = 0; matrix2_col < otherMatrixColCount; matrix2_col++)
                {
                    for (int matrix1_col = 0; matrix1_col < thisMatrixColCount; matrix1_col++)
                    {
                        product[matrix1_row, matrix2_col] +=
                          this[matrix1_row, matrix1_col] *
                          other[matrix1_col, matrix2_col];
                    }
                }
            }

            return product;
        }

        public static Matrix Identity(int dimension)
        {
            var matrix = new Matrix(dimension, dimension);

            for (int i = 0; i < dimension; i++)
            {
                matrix[i, i] = 1;
            }

            return matrix;
        }

        public static Matrix RandInt(int min, int max, (int, int) size)
        {
            var matrix = new Matrix(size.Item1, size.Item2);

            Random rnd = new Random();

            for (int i = 0; i < size.Item1; i++)
            {
                for (int j = 0; j < size.Item2; j++)
                {
                    matrix[i, j] = rnd.Next(min, max);
                }
            }

            return matrix;
        }

        public Matrix Inverse()
        {
            if (!IsSquare())
            {
                throw new InvalidOperationException("Only square matrices can be inverted.");
            }
                
            int dimension = GetLength(0);
            var result = _array.Clone() as double[,];
            var identity = _array.Clone() as double[,];

            for (int _row = 0; _row < dimension; _row++)
                for (int _col = 0; _col < dimension; _col++)
                {
                    identity[_row, _col] = (_row == _col) ? 1.0 : 0.0;
                }

            for (int i = 0; i < dimension; i++)
            {
                double temporary = result[i, i];
                for (int j = 0; j < dimension; j++)
                {
                    result[i, j] = result[i, j] / temporary;
                    identity[i, j] = identity[i, j] / temporary;
                }
                for (int k = 0; k < dimension; k++)
                {
                    if (i != k)
                    {
                        temporary = result[k, i];
                        for (int n = 0; n < dimension; n++)
                        {
                            result[k, n] = result[k, n] - temporary * result[i, n];
                            identity[k, n] = identity[k, n] - temporary * identity[i, n];
                        }
                    }
                }
            }
            return new Matrix(identity);
        }

        public static double Determinant(Matrix matrix)
        {
            int[] perm;
            int toggle;
            double[,] lum = MatrixDecompose(matrix._array, out perm, out toggle);
            if (lum == null)
            {
                Console.WriteLine("Unable to compute MatrixDeterminant");
                return 0;
            }
            double result = toggle;
            for (int i = 0; i < lum.GetLength(0); ++i)
                result *= lum[i, i];

            return result;
        }

        #region Determinat calculation stuff

        private static double[,] MatrixDecompose(double[,] matrix, out int[] perm, out int toggle)
        {
            // Doolittle LUP decomposition with partial pivoting.
            // rerturns: result is L (with 1s on diagonal) and U; perm holds row permutations; toggle is +1 or -1 (even or odd)
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            //Check if matrix is square
            if (rows != cols)
                throw new Exception("Attempt to MatrixDecompose a non-square mattrix");

            double[,] result = MatrixDuplicate(matrix); // make a copy of the input matrix

            perm = new int[rows]; // set up row permutation result
            for (int i = 0; i < rows; ++i) { perm[i] = i; } // i are rows counter

            toggle = 1; // toggle tracks row swaps. +1 -> even, -1 -> odd. used by MatrixDeterminant

            for (int j = 0; j < rows - 1; ++j) // each column, j is counter for coulmns
            {
                double colMax = Math.Abs(result[j, j]); // find largest value in col j
                int pRow = j;
                for (int i = j + 1; i < rows; ++i)
                {
                    if (result[i, j] > colMax)
                    {
                        colMax = result[i, j];
                        pRow = i;
                    }
                }

                if (pRow != j) // if largest value not on pivot, swap rows
                {
                    double[] rowPtr = new double[result.GetLength(1)];

                    //in order to preserve value of j new variable k for counter is declared
                    //rowPtr[] is a 1D array that contains all the elements on a single row of the matrix
                    //there has to be a loop over the columns to transfer the values
                    //from the 2D array to the 1D rowPtr array.
                    //----tranfer 2D array to 1D array BEGIN

                    for (int k = 0; k < result.GetLength(1); k++)
                    {
                        rowPtr[k] = result[pRow, k];
                    }

                    for (int k = 0; k < result.GetLength(1); k++)
                    {
                        result[pRow, k] = result[j, k];
                    }

                    for (int k = 0; k < result.GetLength(1); k++)
                    {
                        result[j, k] = rowPtr[k];
                    }

                    //----tranfer 2D array to 1D array END

                    int tmp = perm[pRow]; // and swap perm info
                    perm[pRow] = perm[j];
                    perm[j] = tmp;

                    toggle = -toggle; // adjust the row-swap toggle
                }

                if (Math.Abs(result[j, j]) < 1.0E-20) // if diagonal after swap is zero . . .
                    return null; // consider a throw

                for (int i = j + 1; i < rows; ++i)
                {
                    result[i, j] /= result[j, j];
                    for (int k = j + 1; k < rows; ++k)
                    {
                        result[i, k] -= result[i, j] * result[j, k];
                    }
                }
            } // main j column loop

            return result;
        } // MatrixDecompose

        // --------------------------------------------------------------------------------------------------------------
        private static double[,] MatrixDuplicate(double[,] matrix)
        {
            // allocates/creates a duplicate of a matrix. assumes matrix is not null.
            var result = new Matrix(matrix.GetLength(0), matrix.GetLength(1));
            for (int i = 0; i < matrix.GetLength(0); ++i) // copy the values
                for (int j = 0; j < matrix.GetLength(1); ++j)
                    result[i, j] = matrix[i, j];
            return result._array;
        }

        // --------------------------------------------------------------------------------------------------------------
        private static double[,] ExtractLower(double[,] matrix)
        {
            // lower part of a Doolittle decomposition (1.0s on diagonal, 0.0s in upper)
            int rows = matrix.GetLength(0); int cols = matrix.GetLength(1);
            var result = new Matrix(rows, cols);
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    if (i == j)
                        result[i, j] = 1.0f;
                    else if (i > j)
                        result[i, j] = matrix[i, j];
                }
            }
            return result._array;
        }

        // --------------------------------------------------------------------------------------------------------------
        private static double[,] ExtractUpper(double[,] matrix)
        {
            // upper part of a Doolittle decomposition (0.0s in the strictly lower part)
            int rows = matrix.GetLength(0); int cols = matrix.GetLength(1);
            var result = new Matrix(rows, cols);
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    if (i <= j)
                        result[i, j] = matrix[i, j];
                }
            }
            return result._array;
        }

        #endregion
    }
}
