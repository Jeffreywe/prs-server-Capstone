﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace prs_server.Models {
    public class Vendor {

        public int Id { get; set; }
        [Required, StringLength(30)]
        public string Code { get; set; }
        [Required, StringLength(30)]
        public string Name { get; set; }
        [Required, StringLength(30)]
        public string Address { get; set; }
        [Required, StringLength(30)]
        public string City { get; set; }
        [Required, StringLength(2)]
        public string State { get; set; }
        [Required, StringLength(5)]
        public string Zip { get; set; }
        [StringLength(12)]
        public string Phone { get; set; }
        [StringLength(255)]
        public string Email { get; set; }

        public virtual IEnumerable<Product> Products { get; set; }

        public Vendor() { }
    }
}
