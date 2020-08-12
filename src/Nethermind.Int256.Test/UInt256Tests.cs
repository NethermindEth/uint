using System;
using System.Numerics;
using FluentAssertions;
using NUnit.Framework;

namespace Nethermind.Int256.Test
{
    public partial class UInt256Tests<T> where T : IInteger<T>
    {
        protected readonly Func<BigInteger, T> convert;
        protected readonly Func<int, T> convertFromInt;
        protected readonly Func<BigInteger, BigInteger> postprocess;
        protected readonly BigInteger maxValue;

        protected UInt256Tests(Func<BigInteger, T> convert, Func<int, T> convertFromInt, Func<BigInteger, BigInteger> postprocess, BigInteger maxValue)
        {
            this.convert = convert;
            this.convertFromInt = convertFromInt;
            this.postprocess = postprocess;
            this.maxValue = maxValue;
        }

        [TestCaseSource(typeof(BinaryOps), nameof(BinaryOps.TestCases))]
        public virtual void Add((BigInteger A, BigInteger B) test)
        {
            BigInteger resBigInt = test.A + test.B;
            resBigInt = resBigInt % (BigInteger.One << 256);
            resBigInt = postprocess(resBigInt);

            T a = convert(test.A);
            T b = convert(test.B);
            a.Add(b, out T res);
            res.Convert(out BigInteger resUInt256);

            resUInt256.Should().Be(resBigInt);
        }

        [TestCaseSource(typeof(TernaryOps), nameof(TernaryOps.TestCases))]
        public virtual void AddMod((BigInteger A, BigInteger B, BigInteger M) test)
        {
            if (test.M.IsZero)
            {
                return;
            }
            BigInteger resBigInt = (test.A + test.B) % test.M;
            resBigInt = resBigInt % (BigInteger.One << 256);
            resBigInt = postprocess(resBigInt);

            T uint256a = convert(test.A);
            T uint256b = convert(test.B);
            T uint256m = convert(test.M);
            uint256a.AddMod(uint256b, uint256m, out T res);
            res.Convert(out BigInteger resUInt256);

            resUInt256.Should().Be(resBigInt);
        }

        [TestCaseSource(typeof(BinaryOps), nameof(BinaryOps.TestCases))]
        public virtual void Subtract((BigInteger A, BigInteger B) test)
        {
            if (test.A < test.B)
            {
                return;
            }

            BigInteger resBigInt = test.A - test.B;
            resBigInt %= BigInteger.One << 256;
            resBigInt = postprocess(resBigInt);

            T uint256a = convert(test.A);
            T uint256b = convert(test.B);
            uint256a.Subtract(uint256b, out T res);
            res.Convert(out BigInteger resUInt256);

            resUInt256.Should().Be(resBigInt);
        }

        [TestCaseSource(typeof(BinaryOps), nameof(BinaryOps.TestCases))]
        public virtual void Multiply((BigInteger A, BigInteger B) test)
        {
            BigInteger resBigInt = (test.A * test.B) % (BigInteger.One << 256);
            resBigInt = postprocess(resBigInt);

            T uint256a = convert(test.A);
            T uint256b = convert(test.B);
            uint256a.Multiply(uint256b, out T res);
            res.Convert(out BigInteger resUInt256);

            resUInt256.Should().Be(resBigInt);
        }

        [TestCaseSource(typeof(TernaryOps), nameof(TernaryOps.TestCases))]
        public virtual void MultiplyMod((BigInteger A, BigInteger B, BigInteger M) test)
        {
            if (test.M.IsZero)
            {
                return;
            }
            BigInteger resBigInt = ((test.A * test.B) % test.M) % (BigInteger.One << 256);
            resBigInt = postprocess(resBigInt);

            T uint256a = convert(test.A);
            T uint256b = convert(test.B);
            T uint256m = convert(test.M);
            uint256a.MultiplyMod(uint256b, uint256m, out T res);
            res.Convert(out BigInteger resUInt256);

            resUInt256.Should().Be(resBigInt);
        }

