using CinemaTicketSystem;
using System.Diagnostics.CodeAnalysis;

namespace CinemaTicketSystemTests
{
    public class CalculatePriceTest
    {
        // ¬Œ«–¿—“
        const int MIN_AGE = 0;
        const int MAX_AGE = 120;

        const int MIN_CHILD_DISCOUNT_AGE = 6;

        const int MIN_STUDENT_DISCOUNT_AGE = 18;
        const int MAX_STUDENT_DISCOUNT_AGE = 25;

        const int MIN_ELDER_DISCOUNT_AGE = 65;

        // ¬–≈Ãﬂ
        TimeSpan MORNING_TIME = new(8, 0, 0);
        TimeSpan EVENING_TIME = new(16, 0, 0);

        // ƒ¿“¿

        // ÷≈Õ€
        const decimal BASE_PRICE = 300;
        const decimal DISCOUNTED_15_PERCENT = 255;
        const decimal DISCOUNTED_20_PERCENT = 240;
        const decimal DISCOUNTED_30_PERCENT = 210;
        const decimal DISCOUNTED_40_PERCENT = 180;
        const decimal DISCOUNTED_50_PERCENT = 150;

        const decimal VIP_MODIFIER = 2;


        TicketPriceCalculator calculator;

        public CalculatePriceTest()
        {
            calculator = new TicketPriceCalculator();
        }

        [Fact]
        public void CalculatePrice_ShouldThrow_ArgumentNullException_ForNull()
        {
            Assert.Throws<ArgumentNullException>(() => calculator.CalculatePrice(null));
        }

        [Theory]
        [InlineData(MIN_AGE - 1)]
        [InlineData(MAX_AGE + 1)]
        public void CalculatePrice_ShouldThrow_ArgumentOutOfRangeException_ForOutOfRangeAge(int age)
        {
            TicketRequest request = new TicketRequest() { Age = age };

            Assert.Throws<ArgumentOutOfRangeException>(() => calculator.CalculatePrice(request));
        }

        [Theory]
        [InlineData(MIN_AGE)]
        [InlineData(MAX_AGE)]
        public void CalculatePrice_ShouldWork_ForBorderAge(int age)
        {
            TicketRequest request = new TicketRequest() { Age = age };

            calculator.CalculatePrice(request);
        }

        [Theory]
        [InlineData(MIN_AGE)]
        [InlineData(MIN_CHILD_DISCOUNT_AGE - 1)]
        public void CalculatePrice_ShouldReturnZero_ForAgeLessThan_6(int age)
        {
            TicketRequest request = new TicketRequest() { Age = age, IsStudent = false, SessionTime = EVENING_TIME, Day = DayOfWeek.Monday, IsVip = false };

            decimal result = calculator.CalculatePrice(request);

            Assert.Equal(0, result);
        }

        [Theory]
        [InlineData(MIN_CHILD_DISCOUNT_AGE)]
        [InlineData(MIN_STUDENT_DISCOUNT_AGE - 1)]
        public void CalculatePrice_ShouldReturn_40_PercentDiscount_Age(int age)
        {
            TicketRequest request = new TicketRequest() { Age = age, IsStudent = false, SessionTime = EVENING_TIME, Day = DayOfWeek.Monday, IsVip = false };

            decimal result = calculator.CalculatePrice(request);

            Assert.Equal(DISCOUNTED_40_PERCENT, result);
        }

        [Theory]
        [InlineData(MIN_STUDENT_DISCOUNT_AGE)]
        [InlineData(MAX_STUDENT_DISCOUNT_AGE)]
        public void CalculatePrice_ShouldReturn_BasePrice_ForStudentAge_ButIsNotStudent_NoMorningDiscount(int age)
        {
            TicketRequest request = new TicketRequest() { Age = age, IsStudent = false, SessionTime = EVENING_TIME, Day = DayOfWeek.Monday, IsVip = false };

            decimal result = calculator.CalculatePrice(request);

            Assert.Equal(BASE_PRICE, result);
        }

        [Theory]
        [InlineData(MIN_STUDENT_DISCOUNT_AGE)]
        [InlineData(MAX_STUDENT_DISCOUNT_AGE)]
        public void CalculatePrice_ShouldReturn_20_PercentDiscount_ForStudentAge_AndIsStudent(int age)
        {
            TicketRequest request = new TicketRequest() { Age = age, IsStudent = true, SessionTime = EVENING_TIME, Day = DayOfWeek.Monday, IsVip = false };

            decimal result = calculator.CalculatePrice(request);

            Assert.Equal(DISCOUNTED_20_PERCENT, result);
        }

        [Theory]
        [InlineData(MAX_STUDENT_DISCOUNT_AGE + 1)]
        [InlineData(MIN_ELDER_DISCOUNT_AGE - 1)]
        public void CalculatePrice_ShouldReturn_BasePrice(int age)
        {
            TicketRequest request = new TicketRequest() { Age = age, IsStudent = false, SessionTime = EVENING_TIME, Day = DayOfWeek.Monday, IsVip = false };

            decimal result = calculator.CalculatePrice(request);

            Assert.Equal(BASE_PRICE, result);
        }

        [Theory]
        [InlineData(MIN_ELDER_DISCOUNT_AGE)]
        [InlineData(MAX_AGE)]
        public void CalculatePrice_ShouldReturn_50_PercentDiscount_ForElderAge(int age)
        {
            TicketRequest request = new TicketRequest() { Age = age, IsStudent = false, SessionTime = EVENING_TIME, Day = DayOfWeek.Monday, IsVip = false };

            decimal result = calculator.CalculatePrice(request);

            Assert.Equal(DISCOUNTED_50_PERCENT, result);
        }

        [Theory]
        [InlineData(MIN_ELDER_DISCOUNT_AGE)]
        [InlineData(MAX_AGE)]
        public void CalculatePrice_50_PercentDiscount_OverridesOtherDiscounts(int age)
        {
            TicketRequest request = new TicketRequest() { Age = age, IsStudent = false, SessionTime = MORNING_TIME, Day = DayOfWeek.Wednesday, IsVip = false };

            decimal result = calculator.CalculatePrice(request);

            Assert.Equal(DISCOUNTED_50_PERCENT, result);
        }

        [Theory]
        [InlineData(MAX_STUDENT_DISCOUNT_AGE + 1)]
        [InlineData(MIN_ELDER_DISCOUNT_AGE - 1)]
        public void CalculatePrice_40_PercentDiscount_OverridesOtherDiscounts(int age)
        {
            TicketRequest request = new TicketRequest() { Age = age, IsStudent = false, SessionTime = MORNING_TIME, Day = DayOfWeek.Wednesday, IsVip = false };

            decimal result = calculator.CalculatePrice(request);

            Assert.Equal(DISCOUNTED_50_PERCENT, result);
        }
    }
}