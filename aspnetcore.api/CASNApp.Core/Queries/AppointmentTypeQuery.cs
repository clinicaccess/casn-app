﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CASNApp.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CASNApp.Core.Queries
{
    public class AppointmentTypeQuery
    {
        private readonly casn_appContext dbContext;

        public AppointmentTypeQuery(casn_appContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<List<AppointmentType>> GetActiveAppointmentTypesAsync(bool readOnly)
        {
            var result = (readOnly ? dbContext.AppointmentTypes.AsNoTracking() : dbContext.AppointmentTypes)
                .Where(e => e.IsActive)
                .OrderBy(e => e.Id)
                .ToListAsync();

            return result;
        }

        public AppointmentType GetById(int appointmentTypeId)
        {
            return dbContext.AppointmentTypes.Find(appointmentTypeId);
        }

    }
}
