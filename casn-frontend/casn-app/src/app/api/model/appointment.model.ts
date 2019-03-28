/**
 * CASN API
 * This is a test CASN API
 *
 * OpenAPI spec version: 1.0.0
 * Contact: katie@clinicaccess.org
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */


export interface Appointment {
    readonly id?: number;
    dispatcherId: number;
    callerId: number;
    clinicId: number;
    pickupLocationVague?: string;
    dropoffLocationVague?: string;
    appointmentDate: Date;
    appointmentTypeId: number;
    readonly created?: Date;
    readonly updated?: Date;
}