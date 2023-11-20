using FourCreate.Persistence.DataModels.Common;

namespace FourCreate.Persistence.DataModels
{
    public class SystemLog : BaseDataModel
    {
        public Guid Id { get; set; }
        public string ResourceType { get; set; }
        public string Event { get; set; }
        public string Changeset { get; set; }
        public string Comment { get; set; }
    }
}
