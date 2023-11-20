namespace FourCreate.Persistence.DataModels.Common
{
    public abstract class BaseDataModel
    {
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }

}
