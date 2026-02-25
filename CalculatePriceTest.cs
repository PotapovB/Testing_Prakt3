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
        const int MORNING_TIME_HOURS = 8;
        const int EVENING_TIME_HOURS = 8;

        // ÷≈Õ€
        const decimal FREE = 0;
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
            TicketRequest request = new TicketRequest() { Age = age, IsStudent = false, SessionTime = TimeSpan.FromHours(MORNING_TIME_HOURS), Day = DayOfWeek.Monday, IsVip = false };

            decimal result = calculator.CalculatePrice(request);

            Assert.Equal(FREE, result);
        }

        [Theory]
        [InlineData(MIN_CHILD_DISCOUNT_AGE)]
        [InlineData(MIN_STUDENT_DISCOUNT_AGE - 1)]
        public void CalculatePrice_ShouldReturn_40_PercentDiscount_Age(int age)
        {
            TicketRequest request = new TicketRequest() { Age = age, IsStudent = false, SessionTime = TimeSpan.FromHours(EVENING_TIME_HOURS), Day = DayOfWeek.Monday, IsVip = false };

            decimal result = calculator.CalculatePrice(request);

            Assert.Equal(DISCOUNTED_40_PERCENT, result);
        }

        [Theory]
        [InlineData(MIN_STUDENT_DISCOUNT_AGE)]
        [InlineData(MAX_STUDENT_DISCOUNT_AGE)]
        public void CalculatePrice_ShouldReturn_BasePrice_ForStudentAge_ButIsNotStudent(int age)
        {
            TicketRequest request = new TicketRequest() { Age = age, IsStudent = false, SessionTime = TimeSpan.FromHours(EVENING_TIME_HOURS), Day = DayOfWeek.Monday, IsVip = false };

            decimal result = calculator.CalculatePrice(request);

            Assert.Equal(BASE_PRICE, result);
        }

        [Theory]
        [InlineData(MIN_STUDENT_DISCOUNT_AGE)]
        [InlineData(MAX_STUDENT_DISCOUNT_AGE)]
        public void CalculatePrice_ShouldReturn_20_PercentDiscount_ForStudentAge_AndIsStudent(int age)
        {
            TicketRequest request = new TicketRequest() { Age = age, IsStudent = true, SessionTime = TimeSpan.FromHours(EVENING_TIME_HOURS), Day = DayOfWeek.Monday, IsVip = false };

            decimal result = calculator.CalculatePrice(request);

            Assert.Equal(DISCOUNTED_20_PERCENT, result);
        }

        [Theory]
        [InlineData(MAX_STUDENT_DISCOUNT_AGE + 1)]
        [InlineData(MIN_ELDER_DISCOUNT_AGE - 1)]
        public void CalculatePrice_ShouldReturn_BasePrice(int age)
        {
            TicketRequest request = new TicketRequest() { Age = age, IsStudent = false, SessionTime = TimeSpan.FromHours(EVENING_TIME_HOURS), Day = DayOfWeek.Monday, IsVip = false };

            decimal result = calculator.CalculatePrice(request);

            Assert.Equal(BASE_PRICE, result);
        }

        [Theory]
        [InlineData(MIN_ELDER_DISCOUNT_AGE)]
        [InlineData(MAX_AGE)]
        public void CalculatePrice_ShouldReturn_50_PercentDiscount_ForElderAge(int age)
        {
            TicketRequest request = new TicketRequest() { Age = age, IsStudent = false, SessionTime = TimeSpan.FromHours(EVENING_TIME_HOURS), Day = DayOfWeek.Monday, IsVip = false };

            decimal result = calculator.CalculatePrice(request);

            Assert.Equal(DISCOUNTED_50_PERCENT, result);
        }

        [Fact]
        [InlineData(MORNING_TIME_HOURS)]
        public void CalculatePrice_ShouldReturn_15_PercentDiscount_ForMorningTime()
        {
            TicketRequest request = new TicketRequest() { Age = MAX_STUDENT_DISCOUNT_AGE + 1, IsStudent = false, SessionTime = TimeSpan.FromHours(MORNING_TIME_HOURS), Day = DayOfWeek.Monday, IsVip = false };

            decimal result = calculator.CalculatePrice(request);

            Assert.Equal(DISCOUNTED_15_PERCENT, result);
        }

        [Theory]
        [InlineData(MIN_ELDER_DISCOUNT_AGE, MORNING_TIME_HOURS, DayOfWeek.Monday)] // morning
        [InlineData(MIN_ELDER_DISCOUNT_AGE, EVENING_TIME_HOURS, DayOfWeek.Wednesday)] //wednesday
        [InlineData(MIN_ELDER_DISCOUNT_AGE, MORNING_TIME_HOURS, DayOfWeek.Wednesday)] //morning+wednesday
        public void CalculatePrice_50_PercentDiscount_OverridesOtherDiscounts(int age, int session_time_hours, DayOfWeek day)
        {
            TicketRequest request = new TicketRequest() { Age = age, IsStudent = false, SessionTime = TimeSpan.FromHours(session_time_hours), Day = day, IsVip = false };

            decimal result = calculator.CalculatePrice(request);

            Assert.Equal(DISCOUNTED_50_PERCENT, result);
        }

        [Theory]
        [InlineData(MIN_CHILD_DISCOUNT_AGE, MORNING_TIME_HOURS, DayOfWeek.Monday)] // morning
        [InlineData(MIN_CHILD_DISCOUNT_AGE, EVENING_TIME_HOURS, DayOfWeek.Wednesday)] // wednesday
        [InlineData(MIN_CHILD_DISCOUNT_AGE, MORNING_TIME_HOURS, DayOfWeek.Wednesday)] // morning+wednesday
        public void CalculatePrice_40_PercentDiscount_OverridesOtherDiscounts(int age, int session_time_hours, DayOfWeek day)
        {
            TicketRequest request = new TicketRequest() { Age = age, IsStudent = false, SessionTime = TimeSpan.FromHours(session_time_hours), Day = day, IsVip = false };

            decimal result = calculator.CalculatePrice(request);

            Assert.Equal(DISCOUNTED_40_PERCENT, result);
        }

        [Theory]
        [InlineData(MIN_STUDENT_DISCOUNT_AGE, true, EVENING_TIME_HOURS)] // student
        [InlineData(MIN_STUDENT_DISCOUNT_AGE, false, MORNING_TIME_HOURS)] // morning
        [InlineData(MIN_STUDENT_DISCOUNT_AGE, true, MORNING_TIME_HOURS)] // student+morning
        public void CalculatePrice_30_PercentDiscount_OverridesOtherDiscounts(int age, bool is_student, int session_time_hours)
        {
            TicketRequest request = new TicketRequest() { Age = age, IsStudent = is_student, SessionTime = TimeSpan.FromHours(session_time_hours), Day = DayOfWeek.Wednesday, IsVip = false };

            decimal result = calculator.CalculatePrice(request);

            Assert.Equal(DISCOUNTED_30_PERCENT, result);
        }

        [Fact]
        public void CalculatePrice_20_PercentDiscount_OverridesOtherDiscounts()
        {
            TicketRequest request = new TicketRequest() { Age = MIN_STUDENT_DISCOUNT_AGE, IsStudent = true, SessionTime = TimeSpan.FromHours(MORNING_TIME_HOURS), Day = DayOfWeek.Monday, IsVip = false };

            decimal result = calculator.CalculatePrice(request);

            Assert.Equal(DISCOUNTED_20_PERCENT, result);
        }

        [Fact]
        public void CalculatePrice_Applies_VipModifier_For_BasePrice()
        {
            TicketRequest request = new TicketRequest() { Age = MAX_STUDENT_DISCOUNT_AGE + 1, IsStudent = false, SessionTime = TimeSpan.FromHours(EVENING_TIME_HOURS), Day = DayOfWeek.Monday, IsVip = true };

            decimal result = calculator.CalculatePrice(request);

            Assert.Equal(BASE_PRICE * VIP_MODIFIER, result);
        }
    }
}