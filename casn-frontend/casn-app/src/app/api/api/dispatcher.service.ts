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
/* tslint:disable:no-unused-variable member-ordering */

import { Inject, Injectable, Optional }                      from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams,
         HttpResponse, HttpEvent }                           from '@angular/common/http';
import { CustomHttpUrlEncodingCodec }                        from '../encoder';

import { Observable }                                        from 'rxjs';

import { AllAppointments } from '../model/allAppointments.model';
import { AppointmentDTO } from '../model/appointmentDTO.model';
import { Body1 } from '../model/body1.model';
import { DeleteSuccessMessage } from '../model/deleteSuccessMessage.model';
import { Patient } from '../model/patient.model';
import { VolunteerDrive } from '../model/volunteerDrive.model';

import { BASE_PATH, COLLECTION_FORMATS }                     from '../variables';
import { Configuration }                                     from '../configuration';
import { DispatcherServiceInterface }                            from './dispatcher.serviceInterface';


@Injectable({
  providedIn: 'root'
})
export class DispatcherService implements DispatcherServiceInterface {

    protected basePath = 'https://virtserver.swaggerhub.com/d-m-wilson/CASN_App_OAS3/1.0.0';
    public defaultHeaders = new HttpHeaders();
    public configuration = new Configuration();

    constructor(protected httpClient: HttpClient, @Optional()@Inject(BASE_PATH) basePath: string, @Optional() configuration: Configuration) {

        if (configuration) {
            this.configuration = configuration;
            this.configuration.basePath = configuration.basePath || basePath || this.basePath;

        } else {
            this.configuration.basePath = basePath || this.basePath;
        }
    }

    /**
     * @param consumes string[] mime-types
     * @return true: consumes contains 'multipart/form-data', false: otherwise
     */
    private canConsumeForm(consumes: string[]): boolean {
        const form = 'multipart/form-data';
        for (const consume of consumes) {
            if (form === consume) {
                return true;
            }
        }
        return false;
    }


    /**
     * adds a new appointment
     * Adds appointment (and drives) to the system
     * @param appointmentDTO appointmentData to add
     * @param observe set whether or not to return the data Observable as the body, response or events. defaults to returning the body.
     * @param reportProgress flag to report request and response progress.
     */
    public addAppointment(appointmentDTO?: AppointmentDTO, observe?: 'body', reportProgress?: boolean): Observable<AppointmentDTO>;
    public addAppointment(appointmentDTO?: AppointmentDTO, observe?: 'response', reportProgress?: boolean): Observable<HttpResponse<AppointmentDTO>>;
    public addAppointment(appointmentDTO?: AppointmentDTO, observe?: 'events', reportProgress?: boolean): Observable<HttpEvent<AppointmentDTO>>;
    public addAppointment(appointmentDTO?: AppointmentDTO, observe: any = 'body', reportProgress: boolean = false ): Observable<any> {

        let headers = this.defaultHeaders;

        // authentication (BearerAuth) required
        if (this.configuration.username || this.configuration.password) {
            headers = headers.set('Authorization', 'Basic ' + btoa(this.configuration.username + ':' + this.configuration.password));
        }

        // to determine the Accept header
        let httpHeaderAccepts: string[] = [
            'application/json'
        ];
        const httpHeaderAcceptSelected: string | undefined = this.configuration.selectHeaderAccept(httpHeaderAccepts);
        if (httpHeaderAcceptSelected !== undefined) {
            headers = headers.set('Accept', httpHeaderAcceptSelected);
        }

        // to determine the Content-Type header
        const consumes: string[] = [
            'application/json'
        ];
        const httpContentTypeSelected: string | undefined = this.configuration.selectHeaderContentType(consumes);
        if (httpContentTypeSelected !== undefined) {
            headers = headers.set('Content-Type', httpContentTypeSelected);
        }

        return this.httpClient.post<AppointmentDTO>(`${this.configuration.basePath}/dispatcher/appointments`,
            appointmentDTO,
            {
                withCredentials: this.configuration.withCredentials,
                headers: headers,
                observe: observe,
                reportProgress: reportProgress
            }
        );
    }

