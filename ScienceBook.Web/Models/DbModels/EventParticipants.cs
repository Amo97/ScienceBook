﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ScienceBook.Web.Models.DbModels
{
    [Table("EventParticipants")]
    public class EventParticipant
    {
        public int ID { get; set; }
        public int EventID { get; set; }

        public virtual Event Event { get; set; }
        public virtual List<Member> Members { get; set; }
    }
}