/* tslint:disable */
/* eslint-disable */
/**
 * Djambi-N API
 * API for Djambi-N
 *
 * The version of the OpenAPI document: v1
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */

/**
 * 
 * @export
 * @enum {string}
 */
export enum PlayerKind {
    User = 'User',
    Guest = 'Guest',
    Neutral = 'Neutral'
}

export function PlayerKindFromJSON(json: any): PlayerKind {
    return PlayerKindFromJSONTyped(json, false);
}

export function PlayerKindFromJSONTyped(json: any, ignoreDiscriminator: boolean): PlayerKind {
    return json as PlayerKind;
}

export function PlayerKindToJSON(value?: PlayerKind | null): any {
    return value as any;
}