    /**
     * approves a volunteer for a drive
     * Adds driverId to a drive
     * @param body1 
     * @param observe set whether or not to return the data Observable as the body, response or events. defaults to returning the body.
     * @param reportProgress flag to report request and response progress.
     */
    public addDriver(body1?: Body1, observe?: 'body', reportProgress?: boolean): Observable<any>;
    public addDriver(body1?: Body1, observe?: 'response', reportProgress?: boolean): Observable<HttpResponse<any>>;
    public addDriver(body1?: Body1, observe?: 'events', reportProgress?: boolean): Observable<HttpEvent<any>>;
    public addDriver(body1?: Body1, observe: any = 'body', reportProgress: boolean = false ): Observable<any> {

        let headers = this.defaultHeaders;

        // authentication (BearerAuth) required
        if (this.configuration.username || this.configuration.password) {
            headers = headers.set('Authorization', 'Basic ' + btoa(this.configuration.username + ':' + this.configuration.password));
        }

        // to determine the Accept header
        let httpHeaderAccepts: string[] = [
        ];
        const httpHeaderAcceptSelected: string | undefined = this.configuration.selectHeaderAccept(httpHeaderAccepts);
        if (httpHeaderAcceptSelected !== undefined) {
            headers = headers.set('Accept', httpHeaderAcceptSelected);
        }

        // to determine the Content-Type header
        const consumes: string[] = [
            'application/json'
        ];
        const httpContentTypeSelected: string | undefined = this.configuration.selectHeaderContentType(consumes);
        if (httpContentTypeSelected !== undefined) {
            headers = headers.set('Content-Type', httpContentTypeSelected);
        }

        return this.httpClient.post<any>(`${this.configuration.basePath}/drives/approve`,
            body1,
            {
                withCredentials: this.configuration.withCredentials,
                headers: headers,
                observe: observe,
                reportProgress: reportProgress
            }
        );
    }

    /**
     * adds a patient
     * Adds patient to the system
     * @param patient patientData to add
     * @param observe set whether or not to return the data Observable as the body, response or events. defaults to returning the body.
     * @param reportProgress flag to report request and response progress.
     */
    public addPatient(patient?: Patient, observe?: 'body', reportProgress?: boolean): Observable<any>;
    public addPatient(patient?: Patient, observe?: 'response', reportProgress?: boolean): Observable<HttpResponse<any>>;
    public addPatient(patient?: Patient, observe?: 'events', reportProgress?: boolean): Observable<HttpEvent<any>>;
    public addPatient(patient?: Patient, observe: any = 'body', reportProgress: boolean = false ): Observable<any> {

        let headers = this.defaultHeaders;

        // authentication (BearerAuth) required
        if (this.configuration.username || this.configuration.password) {
            headers = headers.set('Authorization', 'Basic ' + btoa(this.configuration.username + ':' + this.configuration.password));
        }

        // to determine the Accept header
        let httpHeaderAccepts: string[] = [
        ];
        const httpHeaderAcceptSelected: string | undefined = this.configuration.selectHeaderAccept(httpHeaderAccepts);
        if (httpHeaderAcceptSelected !== undefined) {
            headers = headers.set('Accept', httpHeaderAcceptSelected);
        }

        // to determine the Content-Type header
        const consumes: string[] = [
            'application/json'
        ];
        const httpContentTypeSelected: string | undefined = this.configuration.selectHeaderContentType(consumes);
        if (httpContentTypeSelected !== undefined) {
            headers = headers.set('Content-Type', httpContentTypeSelected);
        }

        return this.httpClient.post<any>(`${this.configuration.basePath}/patient`,
            patient,
            {
                withCredentials: this.configuration.withCredentials,
                headers: headers,
                observe: observe,
                reportProgress: reportProgress
            }
        );
    }

