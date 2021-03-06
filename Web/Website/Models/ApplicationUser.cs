﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Data;
using Data.Models;

namespace Website.Models
{
    public class Comparison
    {
        public string ChangeSetsJson { get; set; }
        public List<ContentHistoryViewModel> ChangeSets { get; set; }
        public Content Old { get; set; }
        public Content New { get; set; }
    }

    public class ContentHistoryViewModel
    {
        public ContentHistoryViewModel(ContentHistory contentHistory)
        {
            Changed = contentHistory.Changed;
            ChangedField = contentHistory.ChangedField;
            OldValue = contentHistory.OldValue;
            ChangedBy = UserApi.GetById(contentHistory.ChangedByUserId);
        }

        public string ChangedField { get; set; }
        public string OldValue { get; set; }
        public DateTime Changed { get; set; }
        public User ChangedBy { get; set; }
    }

    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
    }
}
