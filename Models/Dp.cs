﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class Dp
    {
        [Required]
        public byte[] ImageData { get; set; }

    }
}