    /**
     * 
     * 
     * @param appointmentID pass an appointmentIdentifier
     * @param observe set whether or not to return the data Observable as the body, response or events. defaults to returning the body.
     * @param reportProgress flag to report request and response progress.
     */
    public dispatcherAppointmentsAppointmentIDDelete(appointmentID: string, observe?: 'body', reportProgress?: boolean): Observable<DeleteSuccessMessage>;
    public dispatcherAppointmentsAppointmentIDDelete(appointmentID: string, observe?: 'response', reportProgress?: boolean): Observable<HttpResponse<DeleteSuccessMessage>>;
    public dispatcherAppointmentsAppointmentIDDelete(appointmentID: string, observe?: 'events', reportProgress?: boolean): Observable<HttpEvent<DeleteSuccessMessage>>;
    public dispatcherAppointmentsAppointmentIDDelete(appointmentID: string, observe: any = 'body', reportProgress: boolean = false ): Observable<any> {
        if (appointmentID === null || appointmentID === undefined) {
            throw new Error('Required parameter appointmentID was null or undefined when calling dispatcherAppointmentsAppointmentIDDelete.');
        }

        let headers = this.defaultHeaders;

        // authentication (BearerAuth) required
        if (this.configuration.username || this.configuration.password) {
            headers = headers.set('Authorization', 'Basic ' + btoa(this.configuration.username + ':' + this.configuration.password));
        }

        // to determine the Accept header
        let httpHeaderAccepts: string[] = [
            '*/*'
        ];
        const httpHeaderAcceptSelected: string | undefined = this.configuration.selectHeaderAccept(httpHeaderAccepts);
        if (httpHeaderAcceptSelected !== undefined) {
            headers = headers.set('Accept', httpHeaderAcceptSelected);
        }

        // to determine the Content-Type header
        const consumes: string[] = [
        ];

        return this.httpClient.delete<DeleteSuccessMessage>(`${this.configuration.basePath}/dispatcher/appointments/${encodeURIComponent(String(appointmentID))}`,
            {
                withCredentials: this.configuration.withCredentials,
                headers: headers,
                observe: observe,
                reportProgress: reportProgress
            }
        );
    }

    /**
     * gets appointments with dispatcher-level details
     * Get all appointments within a default date range (possibly adjustable w/ query params). Appointments include details, e.g. exact location, available only to dispatchers. 
     * @param startDate pass a startDate by which to filter
     * @param endDate pass an endDate by which to filter
     * @param observe set whether or not to return the data Observable as the body, response or events. defaults to returning the body.
     * @param reportProgress flag to report request and response progress.
     */
    public getAllAppointmentsForDispatcher(startDate?: string, endDate?: string, observe?: 'body', reportProgress?: boolean): Observable<AllAppointments>;
    public getAllAppointmentsForDispatcher(startDate?: string, endDate?: string, observe?: 'response', reportProgress?: boolean): Observable<HttpResponse<AllAppointments>>;
    public getAllAppointmentsForDispatcher(startDate?: string, endDate?: string, observe?: 'events', reportProgress?: boolean): Observable<HttpEvent<AllAppointments>>;
    public getAllAppointmentsForDispatcher(startDate?: string, endDate?: string, observe: any = 'body', reportProgress: boolean = false ): Observable<any> {

        let queryParameters = new HttpParams({encoder: new CustomHttpUrlEncodingCodec()});
        if (startDate !== undefined && startDate !== null) {
            queryParameters = queryParameters.set('startDate', <any>startDate);
        }
        if (endDate !== undefined && endDate !== null) {
            queryParameters = queryParameters.set('endDate', <any>endDate);
        }

        let headers = this.defaultHeaders;

        // authentication (BearerAuth) required
        if (this.configuration.username || this.configuration.password) {
            headers = headers.set('Authorization', 'Basic ' + btoa(this.configuration.username + ':' + this.configuration.password));
        }

        // to determine the Accept header
        let httpHeaderAccepts: string[] = [
            'application/json'
        ];
        const httpHeaderAcceptSelected: string | undefined = this.configuration.selectHeaderAccept(httpHeaderAccepts);
        if (httpHeaderAcceptSelected !== undefined) {
            headers = headers.set('Accept', httpHeaderAcceptSelected);
        }

        // to determine the Content-Type header
        const consumes: string[] = [
        ];

        return this.httpClient.get<AllAppointments>(`${this.configuration.basePath}/dispatcher/appointments`,
            {
                params: queryParameters,
                withCredentials: this.configuration.withCredentials,
                headers: headers,
                observe: observe,
                reportProgress: reportProgress
            }
        );
    }