        [TestCaseSource(typeof(BinaryOps), nameof(BinaryOps.TestCases))]
        public virtual void Div((BigInteger A, BigInteger B) test)
        {
            if (test.B.IsZero)
            {
                return;
            }
            BigInteger resBigInt = (test.A / test.B) % (BigInteger.One << 256);
            resBigInt = postprocess(resBigInt);

            T uint256a = convert(test.A);
            T uint256b = convert(test.B);
            uint256a.Divide(uint256b, out T res);
            res.Convert(out BigInteger resUInt256);

            resUInt256.Should().Be(resBigInt);
        }

        [TestCaseSource(typeof(BinaryOps), nameof(BinaryOps.ShiftTestCases))]
        public virtual void Exp((BigInteger A, int n) test)
        {
            BigInteger resBigInt = BigInteger.Pow(test.A, test.n);
            resBigInt = resBigInt % (BigInteger.One << 256);
            resBigInt = postprocess(resBigInt);

            T uint256a = convert(test.A);

            uint256a.Exp(convertFromInt(test.n), out T res);
            res.Convert(out BigInteger resUInt256);

            resUInt256.Should().Be(resBigInt);
        }

        [TestCaseSource(typeof(TernaryOps), nameof(TernaryOps.TestCases))]
        public virtual void ExpMod((BigInteger A, BigInteger B, BigInteger M) test)
        {
            if (test.M.IsZero || test.B < 0)
            {
                return;
            }
            BigInteger resBigInt = BigInteger.ModPow(test.A, test.B, test.M);
            resBigInt = resBigInt % (BigInteger.One << 256);
            resBigInt = postprocess(resBigInt);

            T uint256a = convert(test.A);
            T uint256b = convert(test.B);
            T uint256m = convert(test.M);

            uint256a.ExpMod(uint256b, uint256m, out T res);
            res.Convert(out BigInteger resUInt256);

            resUInt256.Should().Be(resBigInt);
        }

        [TestCaseSource(typeof(BinaryOps), nameof(BinaryOps.ShiftTestCases))]
        public virtual void Lsh((BigInteger A, int n) test)
        {
            if (test.n == 0)
            {
                return;
            }
            BigInteger resBigInt = test.A << test.n;
            resBigInt = resBigInt % (BigInteger.One << 256);
            resBigInt = postprocess(resBigInt);

            T uint256a = convert(test.A);
            uint256a.LeftShift(test.n, out T res);
            res.Convert(out BigInteger resUInt256);

            resUInt256.Should().Be(resBigInt);
        }

        [TestCaseSource(typeof(BinaryOps), nameof(BinaryOps.ShiftTestCases))]
        public virtual void Rsh((BigInteger A, int n) test)
        {
            if (test.n == 0)
            {
                return;
            }
            BigInteger resBigInt = test.A >> test.n;
            resBigInt = resBigInt % (BigInteger.One << 256);
            resBigInt = postprocess(resBigInt);

            T uint256a = convert(test.A);
            uint256a.RightShift(test.n, out T res);
            res.Convert(out BigInteger resUInt256);

            resUInt256.Should().Be(resBigInt);
        }

        [TestCaseSource(typeof(UnaryOps), nameof(UnaryOps.TestCases))]
        public virtual void ToBigIntegerAndBack(BigInteger test)
        {
            T uint256 = convert(test);
            uint256.Convert(out BigInteger res);
            res.Should().Be(test);
        }

        [TestCaseSource(typeof(UnaryOps), nameof(UnaryOps.TestCases))]
        public virtual void ToString(BigInteger test)
        {
            T uint256 = convert(test);
            uint256.ToString().Should().Be(test.ToString());
        }
    }

    public class UInt256ImplTests : UInt256Tests<UInt256>
    {
        public UInt256ImplTests() : base((BigInteger x) => (UInt256)x, (int x) => (UInt256)x, x => x, TestNumbers.UInt256Max) {}

        [Test]
        public virtual void Zero_is_min_value()
        {
            convert(1).ZeroValue.Should().Be(UInt256.MinValue);
        }

        [Test]
        public virtual void Zero_is_zero()
        {
            convert(1).ZeroValue.Should().Be(convert(BigInteger.Zero));
        }

        [Test]
        public virtual void Is_zero()
        {
            convert(1).ZeroValue.IsZero.Should().BeTrue();
            UInt256.One.IsZero.Should().BeFalse();
        }

