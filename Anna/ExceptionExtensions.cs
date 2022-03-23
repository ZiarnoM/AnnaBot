using System;
using System.Runtime.Serialization;
using System.Text;

namespace Anna
{
    public static class ExceptionExtensions
    {
        public static string GetMergedErrors(this Exception exception)
        {
            if (exception == null) return null;

            StringBuilder mergedErrors = new StringBuilder(" ");
            Exception currentException = exception;

            do
            {
                if (!string.IsNullOrWhiteSpace(currentException.Message)) mergedErrors.Append(string.Format(" {0} {1} ", mergedErrors, currentException.Message));
                currentException = currentException.InnerException;
            }
            while (currentException != null);

            return mergedErrors.ToString();
        }

        public static string GetMergedErrors(this Result result)
        {
            if (result == null) return null;
            StringBuilder mergedErrors = new StringBuilder(" ");
            
            foreach(Error error in result.Errors)
            {
                mergedErrors.AppendLine(error.ErrorDescription);
            }
            return mergedErrors.ToString();
        }
    }
    public class ValidateException : Exception
    {
        public ValidateException()
        {
        }

        public ValidateException(string message) : base(message)
        {
        }

        public ValidateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ValidateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}