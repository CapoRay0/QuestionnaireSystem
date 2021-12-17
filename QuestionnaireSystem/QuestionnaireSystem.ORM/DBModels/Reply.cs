namespace QuestionnaireSystem.ORM.DBModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Reply")]
    public partial class Reply
    {
        public int ReplyID { get; set; }

        public Guid UserGuid { get; set; }

        public Guid ProbGuid { get; set; }

        [StringLength(1000)]
        public string AnswerText { get; set; }
    }
}
