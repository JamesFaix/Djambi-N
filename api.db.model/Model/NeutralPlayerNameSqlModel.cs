using System.ComponentModel.DataAnnotations;

namespace Apex.Api.Db.Model
{
    public class NeutralPlayerNameSqlModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
