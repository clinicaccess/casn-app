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
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace CASNApp.API.Models
{ 
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class Drive : IEquatable<Drive>
    { 
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name="id")]
        public long? Id { get; private set; }

        /// <summary>
        /// Gets or Sets AppointmentId
        /// </summary>
        [Required]
        [DataMember(Name="appointmentId")]
        public long? AppointmentId { get; set; }

        /// <summary>
        /// 1 &#x3D; toClinic, 2 &#x3D; fromClinic
        /// </summary>
        /// <value>1 &#x3D; toClinic, 2 &#x3D; fromClinic</value>
        [Required]
        [DataMember(Name="direction")]
        public int? Direction { get; set; }

        /// <summary>
        /// Gets or Sets DriverId
        /// </summary>
        [DataMember(Name="driverId")]
        public long? DriverId { get; set; }

        /// <summary>
        /// Gets or Sets StartAddress
        /// </summary>
        [Required]
        [DataMember(Name="startAddress")]
        public string StartAddress { get; set; }

        /// <summary>
        /// Gets or Sets StartCity
        /// </summary>
        [Required]
        [DataMember(Name="startCity")]
        public string StartCity { get; set; }

        /// <summary>
        /// Gets or Sets StartState
        /// </summary>
        [Required]
        [DataMember(Name="startState")]
        public string StartState { get; set; }

        /// <summary>
        /// Gets or Sets StartPostalCode
        /// </summary>
        [Required]
        [DataMember(Name="startPostalCode")]
        public string StartPostalCode { get; set; }

        /// <summary>
        /// Gets or Sets EndAddress
        /// </summary>
        [Required]
        [DataMember(Name="endAddress")]
        public string EndAddress { get; set; }

        /// <summary>
        /// Gets or Sets EndCity
        /// </summary>
        [Required]
        [DataMember(Name="endCity")]
        public string EndCity { get; set; }

        /// <summary>
        /// Gets or Sets EndState
        /// </summary>
        [Required]
        [DataMember(Name="endState")]
        public string EndState { get; set; }

        /// <summary>
        /// Gets or Sets EndPostalCode
        /// </summary>
        [Required]
        [DataMember(Name="endPostalCode")]
        public string EndPostalCode { get; set; }

        /// <summary>
        /// Gets or Sets Created
        /// </summary>
        [DataMember(Name="created")]
        public DateTime? Created { get; private set; }

        /// <summary>
        /// Gets or Sets Updated
        /// </summary>
        [DataMember(Name="updated")]
        public DateTime? Updated { get; private set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Drive {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  AppointmentId: ").Append(AppointmentId).Append("\n");
            sb.Append("  Direction: ").Append(Direction).Append("\n");
            sb.Append("  DriverId: ").Append(DriverId).Append("\n");
            sb.Append("  StartAddress: ").Append(StartAddress).Append("\n");
            sb.Append("  StartCity: ").Append(StartCity).Append("\n");
            sb.Append("  StartState: ").Append(StartState).Append("\n");
            sb.Append("  StartPostalCode: ").Append(StartPostalCode).Append("\n");
            sb.Append("  EndAddress: ").Append(EndAddress).Append("\n");
            sb.Append("  EndCity: ").Append(EndCity).Append("\n");
            sb.Append("  EndState: ").Append(EndState).Append("\n");
            sb.Append("  EndPostalCode: ").Append(EndPostalCode).Append("\n");
            sb.Append("  Created: ").Append(Created).Append("\n");
            sb.Append("  Updated: ").Append(Updated).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Drive)obj);
        }

        /// <summary>
        /// Returns true if Drive instances are equal
        /// </summary>
        /// <param name="other">Instance of Drive to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Drive other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    Id == other.Id ||
                    Id != null &&
                    Id.Equals(other.Id)
                ) && 
                (
                    AppointmentId == other.AppointmentId ||
                    AppointmentId != null &&
                    AppointmentId.Equals(other.AppointmentId)
                ) && 
                (
                    Direction == other.Direction ||
                    Direction != null &&
                    Direction.Equals(other.Direction)
                ) && 
                (
                    DriverId == other.DriverId ||
                    DriverId != null &&
                    DriverId.Equals(other.DriverId)
                ) && 
                (
                    StartAddress == other.StartAddress ||
                    StartAddress != null &&
                    StartAddress.Equals(other.StartAddress)
                ) && 
                (
                    StartCity == other.StartCity ||
                    StartCity != null &&
                    StartCity.Equals(other.StartCity)
                ) && 
                (
                    StartState == other.StartState ||
                    StartState != null &&
                    StartState.Equals(other.StartState)
                ) && 
                (
                    StartPostalCode == other.StartPostalCode ||
                    StartPostalCode != null &&
                    StartPostalCode.Equals(other.StartPostalCode)
                ) && 
                (
                    EndAddress == other.EndAddress ||
                    EndAddress != null &&
                    EndAddress.Equals(other.EndAddress)
                ) && 
                (
                    EndCity == other.EndCity ||
                    EndCity != null &&
                    EndCity.Equals(other.EndCity)
                ) && 
                (
                    EndState == other.EndState ||
                    EndState != null &&
                    EndState.Equals(other.EndState)
                ) && 
                (
                    EndPostalCode == other.EndPostalCode ||
                    EndPostalCode != null &&
                    EndPostalCode.Equals(other.EndPostalCode)
                ) && 
                (
                    Created == other.Created ||
                    Created != null &&
                    Created.Equals(other.Created)
                ) && 
                (
                    Updated == other.Updated ||
                    Updated != null &&
                    Updated.Equals(other.Updated)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                    if (Id != null)
                    hashCode = hashCode * 59 + Id.GetHashCode();
                    if (AppointmentId != null)
                    hashCode = hashCode * 59 + AppointmentId.GetHashCode();
                    if (Direction != null)
                    hashCode = hashCode * 59 + Direction.GetHashCode();
                    if (DriverId != null)
                    hashCode = hashCode * 59 + DriverId.GetHashCode();
                    if (StartAddress != null)
                    hashCode = hashCode * 59 + StartAddress.GetHashCode();
                    if (StartCity != null)
                    hashCode = hashCode * 59 + StartCity.GetHashCode();
                    if (StartState != null)
                    hashCode = hashCode * 59 + StartState.GetHashCode();
                    if (StartPostalCode != null)
                    hashCode = hashCode * 59 + StartPostalCode.GetHashCode();
                    if (EndAddress != null)
                    hashCode = hashCode * 59 + EndAddress.GetHashCode();
                    if (EndCity != null)
                    hashCode = hashCode * 59 + EndCity.GetHashCode();
                    if (EndState != null)
                    hashCode = hashCode * 59 + EndState.GetHashCode();
                    if (EndPostalCode != null)
                    hashCode = hashCode * 59 + EndPostalCode.GetHashCode();
                    if (Created != null)
                    hashCode = hashCode * 59 + Created.GetHashCode();
                    if (Updated != null)
                    hashCode = hashCode * 59 + Updated.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(Drive left, Drive right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Drive left, Drive right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
