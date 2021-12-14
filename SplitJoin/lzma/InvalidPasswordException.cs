namespace ManagedLzma.SevenZip
{
    /// <summary>
    /// InvalidPasswordException is thrown for invalid password provided during decryption
    /// </summary>
    [Serializable]
    public class InvalidPasswordException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the InvalidPasswordException class with a default error message.
        /// </summary>
        public InvalidPasswordException() : base("The password provided is invalid")
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidPasswordException class with a specified error message.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        public InvalidPasswordException(string message) : base(message)
        {
        }
    }
}