    /**
     * gets appointment by appointmentID
     * Search for existing appointment by appointmentIdentifier, return dispatcher-level details 
     * @param appointmentID pass an appointmentIdentifier
     * @param observe set whether or not to return the data Observable as the body, response or events. defaults to returning the body.
     * @param reportProgress flag to report request and response progress.
     */
    public getAppointmentForDispatcherByID(appointmentID: string, observe?: 'body', reportProgress?: boolean): Observable<AppointmentDTO>;
    public getAppointmentForDispatcherByID(appointmentID: string, observe?: 'response', reportProgress?: boolean): Observable<HttpResponse<AppointmentDTO>>;
    public getAppointmentForDispatcherByID(appointmentID: string, observe?: 'events', reportProgress?: boolean): Observable<HttpEvent<AppointmentDTO>>;
    public getAppointmentForDispatcherByID(appointmentID: string, observe: any = 'body', reportProgress: boolean = false ): Observable<any> {
        if (appointmentID === null || appointmentID === undefined) {
            throw new Error('Required parameter appointmentID was null or undefined when calling getAppointmentForDispatcherByID.');
        }

        let headers = this.defaultHeaders;

        // authentication (BearerAuth) required
        if (this.configuration.username || this.configuration.password) {
            headers = headers.set('Authorization', 'Basic ' + btoa(this.configuration.username + ':' + this.configuration.password));
        }

        // to determine the Accept header
        let httpHeaderAccepts: string[] = [
            'application/json'
        ];
        const httpHeaderAcceptSelected: string | undefined = this.configuration.selectHeaderAccept(httpHeaderAccepts);
        if (httpHeaderAcceptSelected !== undefined) {
            headers = headers.set('Accept', httpHeaderAcceptSelected);
        }

        // to determine the Content-Type header
        const consumes: string[] = [
        ];

        return this.httpClient.get<AppointmentDTO>(`${this.configuration.basePath}/dispatcher/appointments/${encodeURIComponent(String(appointmentID))}`,
            {
                withCredentials: this.configuration.withCredentials,
                headers: headers,
                observe: observe,
                reportProgress: reportProgress
            }
        );
    }

    /**
     * gets patient by patientIdentifier
     * Search for existing patients by the dispatcher created patientIdentifier (patient ID) 
     * @param patientIdentifier pass a search string for looking up patientIdentifier
     * @param observe set whether or not to return the data Observable as the body, response or events. defaults to returning the body.
     * @param reportProgress flag to report request and response progress.
     */
    public getPatientByPatientIdentifier(patientIdentifier: string, observe?: 'body', reportProgress?: boolean): Observable<Patient>;
    public getPatientByPatientIdentifier(patientIdentifier: string, observe?: 'response', reportProgress?: boolean): Observable<HttpResponse<Patient>>;
    public getPatientByPatientIdentifier(patientIdentifier: string, observe?: 'events', reportProgress?: boolean): Observable<HttpEvent<Patient>>;
    public getPatientByPatientIdentifier(patientIdentifier: string, observe: any = 'body', reportProgress: boolean = false ): Observable<any> {
        if (patientIdentifier === null || patientIdentifier === undefined) {
            throw new Error('Required parameter patientIdentifier was null or undefined when calling getPatientByPatientIdentifier.');
        }

        let queryParameters = new HttpParams({encoder: new CustomHttpUrlEncodingCodec()});
        if (patientIdentifier !== undefined && patientIdentifier !== null) {
            queryParameters = queryParameters.set('patientIdentifier', <any>patientIdentifier);
        }

        let headers = this.defaultHeaders;

        // authentication (BearerAuth) required
        if (this.configuration.username || this.configuration.password) {
            headers = headers.set('Authorization', 'Basic ' + btoa(this.configuration.username + ':' + this.configuration.password));
        }

        // to determine the Accept header
        let httpHeaderAccepts: string[] = [
            'application/json'
        ];
        const httpHeaderAcceptSelected: string | undefined = this.configuration.selectHeaderAccept(httpHeaderAccepts);
        if (httpHeaderAcceptSelected !== undefined) {
            headers = headers.set('Accept', httpHeaderAcceptSelected);
        }

        // to determine the Content-Type header
        const consumes: string[] = [
        ];

        return this.httpClient.get<Patient>(`${this.configuration.basePath}/patient`,
            {
                params: queryParameters,
                withCredentials: this.configuration.withCredentials,
                headers: headers,
                observe: observe,
                reportProgress: reportProgress
            }
        );
    }

