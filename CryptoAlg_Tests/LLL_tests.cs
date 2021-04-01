using CryptoAlg;
using NUnit.Framework;
namespace CryptoAlg_Tests
{
    public class LLL_tests
    {
        Matrix basis;

        [SetUp]
        public void Init()
        {
            basis = new Matrix(new double[,] { { 1, 1, 1 }, { -1, 0, 2 }, { 3, 5, 6 } });
        }
        
        [Test]
        public void GramSchmidt_CorrectResult()
        {
            Matrix w_actual;
            Matrix u_actual;

            LLL.GramSchmidt(basis, out w_actual, out u_actual);

            Matrix w_expected = new Matrix(new double[,] { { 1.0, 1.0, 1.0 }, { -4.0/3.0, -1.0/3.0, 5.0/3.0 }, { -6.0/14.0, 9.0/14.0, -3.0/14.0 } });
            Matrix u_expected = new Matrix(new double[,] { { 0.0, 0.0, 0.0 }, { 1.0 / 3.0, 0.0, 0.0 }, { 14.0 / 3.0, 13.0 / 14.0, 0.0 } });

            Assert.True(w_actual.Equals(w_expected));
            Assert.True(u_actual.Equals(u_expected));
        }

        [Test]
        public void LLLreduction_CorrectResult()
        {
            var lll_actual = LLL.LLLreduction(basis);

            Matrix lll_expected = new Matrix(new double[,] { { 0.0, 1.0, 0.0 }, { 1.0, 0.0, 1.0 }, { -1.0, 0.0, 2.0 } });

            Assert.True(lll_actual.Equals(lll_expected));
        }
    }
}
