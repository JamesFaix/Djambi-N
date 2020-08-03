/* tslint:disable */
/* eslint-disable */
/**
 * Apex API
 * API for Apex
 *
 * The version of the OpenAPI document: v1
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */

import { exists, mapValues } from '../runtime';
/**
 * 
 * @export
 * @interface CreationSourceDto
 */
export interface CreationSourceDto {
    /**
     * 
     * @type {number}
     * @memberof CreationSourceDto
     */
    readonly userId: number;
    /**
     * 
     * @type {string}
     * @memberof CreationSourceDto
     */
    readonly userName: string;
    /**
     * 
     * @type {Date}
     * @memberof CreationSourceDto
     */
    readonly time: Date;
}

export function CreationSourceDtoFromJSON(json: any): CreationSourceDto {
    return CreationSourceDtoFromJSONTyped(json, false);
}

export function CreationSourceDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): CreationSourceDto {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'userId': json['userId'],
        'userName': json['userName'],
        'time': (new Date(json['time'])),
    };
}

export function CreationSourceDtoToJSON(value?: CreationSourceDto | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
    };
}

