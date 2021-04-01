using System;
using System.Linq;

namespace CryptoAlg
{
    public static class LLL
    {
        public static void GramSchmidt(Matrix basis, out Matrix orthogonalBasis, out Matrix b)
        {
            int dimension = basis.GetLength(0);

            orthogonalBasis = new Matrix(dimension, dimension);
            orthogonalBasis[0] = basis[0];
            b = new Matrix(dimension, dimension);

            for (int n = 1; n < dimension; n++)
            {
                var sum = new Vector(dimension);
                Vector ortVector;

                for (int j = 0; j < n; j++)
                {
                    ortVector = orthogonalBasis[j];
                    b[n, j] = basis[n].DotProduct(ortVector) / ortVector.DotProduct(ortVector);
                }

                for (int s = 0; s < n; s++)
                {
                    sum += b[n, s] * orthogonalBasis[s];
                }

                orthogonalBasis[n] = basis[n] - sum;
            }

        }

        public static Matrix LLLreduction(Matrix basis)
        {

            Matrix w;
            Matrix u;

            GramSchmidt(basis, out w, out u);

            int k = 1;
            int dimension = basis.GetLength(0);

            while (k < dimension)
            {
                for (int j = k - 1; j > -1; j--)
                {
                    if(Math.Abs(u[k, j]) > 0.5)
                    {
                        basis[k] -= Math.Round(u[k, j]) * basis[j];
                        GramSchmidt(basis, out w, out u);
                    }
                }

                if(w[k].DotProduct(w[k]) >= (0.75 - u[k][k - 1] * u[k][k - 1]) * w[k - 1].DotProduct(w[k - 1]))
                {
                    k++;
                }
                else
                {
                    basis.TransposeRows(k, k - 1);
                    GramSchmidt(basis, out w, out u);
                    k = Math.Max(k - 1, 1);
                }
            }

            return basis;
        }
    }
}
