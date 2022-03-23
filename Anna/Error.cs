using System;

namespace Anna
{
    public class Error
    {
        #region Private Variables

        private readonly string _errorName;
        private readonly string _errorDescription;
        private readonly int _code;

        #endregion Private Variables

        #region Properties

        public string ErrorName => _errorName;

        public string ErrorDescription => _errorDescription;

        public int Code => _code;

        #endregion Properties

        #region Constructor

        public Error(string errorName, string errorDescription)
        {
            _errorDescription = errorDescription;
            _errorName = errorName;
        }

        public Error(string errorName, string errorDescription, int code)
        {
            _errorDescription = errorDescription;
            _errorName = errorName;
            _code = code;
        }

        public Error(string errorDescription) : this(string.Empty, errorDescription)
        {

        }

        #endregion

        #region Public Methods

        public static explicit operator Error(Exception e)
        {
            return new Error(e.GetMergedErrors(), e.StackTrace);
        }

        public override string ToString()
        {
            return _errorName + " " + _errorDescription;
        }

        #endregion
    }
}