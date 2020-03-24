namespace Apex.Api.Web.Model
{
    public class SnapshotInfoDto
    {
        public int Id { get; set; }
        public CreationSourceDto CreatedBy { get; set; }
        public string Description { get; set; }
    }

    public class CreateSnapshotRequestDto
    {
        public string Description { get; set; }
    }
}
