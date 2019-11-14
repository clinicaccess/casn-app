﻿using System;
using System.Collections.Generic;

namespace CASNApp.Core.Entities
{
    public class ServiceProviderType
    {
        public ServiceProviderType()
        {
            ServiceProviders = new HashSet<ServiceProvider>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public ICollection<ServiceProvider> ServiceProviders { get; set; }
    }
}