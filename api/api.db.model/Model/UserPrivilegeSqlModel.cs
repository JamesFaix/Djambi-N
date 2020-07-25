﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Apex.Api.Enums;

namespace Apex.Api.Db.Model
{
    [Table("UserPrivileges")]
    public class UserPrivilegeSqlModel
    {
        [Key]
        [Required]
        public int UserPrivilegeId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public Privilege PrivilegeId { get; set; }
    }
}
