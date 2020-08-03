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


import * as runtime from '../runtime';
import {
    LoginRequestDto,
    LoginRequestDtoFromJSON,
    LoginRequestDtoToJSON,
    SessionDto,
    SessionDtoFromJSON,
    SessionDtoToJSON,
} from '../models';

export interface ApiSessionsPostRequest {
    loginRequestDto?: LoginRequestDto;
}

/**
 * 
 */
export class SessionApi extends runtime.BaseAPI {

    /**
     */
    async apiSessionsDeleteRaw(): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/sessions`,
            method: 'DELETE',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async apiSessionsDelete(): Promise<void> {
        await this.apiSessionsDeleteRaw();
    }

    /**
     */
    async apiSessionsPostRaw(requestParameters: ApiSessionsPostRequest): Promise<runtime.ApiResponse<SessionDto>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json-patch+json';

        const response = await this.request({
            path: `/api/sessions`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: LoginRequestDtoToJSON(requestParameters.loginRequestDto),
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => SessionDtoFromJSON(jsonValue));
    }

    /**
     */
    async apiSessionsPost(requestParameters: ApiSessionsPostRequest): Promise<SessionDto> {
        const response = await this.apiSessionsPostRaw(requestParameters);
        return await response.value();
    }

}