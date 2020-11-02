using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Shared.Common
{
    public class ApiResult : Result
    {
        public new static readonly ApiResult Ok = new ApiResult(HttpStatusCode.OK, true);

        public HttpStatusCode StatusCode { get; }
        public int StatusCodeInt => (int)StatusCode;

        protected ApiResult(HttpStatusCode statusCode, bool success, Exception exception = null, string message = null)
            : base(success, exception, message)
        {
            StatusCode = statusCode;
        }

        public static ApiResult Bad(string message)
            => Fail(HttpStatusCode.BadRequest, message);

        public new static ApiResult Fail(Exception exception)
            => Fail(HttpStatusCode.InternalServerError, exception);

        public static ApiResult NotFound(string message)
            => Fail(HttpStatusCode.NotFound, message);

        public static ApiResult Fail(HttpStatusCode statusCode, Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            var ex = exception;
            var sb = new StringBuilder();
            while (ex != null)
            {
                sb.AppendLine(ex.Message);
                ex = ex.InnerException;
            }

            return new ApiResult(statusCode, false, exception, sb.ToString());
        }

        public static ApiResult Fail(HttpStatusCode statusCode, Exception exception, string message)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            return new ApiResult(statusCode, false, exception, message);
        }

        public static ApiResult Fail(HttpStatusCode statusCode, string message) => new ApiResult(statusCode, false, message: message);

        public static ApiResult From(HttpResponseMessage response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            string message = null;
            if (!response.IsSuccessStatusCode)
                message = response.Content.ReadAsStringAsync().Result;

            return new ApiResult(response.StatusCode, response.IsSuccessStatusCode, message: message);
        }

        public static ApiResult From<T>(ApiResult<T> apiResult)
        {
            if (apiResult == null)
                throw new ArgumentNullException(nameof(apiResult));

            return new ApiResult(apiResult.StatusCode, apiResult.Success, apiResult.Exception, apiResult.Message);
        }

        public new static ApiResult Fail(string message) => Fail(HttpStatusCode.InternalServerError, message);
    }

    public class ApiResult<T> : Result<T>
    {
        public HttpStatusCode StatusCode { get; }
        public int StatusCodeInt => (int)StatusCode;
        public ValidationResult ValidationResult { get; }
        public bool IsNotFound => StatusCode == HttpStatusCode.NotFound;

        protected ApiResult(HttpStatusCode statusCode, bool success,
            T value = default(T), Exception exception = null, string message = null, ValidationResult validationResult = null)
            : base(success, value, exception, message)
        {
            StatusCode = statusCode;
            ValidationResult = validationResult;
        }

        public new static ApiResult<T> Ok() => Ok(default(T));
        public new static ApiResult<T> Ok(T value) => new ApiResult<T>(HttpStatusCode.OK, true, value);

        public static ApiResult<T> NoContent() => new ApiResult<T>(HttpStatusCode.NoContent, true);

        public static ApiResult<T> Bad(string message) => Fail(HttpStatusCode.BadRequest, message);

        public static ApiResult<T> Bad(ValidationResult validationResult) => Fail(HttpStatusCode.BadRequest, validationResult);

        public static ApiResult<T> NotFound(string message) => Fail(HttpStatusCode.NotFound, message);

        public new static ApiResult<T> Fail(string message) => Fail(HttpStatusCode.InternalServerError, message);

        private static ApiResult<T> Fail(HttpStatusCode badRequest, ValidationResult validationResult)
            => new ApiResult<T>(badRequest, false, default(T), null, null, validationResult);

        public static ApiResult<T> Fail(HttpStatusCode statusCode, string message) => Fail(statusCode, null, message);

        public static ApiResult<T> Fail<TU>(ApiResult<TU> apiResult)
        {
            return new ApiResult<T>(apiResult.StatusCode, apiResult.Success, exception: apiResult.Exception,
                message: apiResult.Message);
        }

        public static ApiResult<T> Fail(HttpStatusCode statusCode, Exception exception, string message)
            => new ApiResult<T>(statusCode, false, default(T), exception, message);

        public new static ApiResult<T> Fail(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            var ex = exception;
            var sb = new StringBuilder();
            while (ex != null)
            {
                sb.AppendLine(ex.Message);
                ex = ex.InnerException;
            }

            return new ApiResult<T>(HttpStatusCode.InternalServerError, false, default(T), exception, sb.ToString());
        }

        public static implicit operator ApiResult<T>(T value)
        {
            return Ok(value);
        }
    }
}
