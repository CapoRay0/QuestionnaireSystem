namespace QuestionnaireSystem.ORM.DBModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Static")]
    public partial class Static
    {
        public int StaticID { get; set; }

        public Guid QuesGuid { get; set; }

        public Guid ProbGuid { get; set; }

        [StringLength(1000)]
        public string OptionText { get; set; }

        public int Count { get; set; }
    }
}
