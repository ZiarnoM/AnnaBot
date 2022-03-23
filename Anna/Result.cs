using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Anna
{
    public class Result
    {
        #region Private Variables

        private bool _operationResult;
        private Collection<Error> _Errors;

        #endregion Private Variables

        #region Properties

        public bool IsSuccessful => _operationResult;

        public Collection<Error> Errors => _Errors;

        #endregion Properties

        #region Constructor

        public Result(bool operationResult = true)
        {
            _operationResult = operationResult;
            _Errors = new Collection<Error>();
        }

        public Result(IEnumerable<Error> errors)
        {
            _operationResult = false;
            _Errors = new Collection<Error>(errors.ToList());
        }

        public Result(params Error[] errors) : this(errors.AsEnumerable())
        {
        }


        public Result(Error error)
        {
            _operationResult = false;
            _Errors = new Collection<Error>();
            _Errors.Add(error);
        }

        public Result(string errorName, string errorDescription) : this(new Error(errorName, errorDescription))
        {

        }

        public Result(string errorDescription) : this(new Error(errorDescription))
        {

        }

        #endregion Constructor

        #region Public Methods

        public void AddError(Error error)
        {
            _Errors.Add(error);
        }

        public void AddError(params Error[] errors)
        {
            _Errors = new Collection<Error>(_Errors.Concat(errors).ToList());
        }

        public override string ToString()
        {
            string errors = string.Empty;
            foreach (var item in Errors)
            {
                errors += item.ToString() + " ";
            }
            return errors;
        }

        #endregion
    }

    public class Result<TResult> : Result
    {
        private TResult _result;

        /// <summary>
        /// Zawartośc wyniku operacji
        /// </summary>
        public TResult GetResult => _result;

        #region Constructor

        public Result(bool operationResult = false) : base(operationResult)
        {
        }

        public Result(Collection<Error> errors) : base(errors)
        {
        }

        public Result(Error error) : base(error)
        {
        }

        public Result(TResult result, bool operationResult = true) : base(operationResult)
        {
            _result = result;
        }

        public Result(string errorName, string errorDescription) : base(errorName, errorDescription)
        {

        }

        public Result(string errorDescription) : base(errorDescription)
        {

        }

        #endregion Constructor
    }
}
