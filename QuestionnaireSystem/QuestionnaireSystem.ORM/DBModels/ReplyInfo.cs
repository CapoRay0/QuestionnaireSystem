namespace QuestionnaireSystem.ORM.DBModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReplyInfo")]
    public partial class ReplyInfo
    {
        [Key]
        public Guid UserGuid { get; set; }

        public Guid QuesGuid { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        public int Age { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
