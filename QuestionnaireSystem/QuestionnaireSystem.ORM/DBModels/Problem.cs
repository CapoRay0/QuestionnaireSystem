namespace QuestionnaireSystem.ORM.DBModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Problem")]
    public partial class Problem
    {
        [Key]
        public Guid ProbGuid { get; set; }

        public Guid QuesGuid { get; set; }

        public int Count { get; set; }

        [Required]
        [StringLength(1000)]
        public string Text { get; set; }

        public int SelectionType { get; set; }

        public bool IsMust { get; set; }

        [StringLength(1000)]
        public string Selection { get; set; }
    }
}