        [Test]
        public virtual void One_is_one()
        {
            convert(1).OneValue.Should().Be(convert(BigInteger.One));
        }

        [Test]
        public virtual void Is_one()
        {
            convert(1).OneValue.IsOne.Should().BeTrue();
            convert(1).ZeroValue.IsOne.Should().BeFalse();
        }

        [Test]
        public virtual void Max_value_is_correct()
        {
            convert(1).MaximalValue.Should().Be(convert(maxValue));
        }
    }

    public class Int256ImplTests : UInt256Tests<Int256>
    {

        private static BigInteger Postprocess(BigInteger big)
        {
            var bytes = big.ToByteArray();
            return new BigInteger(bytes.AsSpan().Slice(0, Math.Min(256/8, bytes.Length)));
        }

        public Int256ImplTests() : base((BigInteger x) => new Int256(x), (int x) => new Int256(x), Postprocess, TestNumbers.Int256Max)
        {
        }

        [TestCaseSource(typeof(BinaryOps), nameof(BinaryOps.SignedTestCases))]
        public override void Add((BigInteger A, BigInteger B) test) => base.Add(test);

        [TestCaseSource(typeof(TernaryOps), nameof(TernaryOps.SignedTestCases))]
        public override void AddMod((BigInteger A, BigInteger B, BigInteger M) test) => base.AddMod(test);

        [TestCaseSource(typeof(BinaryOps), nameof(BinaryOps.SignedTestCases))]
        public override void Subtract((BigInteger A, BigInteger B) test) => base.Subtract(test);

        [TestCaseSource(typeof(TernaryOps), nameof(TernaryOps.SignedTestCases))]
        public void SubtractMod((BigInteger A, BigInteger B, BigInteger M) test)
        {
            if (test.M.IsZero)
            {
                return;
            }

            BigInteger resBigInt = ((test.A - test.B) % test.M) % (BigInteger.One << 256);
            resBigInt = postprocess(resBigInt);

            Int256 uint256a = convert(test.A);
            Int256 uint256b = convert(test.B);
            Int256 uint256m = convert(test.M);
            uint256a.SubtractMod(uint256b, uint256m, out Int256 res);
            res.Convert(out BigInteger resUInt256);

            resUInt256.Should().Be(resBigInt);
        }

        [TestCaseSource(typeof(BinaryOps), nameof(BinaryOps.SignedTestCases))]
        public override void Multiply((BigInteger A, BigInteger B) test) => base.Multiply(test);

        [TestCaseSource(typeof(TernaryOps), nameof(TernaryOps.SignedTestCases))]
        public override void MultiplyMod((BigInteger A, BigInteger B, BigInteger M) test) => base.MultiplyMod(test);

        [TestCaseSource(typeof(BinaryOps), nameof(BinaryOps.SignedTestCases))]
        public override void Div((BigInteger A, BigInteger B) test) => base.Div(test);

        [TestCaseSource(typeof(BinaryOps), nameof(BinaryOps.SignedShiftTestCases))]
        public override void Exp((BigInteger A, int n) test) => base.Exp(test);

        [TestCaseSource(typeof(TernaryOps), nameof(TernaryOps.SignedTestCases))]
        public override void ExpMod((BigInteger A, BigInteger B, BigInteger M) test) => base.ExpMod(test);

        [TestCaseSource(typeof(BinaryOps), nameof(BinaryOps.SignedShiftTestCases))]
        public override void Lsh((BigInteger A, int n) test) => base.Lsh(test);

        [TestCaseSource(typeof(BinaryOps), nameof(BinaryOps.SignedShiftTestCases))]
        public override void Rsh((BigInteger A, int n) test) => base.Rsh(test);

        [TestCaseSource(typeof(UnaryOps), nameof(UnaryOps.TestCasesSigned))]
        public override void ToBigIntegerAndBack(BigInteger test) => base.ToBigIntegerAndBack(test);

        [TestCaseSource(typeof(UnaryOps), nameof(UnaryOps.TestCasesSigned))]
        public override void ToString(BigInteger test) => base.ToString(test);
    }
}
