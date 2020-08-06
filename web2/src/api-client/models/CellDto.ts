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
import {
    LocationDto,
    LocationDtoFromJSON,
    LocationDtoFromJSONTyped,
    LocationDtoToJSON,
} from './';

/**
 * 
 * @export
 * @interface CellDto
 */
export interface CellDto {
    /**
     * 
     * @type {number}
     * @memberof CellDto
     */
    id: number;
    /**
     * 
     * @type {Array<LocationDto>}
     * @memberof CellDto
     */
    locations: Array<LocationDto>;
}

export function CellDtoFromJSON(json: any): CellDto {
    return CellDtoFromJSONTyped(json, false);
}

export function CellDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): CellDto {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': json['id'],
        'locations': ((json['locations'] as Array<any>).map(LocationDtoFromJSON)),
    };
}

export function CellDtoToJSON(value?: CellDto | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'locations': ((value.locations as Array<any>).map(LocationDtoToJSON)),
    };
}


