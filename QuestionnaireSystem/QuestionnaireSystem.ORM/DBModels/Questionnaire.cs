namespace QuestionnaireSystem.ORM.DBModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Questionnaire")]
    public partial class Questionnaire
    {
        [Key]
        public int QuesID { get; set; }

        public Guid QuesGuid { get; set; }

        [Required]
        [StringLength(100)]
        public string Caption { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime CreateDate { get; set; }

        public int State { get; set; }

        public int Count { get; set; }
    }
}
