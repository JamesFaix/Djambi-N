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
    CreationSourceDto,
    CreationSourceDtoFromJSON,
    CreationSourceDtoFromJSONTyped,
    CreationSourceDtoToJSON,
} from './';

/**
 * 
 * @export
 * @interface SnapshotInfoDto
 */
export interface SnapshotInfoDto {
    /**
     * 
     * @type {number}
     * @memberof SnapshotInfoDto
     */
    id: number;
    /**
     * 
     * @type {CreationSourceDto}
     * @memberof SnapshotInfoDto
     */
    createdBy: CreationSourceDto;
    /**
     * 
     * @type {string}
     * @memberof SnapshotInfoDto
     */
    description: string;
}

export function SnapshotInfoDtoFromJSON(json: any): SnapshotInfoDto {
    return SnapshotInfoDtoFromJSONTyped(json, false);
}

export function SnapshotInfoDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): SnapshotInfoDto {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': json['id'],
        'createdBy': CreationSourceDtoFromJSON(json['createdBy']),
        'description': json['description'],
    };
}

export function SnapshotInfoDtoToJSON(value?: SnapshotInfoDto | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'createdBy': CreationSourceDtoToJSON(value.createdBy),
        'description': value.description,
    };
}


