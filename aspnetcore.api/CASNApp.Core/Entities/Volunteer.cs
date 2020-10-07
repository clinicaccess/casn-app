﻿using System;
using System.Collections.Generic;

namespace CASNApp.Core.Entities
{
    public partial class Volunteer
    {
        public Volunteer()
        {
            Appointments = new HashSet<Appointment>();
            Approvals = new HashSet<Drive>();
            Drives = new HashSet<Drive>();
            VolunteerBadges = new HashSet<VolunteerBadge>();
            VolunteerDriveLogs = new HashSet<VolunteerDriveLog>();
            FundingOffers = new HashSet<FundingOffer>();
        }

        public int Id { get; set; }
        public int CiviContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobilePhone { get; set; }
        public string GoogleAccount { get; set; }
        public bool IsDriver { get; set; }
        public bool IsDispatcher { get; set; }
        public bool HasTextConsent { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime? Geocoded { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<Drive> Approvals { get; set; }
        public virtual ICollection<Drive> Drives { get; set; }
        public virtual ICollection<VolunteerBadge> VolunteerBadges { get; set; }
        public virtual ICollection<VolunteerDriveLog> VolunteerDriveLogs { get; set; }
        public virtual ICollection<FundingOffer> FundingOffers { get; set; }
    }
}
