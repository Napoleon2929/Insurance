using System;
using System.Text;

namespace Shared.Common
{
    public class Result
    {
        public static readonly Result Ok = new Result(true);

        public bool Success { get; }
        public bool Failure => !Success;
        public string Message { get; }
        public Exception Exception { get; }

        protected Result(bool success, Exception exception = null, string message = null)
        {
            Success = success;
            Exception = exception;
            Message = message;
        }

        public static Result Fail(string message) => new Result(false, message: message);

        public static Result Fail(Exception exception)
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

            return new Result(false, exception: exception, message: sb.ToString());
        }
    }

    public class Result<T> : Result
    {
        public T Value { get; }

        protected Result(bool success, T value = default(T),
            Exception exception = null, string message = null)
            : base(success, exception, message)
        {
            Value = value;
        }

        public new static Result<T> Ok(T value)
        {
            return new Result<T>(true, value);
        }

        public new static Result<T> Fail(string message)
        {
            return new Result<T>(false, message: message);
        }

        public new static Result<T> Fail(Exception exception)
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

            return new Result<T>(false, exception: exception, message: sb.ToString());
        }

        public static Result<T> Fail(Exception exception, string message)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            return new Result<T>(false, exception: exception, message: message);
        }

        public static Result<T> Fail<TU>(Result<TU> result)
        {
            return new Result<T>(result.Success, exception: result.Exception, message: result.Message);
        }

        public static implicit operator Result<T>(T value)
        {
            return Ok(value);
        }
    }
}
