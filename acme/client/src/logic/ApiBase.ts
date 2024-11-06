import AxiosInstance from './AxiosInstance'
import axios, { AxiosError } from 'axios'
import ApiException from '../model/ApiException'
import ApiResponse from '../model/ApiResponse'
import { ValidationProblemDetails } from '../model/models'

export default class ApiBase {
  private client = AxiosInstance

  async get<T>(url: string, expectString: boolean = false): Promise<ApiResponse<T | null>> {
    try {
      const res = await this.client.get<T>(url)
      let responseData: T | null = res.data
      if (res.data === '' && !expectString) {
        responseData = null
      }
      return new ApiResponse(res.status, responseData)
    } catch (err) {
      if (axios.isAxiosError(err)) {
        const statusCode = ApiBase.getStatusCode(err)
        return new ApiResponse<T>(statusCode, null, ApiBase.getApiExceptionFromAxiosError(err))
      } else {
        return new ApiResponse<T>(500, null, ApiBase.getApiExceptionFromStockError(err as Error))
      }
    }
  }

  async post<T>(url: string, data: unknown): Promise<ApiResponse<T | null>> {
    try {
      const res = await this.client.post<T>(url, data)
      return new ApiResponse(res.status, res.data)
    } catch (err) {
      if (axios.isAxiosError(err)) {
        const statusCode = ApiBase.getStatusCode(err)
        return new ApiResponse<T>(statusCode, null, ApiBase.getApiExceptionFromAxiosError(err))
      } else {
        return new ApiResponse<T>(500, null, ApiBase.getApiExceptionFromStockError(err as Error))
      }
    }
  }

  async put<T>(url: string, data: unknown): Promise<ApiResponse<T | null>> {
    try {
      const res = await this.client.put<T>(url, data)
      return new ApiResponse(res.status, res.data)
    } catch (err) {
      if (axios.isAxiosError(err)) {
        const statusCode = ApiBase.getStatusCode(err)
        return new ApiResponse<T>(statusCode, null, ApiBase.getApiExceptionFromAxiosError(err))
      } else {
        return new ApiResponse<T>(500, null, ApiBase.getApiExceptionFromStockError(err as Error))
      }
    }
  }

  async delete(url: string): Promise<void> {
    try {
      await this.client.delete(url)
    } catch (err) {
      if (axios.isAxiosError(err)) {
        throw ApiBase.getApiExceptionFromAxiosError(err)
      } else {
        throw ApiBase.getApiExceptionFromStockError(err as Error)
      }
    }
  }

  private static getApiExceptionFromAxiosError(error: AxiosError): ApiException {
    if (error.response) {
      const code = error.response.status
      const data = error.response.data as ValidationProblemDetails

      if (code === 400 && data && data.title && data.errors) {
        return ApiException.fromValidationExceptionResponseData(code, data)
      }

      return new ApiException(code, JSON.stringify(data))
    }

    if (error.request) {
      throw new ApiException(500, 'no response received')
    }

    const stockErrorMessage = 'no request or response available'
    return new ApiException(500, stockErrorMessage)
  }

  private static getApiExceptionFromStockError(error: Error): ApiException {
    return new ApiException(500, JSON.stringify(error))
  }

  private static getStatusCode(axiosError: AxiosError): number {
    let statusCode: number
    try {
      statusCode = axiosError?.response?.status || 500
    } catch (err) {
      console.error('error parsing axios error code - using 500', err)
      statusCode = 500
    }
    return statusCode
  }
}
