﻿using System;
using System.Collections.Generic;
using System.Linq;
using CASNApp.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CASNApp.Core.Queries
{
	public class AppointmentQuery
	{
		private readonly casn_appContext dbContext;

		public AppointmentQuery(casn_appContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public List<Appointment> GetAllAppointmentsWithOpenDrives(bool readOnly)
		{
			var openAppointmentIds = (readOnly ? dbContext.Drives.AsNoTracking() : dbContext.Drives)
				.Where(d => d.IsActive && d.StatusId == Models.Drive.StatusOpen
						&& d.StartLatitude != null && d.StartLongitude != null
						&& d.EndLatitude != null && d.EndLongitude != null)
				.Select(d => d.AppointmentId)
				.Distinct()
				.ToList();

			var result = (readOnly ? dbContext.Appointments.AsNoTracking() : dbContext.Appointments)
				.Include(a => a.Drives)
				.Include(a => a.AppointmentType)
				.Where(a => a.IsActive && DateTime.Compare(a.AppointmentDate, DateTime.UtcNow) >= 0
					&& openAppointmentIds.Contains(a.Id))
				.ToList();

			return result;
		}

		public List<Appointment> GetAllNextDayAppointmentsWithOpenDrives(bool readOnly)
		{
			var openAppointmentIds = (readOnly ? dbContext.Drives.AsNoTracking() : dbContext.Drives)
				.Where(d => d.IsActive && d.StatusId == Models.Drive.StatusOpen 
						&& d.StartLatitude != null && d.StartLongitude != null
						&& d.EndLatitude != null && d.EndLongitude != null)
				.Select(d => d.AppointmentId)
				.Distinct()
				.ToList();

			var tomorrowBeginsUTC = DateTime.Today.AddDays(1).ToUniversalTime();
			var tomorrowEndsUTC = DateTime.Today.AddDays(2).AddMilliseconds(-1).ToUniversalTime();

			var result = (readOnly ? dbContext.Appointments.AsNoTracking() : dbContext.Appointments)
				.Include(a => a.Drives)
				.Include(a => a.AppointmentType)
				.Where(a => a.IsActive && a.AppointmentDate >= tomorrowBeginsUTC && a.AppointmentDate <= tomorrowEndsUTC
					&& openAppointmentIds.Contains(a.Id))
				.ToList();
			
			return result;
		}

		public Appointment GetAppointmentByIdWithCaller(int appointmentId, bool readOnly)
		{
			var result = (readOnly ? dbContext.Appointments.AsNoTracking() : dbContext.Appointments)
				.Include(a => a.Caller)
				.Include(a => a.AppointmentType)
				.Where(a => a.Id == appointmentId)
				.SingleOrDefault();
			return result;
		}
	}
}
