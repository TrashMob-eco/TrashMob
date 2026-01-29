namespace TrashMob.Shared.Poco
{
    /// <summary>
    /// Represents the result of a service operation with optional data and error information.
    /// </summary>
    /// <typeparam name="T">The type of data returned on success.</typeparam>
    public class ServiceResult<T>
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the data returned by the operation when successful.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Gets or sets the error message when the operation fails.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Creates a successful result with the specified data.
        /// </summary>
        /// <param name="data">The data to return.</param>
        /// <returns>A successful service result.</returns>
        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Data = data,
                ErrorMessage = null
            };
        }

        /// <summary>
        /// Creates a failed result with the specified error message.
        /// </summary>
        /// <param name="errorMessage">The error message describing the failure.</param>
        /// <returns>A failed service result.</returns>
        public static ServiceResult<T> Failure(string errorMessage)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                Data = default,
                ErrorMessage = errorMessage
            };
        }
    }
}
