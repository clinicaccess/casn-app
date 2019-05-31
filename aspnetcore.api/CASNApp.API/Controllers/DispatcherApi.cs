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
using CASNApp.Core.Commands;
using CASNApp.Core.Models;
using CASNApp.Core.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CASNApp.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize(Policy = Constants.IsDispatcherPolicy)]
    public class DispatcherApiController : Controller
    {
        private readonly Core.Entities.casn_appContext dbContext;
        private readonly ILoggerFactory loggerFactory;
        private readonly string googleApiKey;
        private readonly double vagueLocationMinOffset;
        private readonly double vagueLocationMaxOffset;
		private readonly string twilioAccountSID;
		private readonly string twilioAuthKey;
		private readonly string twilioPhoneNumber;

        public DispatcherApiController(Core.Entities.casn_appContext dbContext, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.dbContext = dbContext;
            googleApiKey = configuration[Core.Constants.GoogleApiKey];
            vagueLocationMinOffset = double.Parse(configuration[Core.Constants.VagueLocationMinOffset]);
            vagueLocationMaxOffset = double.Parse(configuration[Core.Constants.VagueLocationMaxOffset]);
            this.loggerFactory = loggerFactory;
			twilioAccountSID = configuration[Core.Constants.TwilioAccountSID];
			twilioAuthKey = configuration[Core.Constants.TwilioAuthKey];
			twilioPhoneNumber = configuration[Core.Constants.TwilioPhoneNumber];
		}

		/// <summary>
		/// adds a new appointment
		/// </summary>
		/// <remarks>Adds appointment (and drives) to the system</remarks>
		/// <param name="appointmentDTO">appointmentData to add</param>
		/// <response code="200">Success. Created appointment.</response>
		/// <response code="400">Client Error - please check your request format &amp; try again.</response>
		/// <response code="409">Error. That appointment already exists.</response>
		[HttpPost]
        [Route("api/dispatcher/appointments")]
        [ValidateModelState]
        [SwaggerOperation("AddAppointment")]
        [SwaggerResponse(statusCode: 200, type: typeof(AppointmentDTO), description: "Success. Created appointment.")]
        public virtual async Task<IActionResult> AddAppointment([FromBody]AppointmentDTO appointmentDTO)
        {
            var userEmail = HttpContext.GetUserEmail();
            var volunteerQuery = new VolunteerQuery(dbContext);
            var volunteer = volunteerQuery.GetActiveDispatcherByEmail(userEmail, true);

            if (volunteer == null)
            {
                return Forbid();
            }

            if (!appointmentDTO.Validate())
            {
                return BadRequest(appointmentDTO);
            }

            var appointment = appointmentDTO.Appointment;
            var driveTo = appointmentDTO.DriveTo;
            var driveFrom = appointmentDTO.DriveFrom;

            var clinic = await dbContext.Clinic
                .AsNoTracking()
                .Where(c => c.Id == appointment.ClinicId)
                .FirstOrDefaultAsync();

            if (clinic == null)
            {
                return BadRequest(appointmentDTO);
            }

            var caller = await dbContext.Caller
                .AsNoTracking()
                .Where(p => p.Id == appointment.CallerId)
                .FirstOrDefaultAsync();

            if (caller == null)
            {
                return BadRequest(appointmentDTO);
            }

            var appointmentEntity = new Core.Entities.Appointment
            {
                DispatcherId = volunteer.Id,
                CallerId = caller.Id,
                ClinicId = clinic.Id,
                PickupLocationVague = appointment.PickupLocationVague,
                DropoffLocationVague = appointment.DropoffLocationVague,
                AppointmentDate = appointment.AppointmentDate.Value,
                AppointmentTypeId = appointment.AppointmentTypeId.Value,
                IsActive = true,
                Created = DateTime.UtcNow,
                Updated = null,
            };

            Core.Entities.Drive driveToEntity = null;

            if (driveTo != null)
            {
                driveToEntity = new Core.Entities.Drive
                {
                    Appointment = appointmentEntity,
                    Direction = Drive.DirectionToClinic,
                    DriverId = null,
                    StartAddress = driveTo.StartAddress,
                    StartCity = driveTo.StartCity,
                    StartState = driveTo.StartState,
                    StartPostalCode = driveTo.StartPostalCode,
                    EndAddress = clinic.Address,
                    EndCity = clinic.City,
                    EndState = clinic.State,
                    EndPostalCode = clinic.PostalCode,
                    EndLatitude = clinic.Latitude,
                    EndLongitude = clinic.Longitude,
                    EndGeocoded = clinic.Geocoded,
                    IsActive = true,
                    Created = DateTime.UtcNow,
                    Updated = null,
                };
            }

            Core.Entities.Drive driveFromEntity = null;

            if (driveFrom != null)
            {
                driveFromEntity = new Core.Entities.Drive
                {
                    Appointment = appointmentEntity,
                    Direction = Drive.DirectionFromClinic,
                    DriverId = null,
                    StartAddress = clinic.Address,
                    StartCity = clinic.City,
                    StartState = clinic.State,
                    StartPostalCode = clinic.PostalCode,
                    StartLatitude = clinic.Latitude,
                    StartLongitude = clinic.Longitude,
                    StartGeocoded = clinic.Geocoded,
                    EndAddress = driveFrom.EndAddress,
                    EndCity = driveFrom.EndCity,
                    EndState = driveFrom.EndState,
                    EndPostalCode = driveFrom.EndPostalCode,
                    IsActive = true,
                    Created = DateTime.UtcNow,
                    Updated = null,
                };
            }

            var geocoder = new GeocoderQuery(googleApiKey, loggerFactory.CreateLogger<GeocoderQuery>());

            var driveToAddress = driveToEntity?.GetCallerAddress();
            GeocoderQuery.LatLng driveToLocation = null;

            if (driveToAddress != null)
            {
                driveToLocation = await geocoder.ForwardLookupAsync(driveToAddress);
            }

            if (driveTo != null && driveToLocation == null)
            {
                return StatusCode((int)System.Net.HttpStatusCode.UnprocessableEntity, "Geocoding failed for Pickup Address.");
            }

            driveToEntity?.SetCallerLocation(driveToLocation);
            driveTo?.SetCallerLocation(driveToLocation);

            var driveFromAddress = driveFromEntity?.GetCallerAddress();
            GeocoderQuery.LatLng driveFromLocation = null;

            if (driveFromAddress != null)
            {
                if (driveToAddress != null &&
                    string.Equals(driveToAddress, driveFromAddress, StringComparison.CurrentCultureIgnoreCase))
                {
                    driveFromLocation = driveToLocation;
                }
                else
                {
                    driveFromLocation = await geocoder.ForwardLookupAsync(driveFromAddress);
                }
            }

            if (driveFrom != null && driveFromLocation == null)
            {
                return StatusCode((int)System.Net.HttpStatusCode.UnprocessableEntity, "Geocoding failed for Dropoff Address.");
            }

            driveFromEntity?.SetCallerLocation(driveFromLocation);
            driveFrom?.SetCallerLocation(driveFromLocation);

            var random = new Random();

            var pickupVagueLocation = driveToLocation?.ToVagueLocation(random, vagueLocationMinOffset, vagueLocationMaxOffset);

            if (pickupVagueLocation != null)
            {
                appointmentEntity.PickupVagueLatitude = pickupVagueLocation.Latitude;
                appointmentEntity.PickupVagueLongitude = pickupVagueLocation.Longitude;
            }

            GeocoderQuery.LatLng dropoffVagueLocation;

            if (driveToAddress != null && driveFromAddress != null &&
                string.Equals(driveToAddress, driveFromAddress, StringComparison.CurrentCultureIgnoreCase))
            {
                dropoffVagueLocation = pickupVagueLocation;
            }
            else
            {
                dropoffVagueLocation = driveFromLocation?.ToVagueLocation(random, vagueLocationMinOffset, vagueLocationMaxOffset);
            }

            if (dropoffVagueLocation != null)
            {
                appointmentEntity.DropoffVagueLatitude = dropoffVagueLocation.Latitude;
                appointmentEntity.DropoffVagueLongitude = dropoffVagueLocation.Longitude;
            }

            dbContext.Appointment.Add(appointmentEntity);

            if (driveToEntity != null)
            {
                dbContext.Drive.Add(driveToEntity);
            }

            if (driveFromEntity != null)
            {
                dbContext.Drive.Add(driveFromEntity);
            }

            await dbContext.SaveChangesAsync();

            appointment.Id = appointmentEntity.Id;
            appointment.Created = appointmentEntity.Created;

            appointment.PickupVagueLatitude = appointmentEntity.PickupVagueLatitude;
            appointment.PickupVagueLongitude = appointmentEntity.PickupVagueLongitude;
            appointment.DropoffVagueLatitude = appointmentEntity.DropoffVagueLatitude;
            appointment.DropoffVagueLongitude = appointmentEntity.DropoffVagueLongitude;

            if (driveToEntity != null)
            {
                driveTo.Id = driveToEntity.Id;
                driveTo.EndAddress = driveToEntity.EndAddress;
                driveTo.EndCity = driveToEntity.EndCity;
                driveTo.EndState = driveToEntity.EndState;
                driveTo.EndPostalCode = driveToEntity.EndPostalCode;
                driveTo.Created = driveToEntity.Created;
            }

            if (driveFromEntity != null)
            {
                driveFrom.Id = driveFromEntity.Id;
                driveFrom.EndAddress = driveFromEntity.EndAddress;
                driveFrom.EndCity = driveFromEntity.EndCity;
                driveFrom.EndState = driveFromEntity.EndState;
                driveFrom.EndPostalCode = driveFromEntity.EndPostalCode;
                driveFrom.Created = driveFromEntity.Created;
            }

			//send initial text message to drivers
			TwilioCommand newSMS = new TwilioCommand(twilioAccountSID, twilioAuthKey, twilioPhoneNumber, loggerFactory.CreateLogger<TwilioCommand>(), dbContext);
			newSMS.SendAppointmentMessage(appointmentEntity, driveToEntity, driveFromEntity, 5);
			
            return new ObjectResult(appointmentDTO);
        }

        /// <summary>
        /// approves a volunteer for a drive
        /// </summary>
        /// <remarks>Adds driverId to a drive</remarks>
        /// <param name="body1"></param>
        /// <response code="200">Success. Added driver to drive.</response>
        /// <response code="400">Client Error - please check your request format &amp; try again.</response>
        /// <response code="404">Error. The driveId or volunteerId was not found.</response>
        [HttpPost]
        [Route("api/drives/approve")]
        [ValidateModelState]
        [SwaggerOperation("AddDriver")]
        public virtual IActionResult AddDriver([FromBody]Body1 body1)
        {
            var userEmail = HttpContext.GetUserEmail();
            var volunteerQuery = new VolunteerQuery(dbContext);
            var volunteer = volunteerQuery.GetActiveDispatcherByEmail(userEmail, true);

            if (volunteer == null)
            {
                return Forbid();
            }

            var volunteerDriveId = body1.VolunteerDriveId;

            if (!volunteerDriveId.HasValue)
            {
                return BadRequest(body1);
            }

            var volunteerDrive = dbContext.VolunteerDrive
                .Include(vd => vd.Drive)
                .Where(vd => vd.Id == volunteerDriveId.Value && vd.IsActive)
                .SingleOrDefault();

            if (volunteerDrive == null)
            {
                return NotFound(body1);
            }

            if (volunteerDrive.Drive.StatusId != Drive.StatusPending ||
                volunteerDrive.Drive.DriverId.HasValue)
            {
                return Conflict(body1);
            }

            var drive = volunteerDrive.Drive;

            drive.DriverId = volunteerDrive.VolunteerId;
            drive.StatusId = Drive.StatusApproved;
            drive.Approved = DateTime.UtcNow;
            drive.ApprovedById = volunteer.Id;
            drive.Updated = DateTime.UtcNow;

            dbContext.SaveChanges();

            return Ok();

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200);

            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);

            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);
        }

        /// <summary>
        /// cancels a drive
        /// </summary>
        /// <remarks>Updates statusId to canceled and applies the given cancelReasonId</remarks>
        /// <param name="driveId">Id of the drive to cancel</param>
        /// <param name="cancelReasonId">Id of the DriveCancelReason to apply</param>
        /// <response code="200">Success. Drive canceled.</response>
        /// <response code="400">Client Error - please check your request format and try again.</response>
        /// <response code="404">Error. The driveId was not found.</response>
        /// <response code="409">Error. The drive cannot be canceled in its current state.</response>
        [HttpPost]
        [Route("api/drives/{driveId}/cancel")]
        [ValidateModelState]
        [SwaggerOperation("CancelDrive")]
        public virtual async Task<IActionResult> CancelDrive([FromRoute]uint driveId, [FromQuery]uint cancelReasonId)
        {
            var userEmail = HttpContext.GetUserEmail();
            var volunteerQuery = new VolunteerQuery(dbContext);
            var volunteer = volunteerQuery.GetActiveDispatcherByEmail(userEmail, true);

            if (volunteer == null)
            {
                return Forbid();
            }

            var driveCommand = new DriveCommand(dbContext);

            var result = await driveCommand.CancelDriveAsync(driveId, cancelReasonId);

            await dbContext.SaveChangesAsync();

            switch (result.ErrorCode)
            {
                case Core.Misc.ErrorCode.None:
                    return Ok(result.Data);
                case Core.Misc.ErrorCode.NotFound:
                    return NotFound();
                case Core.Misc.ErrorCode.InvalidOperation:
                    return Conflict();
                default:
                    return BadRequest();
            }
        }

        /// <summary>
        /// adds a caller
        /// </summary>
        /// <remarks>Adds caller to the system</remarks>
        /// <param name="caller">callerData to add</param>
        /// <response code="201">item created</response>
        /// <response code="400">invalid input, object invalid</response>
        /// <response code="409">the item already exists</response>
        [HttpPost]
        [Route("api/caller")]
        [ValidateModelState]
        [SwaggerOperation("AddCaller")]
        public virtual async Task<IActionResult> AddCaller([FromBody]Caller caller)
        {
            var userEmail = HttpContext.GetUserEmail();
            var volunteerQuery = new VolunteerQuery(dbContext);
            var volunteer = volunteerQuery.GetActiveDispatcherByEmail(userEmail, true);

            if (volunteer == null)
            {
                return Forbid();
            }

            if (!ModelState.IsValid ||
                caller == null ||
                string.IsNullOrWhiteSpace(caller.CallerIdentifier) ||
                (string.IsNullOrWhiteSpace(caller.FirstName) && string.IsNullOrWhiteSpace(caller.LastName)) ||
                string.IsNullOrWhiteSpace(caller.Phone))
            {
                return BadRequest();
            }

            var exists = await dbContext.Caller
                .Where(p => p.CallerIdentifier == caller.CallerIdentifier)
                .AnyAsync();

            if (exists)
            {
                return Conflict();
            }

            var callerEntity = new Core.Entities.Caller(caller);

            dbContext.Caller.Add(callerEntity);
            await dbContext.SaveChangesAsync();

            caller.Id = callerEntity.Id;
            caller.Created = callerEntity.Created;
            caller.Updated = callerEntity.Updated;

            return CreatedAtAction(nameof(AddCaller), caller);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appointmentID">pass an appointmentIdentifier</param>
        /// <response code="200">Success. Appointment with ID {appointmentID} was deleted.</response>
        /// <response code="400">Client Error - please check your request format &amp; try again.</response>
        /// <response code="404">No appointment found.</response>
        [HttpDelete]
        [Route("api/dispatcher/appointments/{appointmentID}")]
        [ValidateModelState]
        [SwaggerOperation("DeleteAppointment")]
        [SwaggerResponse(statusCode: 200, type: typeof(DeleteSuccessMessage), description: "Success. Appointment with ID {appointmentID} was deleted.")]
        public virtual IActionResult DeleteAppointment([FromRoute][Required]string appointmentID)
        {
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(DeleteSuccessMessage));

            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);

            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);

            string exampleJson = null;
            exampleJson = "{\r\n  \"message\" : \"Success. Your {dataType} of ID {objectID} has been deleted.\"\r\n}";
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<DeleteSuccessMessage>(exampleJson)
            : default(DeleteSuccessMessage);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// gets caller by callerIdentifier
        /// </summary>
        /// <remarks>Search for existing callers by the dispatcher created callerIdentifier (caller ID) </remarks>
        /// <param name="callerIdentifier">pass a search string for looking up callerIdentifier</param>
        /// <response code="200">search results matching criteria</response>
        /// <response code="400">GAH IT IS SO BROKEN</response>
        /// <response code="404">caller not found frownies</response>
        [HttpGet]
        [Route("api/caller")]
        [ValidateModelState]
        [SwaggerOperation("GetCallerByCallerIdentifier")]
        [SwaggerResponse(statusCode: 200, type: typeof(Caller), description: "search results matching criteria")]
        public virtual async Task<IActionResult> GetCallerByCallerIdentifier([FromQuery][Required()] [MinLength(4)]string callerIdentifier)
        {
            var userEmail = HttpContext.GetUserEmail();
            var volunteerQuery = new VolunteerQuery(dbContext);
            var volunteer = volunteerQuery.GetActiveDispatcherByEmail(userEmail, true);

            if (volunteer == null)
            {
                return Forbid();
            }

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(Caller));

            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);

            var callers = await dbContext.Caller
                .AsNoTracking()
                .Where(p => p.CallerIdentifier == callerIdentifier)
                .Select(p => new Core.Models.Caller(p))
                .ToListAsync();

            if (callers.Count == 0)
            {
                return NotFound();
            }
            else if (callers.Count == 1)
            {
                return new ObjectResult(callers.First());
            }
            else
            {
                return new BadRequestResult();
            }

        }

        /// <summary>
        /// get list of applicants for a drive
        /// </summary>
        /// <param name="driveId">id of drive</param>
        /// <response code="200">successful operation</response>
        [HttpGet]
        [Route("api/volunteerDrive")]
        [ValidateModelState]
        [SwaggerOperation("GetVolunteerDrives")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<VolunteerDrive>), description: "successful operation")]
        public virtual IActionResult GetVolunteerDrives([FromQuery][Required()]long? driveId)
        {
            var userEmail = HttpContext.GetUserEmail();
            var volunteerQuery = new VolunteerQuery(dbContext);
            var volunteer = volunteerQuery.GetActiveDispatcherByEmail(userEmail, true);

            if (volunteer == null)
            {
                return Forbid();
            }

            if (!driveId.HasValue || !dbContext.Drive.Any(d => d.Id == driveId.Value))
            {
                return BadRequest();
            }

            var results = dbContext.VolunteerDrive
                .AsNoTracking()
                .Include(vd => vd.Volunteer)
                .Where(vd => vd.DriveId == driveId.Value && vd.IsActive)
                .OrderBy(vd => vd.Created)
                .Select(vd => new Core.Models.VolunteerDrive(vd))
                .ToList();

            return Ok(results);
        }

        /// <summary>
        /// updates an existing appointment
        /// </summary>
        /// <remarks>Updates appointment (and corresponding drive) information</remarks>
        /// <param name="appointmentID">pass an appointmentIdentifier</param>
        /// <param name="appointmentDTO">appointmentData with updated fields</param>
        /// <response code="200">Success. Created appointment.</response>
        /// <response code="400">Client Error - please check your request format &amp; try again.</response>
        /// <response code="404">Error. The appointment ID you requested does not exist.</response>
        [HttpPut]
        [Route("api/dispatcher/appointments/{appointmentID}")]
        [ValidateModelState]
        [SwaggerOperation("UpdateAppointment")]
        [SwaggerResponse(statusCode: 200, type: typeof(AppointmentDTO), description: "Success. Created appointment.")]
        public virtual IActionResult UpdateAppointment([FromRoute][Required]string appointmentID, [FromBody]AppointmentDTO appointmentDTO)
        {
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(AppointmentDTO));

            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);

            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);

            string exampleJson = null;
            exampleJson = "{\r\n  \"driveTo\" : {\r\n    \"startCity\" : \"Houston\",\r\n    \"startAddress\" : \"11415 Roark Rd\",\r\n    \"endState\" : \"TX\",\r\n    \"created\" : \"2000-01-23T04:56:07.000+00:00\",\r\n    \"endCity\" : \"Houston\",\r\n    \"driverId\" : 42,\r\n    \"appointmentId\" : 42,\r\n    \"startPostalCode\" : \"77031\",\r\n    \"id\" : 42,\r\n    \"startState\" : \"TX\",\r\n    \"endPostalCode\" : \"77024\",\r\n    \"updated\" : \"2000-01-23T04:56:07.000+00:00\",\r\n    \"endAddress\" : \"7373 Old Katy Rd\",\r\n    \"direction\" : 1\r\n  },\r\n  \"caller\" : {\r\n    \"civiContactId\" : 42,\r\n    \"firstName\" : \"Jane\",\r\n    \"lastName\" : \"Smith\",\r\n    \"isMinor\" : true,\r\n    \"callerIdentifier\" : \"JS1234\",\r\n    \"preferredLanguage\" : \"French\",\r\n    \"preferredContactMethod\" : 1,\r\n    \"phone\" : \"5555551234\",\r\n    \"created\" : \"2000-01-23T04:56:07.000+00:00\",\r\n    \"id\" : 42,\r\n    \"updated\" : \"2000-01-23T04:56:07.000+00:00\"\r\n  },\r\n  \"driveFrom\" : {\r\n    \"startCity\" : \"Houston\",\r\n    \"startAddress\" : \"11415 Roark Rd\",\r\n    \"endState\" : \"TX\",\r\n    \"created\" : \"2000-01-23T04:56:07.000+00:00\",\r\n    \"endCity\" : \"Houston\",\r\n    \"driverId\" : 42,\r\n    \"appointmentId\" : 42,\r\n    \"startPostalCode\" : \"77031\",\r\n    \"id\" : 42,\r\n    \"startState\" : \"TX\",\r\n    \"endPostalCode\" : \"77024\",\r\n    \"updated\" : \"2000-01-23T04:56:07.000+00:00\",\r\n    \"endAddress\" : \"7373 Old Katy Rd\",\r\n    \"direction\" : 1\r\n  },\r\n  \"appointment\" : {\r\n    \"pickupLocationVague\" : \"US59 S and BW8\",\r\n    \"clinicId\" : 42,\r\n    \"dropoffLocationVague\" : \"I10 W and 610\",\r\n    \"callerId\" : 42,\r\n    \"created\" : \"2000-01-23T04:56:07.000+00:00\",\r\n    \"id\" : 42,\r\n    \"dispatcherId\" : 42,\r\n    \"appointmentTypeId\" : 1,\r\n    \"appointmentDate\" : \"2000-01-23T04:56:07.000+00:00\",\r\n    \"updated\" : \"2000-01-23T04:56:07.000+00:00\"\r\n  }\r\n}";
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<AppointmentDTO>(exampleJson)
            : default(AppointmentDTO);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }
    }
}
