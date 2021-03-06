/**
 * CASN API
 * CASN API (ASP.NET Core 2.1)
 *
 * OpenAPI spec version: 2.0
 * Contact: katie@clinicaccess.org
 *
 * NOTE: This class is auto generated by the swagger code generator program.
 * https://github.com/swagger-api/swagger-codegen.git
 * Do not edit the class manually.
 */


export interface CASNAppCoreModelsDrive { 
    id?: number;
    appointmentId?: number;
    direction: number;
    statusId?: number;
    status?: string;
    driverId?: number;
    startAddress?: string;
    startCity?: string;
    startState?: string;
    startPostalCode?: string;
    startLatitude?: number;
    startLongitude?: number;
    endAddress?: string;
    endCity?: string;
    endState?: string;
    endPostalCode?: string;
    endLatitude?: number;
    endLongitude?: number;
    created?: Date;
    updated?: Date;
    approved?: Date;
    approvedById?: number;
    cancelReasonId?: number;
    cancelReason?: string;
}
