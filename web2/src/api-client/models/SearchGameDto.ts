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
    GameParametersDto,
    GameParametersDtoFromJSON,
    GameParametersDtoFromJSONTyped,
    GameParametersDtoToJSON,
    GameStatus,
    GameStatusFromJSON,
    GameStatusFromJSONTyped,
    GameStatusToJSON,
} from './';

/**
 * 
 * @export
 * @interface SearchGameDto
 */
export interface SearchGameDto {
    /**
     * 
     * @type {number}
     * @memberof SearchGameDto
     */
    readonly id: number;
    /**
     * 
     * @type {GameParametersDto}
     * @memberof SearchGameDto
     */
    parameters: GameParametersDto;
    /**
     * 
     * @type {CreationSourceDto}
     * @memberof SearchGameDto
     */
    createdBy: CreationSourceDto;
    /**
     * 
     * @type {GameStatus}
     * @memberof SearchGameDto
     */
    status: GameStatus;
    /**
     * 
     * @type {Date}
     * @memberof SearchGameDto
     */
    readonly lastEventOn?: Date;
    /**
     * 
     * @type {number}
     * @memberof SearchGameDto
     */
    readonly playerCount: number;
    /**
     * 
     * @type {boolean}
     * @memberof SearchGameDto
     */
    readonly containsMe: boolean;
}

export function SearchGameDtoFromJSON(json: any): SearchGameDto {
    return SearchGameDtoFromJSONTyped(json, false);
}

export function SearchGameDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): SearchGameDto {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': json['id'],
        'parameters': GameParametersDtoFromJSON(json['parameters']),
        'createdBy': CreationSourceDtoFromJSON(json['createdBy']),
        'status': GameStatusFromJSON(json['status']),
        'lastEventOn': !exists(json, 'lastEventOn') ? undefined : (new Date(json['lastEventOn'])),
        'playerCount': json['playerCount'],
        'containsMe': json['containsMe'],
    };
}

export function SearchGameDtoToJSON(value?: SearchGameDto | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'parameters': GameParametersDtoToJSON(value.parameters),
        'createdBy': CreationSourceDtoToJSON(value.createdBy),
        'status': GameStatusToJSON(value.status),
    };
}


