namespace Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class booking
    {
        public int id { get; set; }

        public int? room_id { get; set; }

        public int? user_id { get; set; }

        [Column(TypeName = "date")]
        public DateTime? booking_date { get; set; }

        public bool? booking_status { get; set; }

        public byte? confirmation_status { get; set; }

        public virtual account account { get; set; }

        public virtual room room { get; set; }
    }
}
