namespace Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class equipment
    {
        public int id { get; set; }

        [Required]
        [StringLength(255)]
        public string equipment_number { get; set; }

        [Required]
        [StringLength(255)]
        public string equipment_type { get; set; }

        [Required]
        [StringLength(255)]
        public string origin { get; set; }

        public int production_year { get; set; }

        public int? voltage { get; set; }

        public bool status { get; set; }

        public int? room_id { get; set; }

        public bool is_deleted { get; set; }

        public virtual room room { get; set; }
    }
}