    /**
     * get list of applicants for a drive
     * 
     * @param driveId id of drive
     * @param observe set whether or not to return the data Observable as the body, response or events. defaults to returning the body.
     * @param reportProgress flag to report request and response progress.
     */
    public getVolunteerDrives(driveId: number, observe?: 'body', reportProgress?: boolean): Observable<Array<VolunteerDrive>>;
    public getVolunteerDrives(driveId: number, observe?: 'response', reportProgress?: boolean): Observable<HttpResponse<Array<VolunteerDrive>>>;
    public getVolunteerDrives(driveId: number, observe?: 'events', reportProgress?: boolean): Observable<HttpEvent<Array<VolunteerDrive>>>;
    public getVolunteerDrives(driveId: number, observe: any = 'body', reportProgress: boolean = false ): Observable<any> {
        if (driveId === null || driveId === undefined) {
            throw new Error('Required parameter driveId was null or undefined when calling getVolunteerDrives.');
        }

        let queryParameters = new HttpParams({encoder: new CustomHttpUrlEncodingCodec()});
        if (driveId !== undefined && driveId !== null) {
            queryParameters = queryParameters.set('driveId', <any>driveId);
        }

        let headers = this.defaultHeaders;

        // authentication (BearerAuth) required
        if (this.configuration.username || this.configuration.password) {
            headers = headers.set('Authorization', 'Basic ' + btoa(this.configuration.username + ':' + this.configuration.password));
        }

        // to determine the Accept header
        let httpHeaderAccepts: string[] = [
            'application/json'
        ];
        const httpHeaderAcceptSelected: string | undefined = this.configuration.selectHeaderAccept(httpHeaderAccepts);
        if (httpHeaderAcceptSelected !== undefined) {
            headers = headers.set('Accept', httpHeaderAcceptSelected);
        }

        // to determine the Content-Type header
        const consumes: string[] = [
        ];

        return this.httpClient.get<Array<VolunteerDrive>>(`${this.configuration.basePath}/volunteerDrive`,
            {
                params: queryParameters,
                withCredentials: this.configuration.withCredentials,
                headers: headers,
                observe: observe,
                reportProgress: reportProgress
            }
        );
    }

    /**
     * updates an existing appointment
     * Updates appointment (and corresponding drive) information
     * @param appointmentID pass an appointmentIdentifier
     * @param appointmentDTO appointmentData with updated fields
     * @param observe set whether or not to return the data Observable as the body, response or events. defaults to returning the body.
     * @param reportProgress flag to report request and response progress.
     */
    public updateAppointment(appointmentID: string, appointmentDTO: AppointmentDTO, observe?: 'body', reportProgress?: boolean): Observable<AppointmentDTO>;
    public updateAppointment(appointmentID: string, appointmentDTO: AppointmentDTO, observe?: 'response', reportProgress?: boolean): Observable<HttpResponse<AppointmentDTO>>;
    public updateAppointment(appointmentID: string, appointmentDTO: AppointmentDTO, observe?: 'events', reportProgress?: boolean): Observable<HttpEvent<AppointmentDTO>>;
    public updateAppointment(appointmentID: string, appointmentDTO: AppointmentDTO, observe: any = 'body', reportProgress: boolean = false ): Observable<any> {
        if (appointmentID === null || appointmentID === undefined) {
            throw new Error('Required parameter appointmentID was null or undefined when calling updateAppointment.');
        }
        if (appointmentDTO === null || appointmentDTO === undefined) {
            throw new Error('Required parameter appointmentDTO was null or undefined when calling updateAppointment.');
        }

        let headers = this.defaultHeaders;

        // authentication (BearerAuth) required
        if (this.configuration.username || this.configuration.password) {
            headers = headers.set('Authorization', 'Basic ' + btoa(this.configuration.username + ':' + this.configuration.password));
        }

        // to determine the Accept header
        let httpHeaderAccepts: string[] = [
            'application/json'
        ];
        const httpHeaderAcceptSelected: string | undefined = this.configuration.selectHeaderAccept(httpHeaderAccepts);
        if (httpHeaderAcceptSelected !== undefined) {
            headers = headers.set('Accept', httpHeaderAcceptSelected);
        }

        // to determine the Content-Type header
        const consumes: string[] = [
            'application/json'
        ];
        const httpContentTypeSelected: string | undefined = this.configuration.selectHeaderContentType(consumes);
        if (httpContentTypeSelected !== undefined) {
            headers = headers.set('Content-Type', httpContentTypeSelected);
        }

        return this.httpClient.put<AppointmentDTO>(`${this.configuration.basePath}/dispatcher/appointments/${encodeURIComponent(String(appointmentID))}`,
            appointmentDTO,
            {
                withCredentials: this.configuration.withCredentials,
                headers: headers,
                observe: observe,
                reportProgress: reportProgress
            }
        );
    }

}