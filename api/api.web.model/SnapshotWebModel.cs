namespace Apex.Api.Web.Model
{
    public class SnapshotInfo
    {
        public int Id { get; set; }
        public CreationSource CreatedBy { get; set; }
        public string Description { get; set; }
    }

    public class CreateSnapshotRequest
    {
        public string Description { get; set; }
    }
}
