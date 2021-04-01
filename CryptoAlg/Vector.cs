using System;
namespace CryptoAlg
{
    public class Vector: IEquatable<Vector>
    {
        double[] _array;

        public Vector(double[] array)
        {
            _array = array;
        }

        public Vector(int length)
        {
            _array = new double[length];
        }

        public int Length { get => _array.Length; }

        public double this[int index]
        {
            get
            {
                return _array[index];
            }
            set
            {
                _array[index] = value;
            }
        }

        public static Vector operator +(Vector vector1, Vector vector2)
        {
            if (vector1 == null)
                throw new ArgumentNullException();

            if (vector2 == null)
                throw new ArgumentNullException();

            if (vector1.Length != vector2.Length)
                throw new ArgumentException();

            var result = new Vector(vector1.Length);

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = vector1[i] + vector2[i];
            }

            return result;
        }

        public static Vector operator -(Vector vector1, Vector vector2)
        {
            if (vector1 == null)
                throw new ArgumentNullException();

            if (vector2 == null)
                throw new ArgumentNullException();

            if (vector1.Length != vector2.Length)
                throw new ArgumentException();

            var result = new Vector(vector1.Length);

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = vector1[i] - vector2[i];
            }

            return result;
        }

        public static Vector operator *(double value, Vector vector)
        {
            if (vector == null)
                throw new ArgumentNullException();


            var result = new Vector(vector.Length);

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = vector[i] * value;
            }

            return result;
        }

        public double DotProduct(Vector vector)
        {
            if (_array == null)
                return 0;

            if (vector == null)
                return 0;

            if (_array.Length != vector.Length)
                return 0;

            double result = 0;
            for (int x = 0; x < _array.Length; x++)
            {
                result += _array[x] * vector[x];
            }

            return result;
        }

        public Vector MatMul(Matrix other)
        {
            var vectorLength = Length;
            var matrixRowCount = other.GetLength(0);
            var matrixColCount = other.GetLength(1);

            if (vectorLength != matrixRowCount)
                throw new InvalidOperationException
                  ("Product is undefined. The length of the vector must equal to # of rows of the matrix");

            Vector product = new Vector(matrixColCount);

            for (int matrixCol = 0; matrixCol < matrixColCount; matrixCol++)
            {
                for (int vectorPosition = 0; vectorPosition < vectorLength; vectorPosition++)
                {
                    product[matrixCol] += this[vectorPosition] * other[vectorPosition, matrixCol];
                }
            }

            return product;
        }

        public Vector RoundInt()
        {
            var result = new Vector(this.Length);

            for (int i = 0; i < this.Length; i++)
            {
                result[i] = Math.Round(this[i]);
            }

            return result;
        }

        public bool Equals(Vector other)
        {
            if (other.Length != this.Length)
            {
                return false;
            }

            for (int i = 0; i < Length; i++)
            {
                double difference = Math.Abs(this[i] * .00001);

                if (Math.Abs(this[i] - other[i]) > difference)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
