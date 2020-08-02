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
 * @interface GameParametersDto
 */
export interface GameParametersDto {
    /**
     * 
     * @type {string}
     * @memberof GameParametersDto
     */
    description?: string | null;
    /**
     * 
     * @type {number}
     * @memberof GameParametersDto
     */
    regionCount: number;
    /**
     * 
     * @type {boolean}
     * @memberof GameParametersDto
     */
    isPublic: boolean;
    /**
     * 
     * @type {boolean}
     * @memberof GameParametersDto
     */
    allowGuests: boolean;
}

export function GameParametersDtoFromJSON(json: any): GameParametersDto {
    return GameParametersDtoFromJSONTyped(json, false);
}

export function GameParametersDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): GameParametersDto {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'description': !exists(json, 'description') ? undefined : json['description'],
        'regionCount': json['regionCount'],
        'isPublic': json['isPublic'],
        'allowGuests': json['allowGuests'],
    };
}

export function GameParametersDtoToJSON(value?: GameParametersDto | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'description': value.description,
        'regionCount': value.regionCount,
        'isPublic': value.isPublic,
        'allowGuests': value.allowGuests,
    };
}


