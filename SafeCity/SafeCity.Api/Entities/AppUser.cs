﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeCity.Api.Entity
{
    public class AppUser : IdentityUser<int>
    {
        public string? FirstName { get; set; }
        public string? FullName { get; set; }
        public string? SecondName { get; set; }
    }
}
