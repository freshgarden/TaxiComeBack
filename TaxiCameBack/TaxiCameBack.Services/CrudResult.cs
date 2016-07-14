using System.Collections.Generic;

namespace TaxiCameBack.Services
{
    public class CrudResult
    {
        public CrudResult()
        {
            Errors = new List<string>();
        }

        public bool Success => Errors.Count == 0;

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public IList<string> Errors { get; set; }
    }
}
