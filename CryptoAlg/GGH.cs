using System;

namespace CryptoAlg
{
    public static class GGH
    {
		public static Matrix GeneratePublicKey(Matrix privateKey, Matrix unimodular)
        {
			return unimodular.MatMul(privateKey);
        }

		public static Matrix GeneratePublicKey(Matrix privateKey)
		{
			var unimodular = GenerateUnimodularMatrix(privateKey.GetLength(0));
			return unimodular.MatMul(privateKey);
		}

		private static Matrix GenerateUnimodularMatrix(int dimension)
        {
			var matrix = Matrix.Identity(dimension);
			int k = 5;

            while (k != 0)
            {
				var A = Matrix.RandInt(-5, 5, (dimension, dimension));
				var det = Matrix.Determinant(A);

				if (Math.Abs(det) == 1.0)
                {
					matrix = matrix.MatMul(A);
					k--;
                }
            }

			return matrix;

        }

		public static Vector Encrypt(Vector message, Matrix publicKey, Vector error)
        {
			var c = message.MatMul(publicKey) + error;

			return c.RoundInt();
        }

		public static Vector Decrypt(Vector message, Matrix publicKey, Matrix privateKey)
		{
			var U = message.MatMul(privateKey.Inverse());
			U = U.RoundInt().MatMul(privateKey);

			var result = U.MatMul(publicKey.Inverse());

			return result.RoundInt();
		}
	}
}
