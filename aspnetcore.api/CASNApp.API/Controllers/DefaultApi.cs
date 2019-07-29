/*
 * CASN API
 *
 * This is a test CASN API
 *
 * OpenAPI spec version: 1.0.0
 * Contact: katie@clinicaccess.org
 * Generated by: https://openapi-generator.tech
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CASNApp.API.Attributes;
using CASNApp.API.Extensions;
using CASNApp.Core.Entities;
using CASNApp.Core.Models;
using CASNApp.Core.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CASNApp.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize(Policy = Constants.IsDispatcherOrDriverPolicy)]
    public class DefaultApiController : Controller
    {
        private readonly casn_appContext dbContext;

        public DefaultApiController(casn_appContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// gets list of clinics
        /// </summary>
        /// <response code="200">successful operation</response>
        [HttpGet]
        [Route("api/clinic")]
        [ValidateModelState]
        [SwaggerOperation("GetClinics")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Core.Models.Clinic>), description: "successful operation")]
        public virtual async Task<IActionResult> GetClinics()
        {
            var userEmail = HttpContext.GetUserEmail();
            var volunteerQuery = new VolunteerQuery(dbContext);
            var volunteer = volunteerQuery.GetActiveVolunteerByEmail(userEmail, true);

            if (volunteer == null)
            {
                return Forbid();
            }

            var clinics = await dbContext.Clinic
                .AsNoTracking()
                .Select(c => new Core.Models.Clinic(c))
                .ToListAsync();

            return new ObjectResult(clinics);
        }

        /// <summary>
        /// gets appointments with dispatcher-level details
        /// </summary>
        /// <remarks>Get all appointments within a default date range (possibly adjustable w/ query params). Appointments include details, e.g. exact location, available only to dispatchers. </remarks>
        /// <param name="startDate">pass a startDate by which to filter</param>
        /// <param name="endDate">pass an endDate by which to filter</param>
        /// <response code="200">all appointments in date range</response>
        /// <response code="400">Client Error - please check your request format &amp; try again.</response>
        /// <response code="404">Error - Not Found</response>
        [HttpGet]
        [Route("api/appointments")]
        [ValidateModelState]
        [SwaggerOperation("GetAppointments")]
        [SwaggerResponse(statusCode: 200, type: typeof(AllAppointments), description: "all appointments in date range")]
        public virtual async Task<IActionResult> GetAppointments([FromQuery] [MinLength(4)]string startDate, [FromQuery] [MinLength(4)]string endDate)
        {
            var userEmail = HttpContext.GetUserEmail();
            var volunteerQuery = new VolunteerQuery(dbContext);
            var volunteer = volunteerQuery.GetActiveVolunteerByEmail(userEmail, true);

            if (volunteer == null)
            {
                return Forbid();
            }

            var start = DateTime.Parse(startDate, styles: System.Globalization.DateTimeStyles.AssumeLocal);
            var end = DateTime.Parse(endDate, styles: System.Globalization.DateTimeStyles.AssumeLocal);

            var appointmentEntities = await dbContext.Appointment
                .AsNoTracking()
                .Include(a => a.Drives)
                .Include(a => a.Caller)
                .Where(a => a.AppointmentDate >= start &&
                            a.AppointmentDate <= end &&
                            a.IsActive)
                .ToListAsync();

            var driveIds = new List<long>();

            foreach (var appt in appointmentEntities)
            {
                var driveTo = appt.Drives.FirstOrDefault(d => d.IsActive && d.Direction == Core.Models.Drive.DirectionToClinic);
                var driveFrom = appt.Drives.FirstOrDefault(d => d.IsActive && d.Direction == Core.Models.Drive.DirectionFromClinic);

                if (driveTo?.Id != null)
                {
                    driveIds.Add(driveTo.Id);
                }

                if (driveFrom?.Id != null)
                {
                    driveIds.Add(driveFrom.Id);
                }
            }

            var appointmentDTOs = appointmentEntities.Select(a =>
            {
                var driveTo = a.Drives.FirstOrDefault(d => d.IsActive && d.Direction == Core.Models.Drive.DirectionToClinic);
                var driveFrom = a.Drives.FirstOrDefault(d => d.IsActive && d.Direction == Core.Models.Drive.DirectionFromClinic);

                var apptDto = new AppointmentDTO
                {
                    Appointment = new Core.Models.Appointment(a),
                    DriveTo = driveTo == null ? null : new Core.Models.Drive(driveTo),
                    DriveFrom = driveFrom == null ? null : new Core.Models.Drive(driveFrom)
                };

                return apptDto;
            }).ToList();

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(AllAppointments));

            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);

            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);

            var result = new AllAppointments(appointmentDTOs);

            return new ObjectResult(result);
        }

        /// <summary>
        /// gets appointment by appointmentID
        /// </summary>
        /// <remarks>Search for existing appointment by appointmentIdentifier, return dispatcher-level details </remarks>
        /// <param name="appointmentID">pass an appointmentIdentifier</param>
        /// <response code="200">Success. Found appointment.</response>
        /// <response code="400">Client Error - please check your request format &amp; try again.</response>
        /// <response code="404">No appointment found.</response>
        [HttpGet]
        [Route("api/appointments/{appointmentID}")]
        [ValidateModelState]
        [SwaggerOperation("GetAppointmentByID")]
        [SwaggerResponse(statusCode: 200, type: typeof(AppointmentDTO), description: "Success. Found appointment.")]
        public virtual async Task<IActionResult> GetAppointmentByID([FromRoute][Required]string appointmentID)
        {
            var userEmail = HttpContext.GetUserEmail();
            var volunteerQuery = new VolunteerQuery(dbContext);
            var volunteer = volunteerQuery.GetActiveVolunteerByEmail(userEmail, true);

            if (volunteer == null)
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(appointmentID) || !long.TryParse(appointmentID, out long apptId))
            {
                return BadRequest();
            }

            var apptEntity = await dbContext.Appointment
                .AsNoTracking()
                .Include(a => a.Drives)
                .Include(a => a.Caller)
                .Where(a => a.Id == apptId &&
                            a.IsActive)
                .FirstOrDefaultAsync();

            var driveIds = new List<long>();

                var driveToEntity = apptEntity.Drives.FirstOrDefault(d => d.IsActive && d.Direction == Core.Models.Drive.DirectionToClinic);
                var driveFromEntity = apptEntity.Drives.FirstOrDefault(d => d.IsActive && d.Direction == Core.Models.Drive.DirectionFromClinic);

                if (driveToEntity?.Id != null)
                {
                    driveIds.Add(driveToEntity.Id);
                }

                if (driveFromEntity?.Id != null)
                {
                    driveIds.Add(driveFromEntity.Id);
                }

            var driveTo = apptEntity.Drives.FirstOrDefault(d => d.IsActive && d.Direction == Core.Models.Drive.DirectionToClinic);
            var driveFrom = apptEntity.Drives.FirstOrDefault(d => d.IsActive && d.Direction == Core.Models.Drive.DirectionFromClinic);

            var apptDTO = new AppointmentDTO
            {
                Appointment = new Core.Models.Appointment(apptEntity),
                DriveTo = driveTo == null ? null : new Core.Models.Drive(driveTo),
                DriveFrom = driveFrom == null ? null : new Core.Models.Drive(driveFrom)
            };

            return Ok(apptDTO);

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(AppointmentDTO));

            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);

            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);

        }

        /// <summary>
        /// gets list of appointment types
        /// </summary>
        /// <response code="200">successful operation</response>
        [HttpGet]
        [Route("api/appointmentType")]
        [ValidateModelState]
        [SwaggerOperation("GetAppointmentTypes")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Core.Models.AppointmentType>), description: "successful operation")]
        public virtual async Task<IActionResult> GetAppointmentTypes()
        {
            var userEmail = HttpContext.GetUserEmail();
            var volunteerQuery = new VolunteerQuery(dbContext);
            var volunteer = volunteerQuery.GetActiveVolunteerByEmail(userEmail, true);

            if (volunteer == null)
            {
                return Forbid();
            }

            var query = new AppointmentTypeQuery(dbContext);
            var entities = await query.GetActiveAppointmentTypesAsync(true);

            var results = entities.Select(at => new Core.Models.AppointmentType(at)).ToList();

            return new ObjectResult(results);
        }

        /// <summary>
        /// gets list of drive statuses
        /// </summary>
        /// <response code="200">successful operation</response>
        [HttpGet]
        [Route("api/driveStatus")]
        [ValidateModelState]
        [SwaggerOperation("GetDriveStatuses")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Core.Models.DriveStatus>), description: "successful operation")]
        public virtual async Task<IActionResult> GetDriveStatuses()
        {
            var userEmail = HttpContext.GetUserEmail();
            var volunteerQuery = new VolunteerQuery(dbContext);
            var volunteer = volunteerQuery.GetActiveVolunteerByEmail(userEmail, true);

            if (volunteer == null)
            {
                return Forbid();
            }

            var query = new DriveStatusQuery(dbContext);
            var results = await query.GetActiveDriveStatusesAsync(true);

            return new ObjectResult(results);
        }

        /// <summary>
        /// gets list of drive cancel reasons
        /// </summary>
        /// <response code="200">successful operation</response>
        [HttpGet]
        [Route("api/driveCancelReason")]
        [ValidateModelState]
        [SwaggerOperation("GetDriveCancelReasons")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Core.Models.DriveCancelReason>), description: "successful operation")]
        public virtual async Task<IActionResult> GetDriveCancelReasons()
        {
            var userEmail = HttpContext.GetUserEmail();
            var volunteerQuery = new VolunteerQuery(dbContext);
            var volunteer = volunteerQuery.GetActiveVolunteerByEmail(userEmail, true);

            if (volunteer == null)
            {
                return Forbid();
            }

            var query = new DriveCancelReasonQuery(dbContext);
            var results = await query.GetActiveCancelReasonsAsync(true);

            return new ObjectResult(results);
        }

        /// <summary>
        /// gets list of badges combined with badges earned for the current user
        /// </summary>
        /// <response code="200">successful operation</response>
        [HttpGet]
        [Route("api/badge")]
        [ValidateModelState]
        [SwaggerOperation("GetBadgesForVolunteerId")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<BadgeDTO>), description: "successful operation")]
        public virtual async Task<IActionResult> GetBadgesForVolunteerId()
        {
            var userEmail = HttpContext.GetUserEmail();
            var volunteerQuery = new VolunteerQuery(dbContext);
            var volunteer = volunteerQuery.GetActiveVolunteerByEmail(userEmail, true);

            if (volunteer == null)
            {
                return Forbid();
            }

            var badgeQuery = new BadgeQuery(dbContext);

            var results = await badgeQuery.GetBadgesForVolunteerIdAsync(volunteer.Id, true);

            return new ObjectResult(results);
        }

    }
}
