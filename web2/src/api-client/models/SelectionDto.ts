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
    SelectionKind,
    SelectionKindFromJSON,
    SelectionKindFromJSONTyped,
    SelectionKindToJSON,
} from './';

/**
 * 
 * @export
 * @interface SelectionDto
 */
export interface SelectionDto {
    /**
     * 
     * @type {SelectionKind}
     * @memberof SelectionDto
     */
    kind: SelectionKind;
    /**
     * 
     * @type {number}
     * @memberof SelectionDto
     */
    cellId: number;
    /**
     * 
     * @type {number}
     * @memberof SelectionDto
     */
    pieceId?: number | null;
}

export function SelectionDtoFromJSON(json: any): SelectionDto {
    return SelectionDtoFromJSONTyped(json, false);
}

export function SelectionDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): SelectionDto {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'kind': SelectionKindFromJSON(json['kind']),
        'cellId': json['cellId'],
        'pieceId': !exists(json, 'pieceId') ? undefined : json['pieceId'],
    };
}

export function SelectionDtoToJSON(value?: SelectionDto | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'kind': SelectionKindToJSON(value.kind),
        'cellId': value.cellId,
        'pieceId': value.pieceId,
    };
}


