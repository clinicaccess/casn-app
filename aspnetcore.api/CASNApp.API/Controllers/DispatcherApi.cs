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
using CASNApp.API.Models;
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
    [Authorize(Roles = Constants.Roles.Dispatchers)]
    public class DispatcherApiController : Controller
    {
        private readonly Entities.casn_appContext dbContext;

        public DispatcherApiController(Entities.casn_appContext dbContext)
        {
            this.dbContext = dbContext;
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
            if (!appointmentDTO.Validate())
            {
                return BadRequest(appointmentDTO);
            }

            var appointment = appointmentDTO.Appointment;
            var driveTo = appointmentDTO.DriveTo;
            var driveFrom = appointmentDTO.DriveFrom;

            var dispatcher = await dbContext.Volunteer
                .AsNoTracking()
                .Where(v => v.Id == appointment.DispatcherId) // TODO: Later we'll use the userId from the JWT
                .FirstOrDefaultAsync();

            if (dispatcher == null)
            {
                return BadRequest(appointmentDTO);
            }

            var clinic = await dbContext.Clinic
                .AsNoTracking()
                .Where(c => c.Id == appointment.ClinicId)
                .FirstOrDefaultAsync();

            if (clinic == null)
            {
                return BadRequest(appointmentDTO);
            }

            var patient = await dbContext.Patient
                .AsNoTracking()
                .Where(p => p.Id == appointment.PatientId)
                .FirstOrDefaultAsync();

            if (patient == null)
            {
                return BadRequest(appointmentDTO);
            }

            var appointmentEntity = new Entities.Appointment
            {
                DispatcherId = dispatcher.Id,
                PatientId = patient.Id,
                ClinicId = clinic.Id,
                PickupLocationVague = appointment.PickupLocationVague,
                DropoffLocationVague = appointment.DropoffLocationVague,
                AppointmentDate = appointment.AppointmentDate.Value,
                AppointmentTypeId = appointment.AppointmentTypeId.Value,
                IsActive = true,
                Created = DateTime.UtcNow,
                Updated = null,
            };

            var driveToEntity = new Entities.Drive
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
                IsActive = true,
                Created = DateTime.UtcNow,
                Updated = null,
            };

            var driveFromEntity = new Entities.Drive
            {
                Appointment = appointmentEntity,
                Direction = Drive.DirectionFromClinic,
                DriverId = null,
                StartAddress = clinic.Address,
                StartCity = clinic.City,
                StartState = clinic.State,
                StartPostalCode = clinic.PostalCode,
                EndAddress = driveTo.StartAddress,
                EndCity = driveTo.StartCity,
                EndState = driveTo.StartState,
                EndPostalCode = driveTo.StartPostalCode,
                IsActive = true,
                Created = DateTime.UtcNow,
                Updated = null,
            };

            dbContext.Appointment.Add(appointmentEntity);
            dbContext.Drive.Add(driveToEntity);
            dbContext.Drive.Add(driveFromEntity);

            await dbContext.SaveChangesAsync();

            appointment.Id = appointmentEntity.Id;
            appointment.Created = appointmentEntity.Created;

            driveTo.Id = driveToEntity.Id;
            driveTo.EndAddress = driveToEntity.EndAddress;
            driveTo.EndCity = driveToEntity.EndCity;
            driveTo.EndState = driveToEntity.EndState;
            driveTo.EndPostalCode = driveToEntity.EndPostalCode;
            driveTo.Created = driveToEntity.Created;

            driveFrom.Id = driveFromEntity.Id;
            driveFrom.EndAddress = driveFromEntity.EndAddress;
            driveFrom.EndCity = driveFromEntity.EndCity;
            driveFrom.EndState = driveFromEntity.EndState;
            driveFrom.EndPostalCode = driveFromEntity.EndPostalCode;
            driveFrom.Created = driveFromEntity.Created;

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

            if (volunteerDrive.Drive.Status != Drive.StatusPending ||
                volunteerDrive.Drive.DriverId.HasValue)
            {
                return Conflict(body1);
            }

            // TODO: Remove hard-coded "approver" ID
            uint approverId = 1;

            var drive = volunteerDrive.Drive;

            drive.DriverId = volunteerDrive.VolunteerId;
            drive.Status = Drive.StatusApproved;
            drive.Approved = DateTime.UtcNow;
            drive.ApprovedBy = approverId;
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
        /// adds a patient
        /// </summary>
        /// <remarks>Adds patient to the system</remarks>
        /// <param name="patient">patientData to add</param>
        /// <response code="201">item created</response>
        /// <response code="400">invalid input, object invalid</response>
        /// <response code="409">the item already exists</response>
        [HttpPost]
        [Route("api/patient")]
        [ValidateModelState]
        [SwaggerOperation("AddPatient")]
        public virtual async Task<IActionResult> AddPatient([FromBody]Patient patient)
        {
            if (!ModelState.IsValid ||
                patient == null ||
                string.IsNullOrWhiteSpace(patient.PatientIdentifier) ||
                (string.IsNullOrWhiteSpace(patient.FirstName) && string.IsNullOrWhiteSpace(patient.LastName)) ||
                string.IsNullOrWhiteSpace(patient.Phone))
            {
                return BadRequest();
            }

            var exists = await dbContext.Patient
                .Where(p => p.PatientIdentifier == patient.PatientIdentifier)
                .AnyAsync();

            if (exists)
            {
                return Conflict();
            }

            var patientEntity = new Entities.Patient(patient);

            dbContext.Patient.Add(patientEntity);
            await dbContext.SaveChangesAsync();

            patient.Id = patientEntity.Id;
            patient.Created = patientEntity.Created;
            patient.Updated = patientEntity.Updated;

            return CreatedAtAction(nameof(AddPatient), patient);
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
        /// gets patient by patientIdentifier
        /// </summary>
        /// <remarks>Search for existing patients by the dispatcher created patientIdentifier (patient ID) </remarks>
        /// <param name="patientIdentifier">pass a search string for looking up patientIdentifier</param>
        /// <response code="200">search results matching criteria</response>
        /// <response code="400">GAH IT IS SO BROKEN</response>
        /// <response code="404">patient not found frownies</response>
        [HttpGet]
        [Route("api/patient")]
        [ValidateModelState]
        [SwaggerOperation("GetPatientByPatientIdentifier")]
        [SwaggerResponse(statusCode: 200, type: typeof(Patient), description: "search results matching criteria")]
        public virtual async Task<IActionResult> GetPatientByPatientIdentifier([FromQuery][Required()] [MinLength(4)]string patientIdentifier)
        {
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(Patient));

            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);

            var patients = await dbContext.Patient
                .AsNoTracking()
                .Where(p => p.PatientIdentifier == patientIdentifier)
                .Select(p => new Models.Patient(p))
                .ToListAsync();

            if (patients.Count == 0)
            {
                return NotFound();
            }
            else if (patients.Count == 1)
            {
                return new ObjectResult(patients.First());
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
            if (!driveId.HasValue || !dbContext.Drive.Any(d => d.Id == driveId.Value))
            {
                return BadRequest();
            }

            var results = dbContext.VolunteerDrive
                .AsNoTracking()
                .Include(vd => vd.Volunteer)
                .Where(vd => vd.DriveId == driveId.Value && vd.IsActive)
                .OrderBy(vd => vd.Created)
                .Select(vd => new Models.VolunteerDrive(vd))
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
            exampleJson = "{\r\n  \"driveTo\" : {\r\n    \"startCity\" : \"Houston\",\r\n    \"startAddress\" : \"11415 Roark Rd\",\r\n    \"endState\" : \"TX\",\r\n    \"created\" : \"2000-01-23T04:56:07.000+00:00\",\r\n    \"endCity\" : \"Houston\",\r\n    \"driverId\" : 42,\r\n    \"appointmentId\" : 42,\r\n    \"startPostalCode\" : \"77031\",\r\n    \"id\" : 42,\r\n    \"startState\" : \"TX\",\r\n    \"endPostalCode\" : \"77024\",\r\n    \"updated\" : \"2000-01-23T04:56:07.000+00:00\",\r\n    \"endAddress\" : \"7373 Old Katy Rd\",\r\n    \"direction\" : 1\r\n  },\r\n  \"patient\" : {\r\n    \"civiContactId\" : 42,\r\n    \"firstName\" : \"Jane\",\r\n    \"lastName\" : \"Smith\",\r\n    \"isMinor\" : true,\r\n    \"patientIdentifier\" : \"JS1234\",\r\n    \"preferredLanguage\" : \"French\",\r\n    \"preferredContactMethod\" : 1,\r\n    \"phone\" : \"5555551234\",\r\n    \"created\" : \"2000-01-23T04:56:07.000+00:00\",\r\n    \"id\" : 42,\r\n    \"updated\" : \"2000-01-23T04:56:07.000+00:00\"\r\n  },\r\n  \"driveFrom\" : {\r\n    \"startCity\" : \"Houston\",\r\n    \"startAddress\" : \"11415 Roark Rd\",\r\n    \"endState\" : \"TX\",\r\n    \"created\" : \"2000-01-23T04:56:07.000+00:00\",\r\n    \"endCity\" : \"Houston\",\r\n    \"driverId\" : 42,\r\n    \"appointmentId\" : 42,\r\n    \"startPostalCode\" : \"77031\",\r\n    \"id\" : 42,\r\n    \"startState\" : \"TX\",\r\n    \"endPostalCode\" : \"77024\",\r\n    \"updated\" : \"2000-01-23T04:56:07.000+00:00\",\r\n    \"endAddress\" : \"7373 Old Katy Rd\",\r\n    \"direction\" : 1\r\n  },\r\n  \"appointment\" : {\r\n    \"pickupLocationVague\" : \"US59 S and BW8\",\r\n    \"clinicId\" : 42,\r\n    \"dropoffLocationVague\" : \"I10 W and 610\",\r\n    \"patientId\" : 42,\r\n    \"created\" : \"2000-01-23T04:56:07.000+00:00\",\r\n    \"id\" : 42,\r\n    \"dispatcherId\" : 42,\r\n    \"appointmentTypeId\" : 1,\r\n    \"appointmentDate\" : \"2000-01-23T04:56:07.000+00:00\",\r\n    \"updated\" : \"2000-01-23T04:56:07.000+00:00\"\r\n  }\r\n}";
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<AppointmentDTO>(exampleJson)
            : default(AppointmentDTO);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }
    }
}
