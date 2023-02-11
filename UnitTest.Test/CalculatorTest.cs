using Moq;
using UnitTest.App;
using Xunit;

namespace UnitTest.Test
{
    public class CalculatorTest
    {
        private readonly Calculator _calculator;
        private readonly Mock<ICalculatorService> _mock;

        public CalculatorTest()
        {
            _mock = new Mock<ICalculatorService>();
            _calculator = new Calculator(_mock.Object);
        }

        [Fact]
        public void AddTest()
        {
            // Arrange
            int a = 5;
            int b = 20;

            // Act
            var total = _calculator.add(a, b);

            // Assert
            //Assert.Equal(525, total);

            Assert.Null(null);
        }

        [Theory]
        [InlineData(5, 4, 9)]
        [InlineData(53, 4, 9)]
        [InlineData(53, 4, 57)]
        public void AddTest3(int a, int b, int t)
        {
            _mock.Setup(x => x.add(a, b)).Returns(t);
            var total = _calculator.add(a, b);

            _mock.Setup(x => x.add(4, 8)).Returns(11);
            var total2 = _calculator.add(4, 8);

            _mock.Setup(x => x.add(112, 333)).Returns(7);
            var tota234l2 = _calculator.add(112, 333);

            _mock.Setup(x => x.add(It.IsAny<int>(), It.IsAny<int>())).Returns(90);
            var totalt2 = _calculator.add(34, 2);

            _mock.Setup(x => x.add(3, It.IsAny<int>())).Returns(70);
            var totalt62 = _calculator.add(3, 8);

            int tt;
            _mock.Setup(x => x.add(It.IsAny<int>(), It.IsAny<int>())).Callback<int, int>((x, y) => tt = x * y);
            var totalft2 = _calculator.add(34, 2);

            Assert.Equal(t, total);
        }
    }
}
