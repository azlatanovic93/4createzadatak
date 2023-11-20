using FourCreate.Domain.Entities.Employee.Enumerations;
using Shouldly;

namespace FourCreate.Test.Domain.Employee
{
    public class TitleTests
    {
        [Theory]
        [InlineData("NONE")]
        [InlineData("DEVELOPER")]
        [InlineData("MANAGER")]
        [InlineData("TESTER")]
        public void Create_valid_title(string value)
        {
            Title title = Title.FromNullableName(value);
            title.DisplayName.ShouldBe(value);
        }

        [Theory]
        [InlineData("None")]
        [InlineData("DEVELOPER1")]
        [InlineData("BadSet")]
        public void Title_is_null(string value)
        {
            Title title = Title.FromNullableName(value);
            title.ShouldBeNull();
        }

    }
}
