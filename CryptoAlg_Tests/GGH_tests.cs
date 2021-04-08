using CryptoAlg;
using NUnit.Framework;

namespace CryptoAlg_Tests
{
    public class GGH_tests
    {
        Matrix privateKey;
        Matrix unimodular;

        [SetUp]
        public void Init()
        {
            privateKey = new Matrix(new double[,] { { 7, 0 }, { 0, 3 } });
            unimodular = new Matrix(new double[,] { { 2, 3 }, { 3, 5 } });
        }

        [Test]
        public void Encrypt_CorrectResult()
        {
            var publicKey = GGH.GeneratePublicKey(privateKey, unimodular);

            Vector message = new Vector(new double[] { 3, -7 });
            Vector error = new Vector(new double[] { 1, -1 });

            var c_actual = GGH.Encrypt(message, publicKey, error);

            var c_expected = new Vector(new double[] { -104, -79 });

            Assert.True(c_actual.Equals(c_expected));
        }

        [Test]
        public void Decrypt_CorrectResult()
        {
            var publicKey = GGH.GeneratePublicKey(privateKey, unimodular);

            Vector c = new Vector(new double[] { -104, -79 });

            var message_expected = new Vector(new double[] { 3, -7 });

            var message_actual = GGH.Decrypt(c, publicKey, privateKey);

            Assert.True(message_actual.Equals(message_expected));
        }

        [Test]
        public void EncryptDecrypt_GenerateUnimodular_CorrectResult()
        {
            var publicKey = GGH.GeneratePublicKey(privateKey);

            Vector message = new Vector(new double[] { 3, -7 });
            Vector error = new Vector(new double[] { 1, -1 });

            var c_actual = GGH.Encrypt(message, publicKey, error);

            var message_decrypted = GGH.Decrypt(c_actual, publicKey, privateKey);

            Assert.True(message_decrypted.Equals(message));
        }
    }
}
