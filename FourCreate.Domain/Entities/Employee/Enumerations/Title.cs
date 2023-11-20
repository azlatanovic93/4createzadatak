using FourCreate.Domain.Entities.Common.Enumerations;

namespace FourCreate.Domain.Entities.Employee.Enumerations
{
    public class Title : Enumeration<Title, int>
    {
        public static readonly Title None = new Title(0, "NONE");
        public static readonly Title Developer = new Title(1, "DEVELOPER");
        public static readonly Title Manager = new Title(2, "MANAGER");
        public static readonly Title Tester = new Title(3, "TESTER");

        private Title(int value, string displayName) : base(value, displayName) { }
    }

}
