using System;
using System.Collections.Generic;
using System.Text;

namespace MedPark.Common
{
    public class QueryResult
    {
        public string FailureReason { get; }
        public bool IsSuccess => String.IsNullOrEmpty(FailureReason);
        public object Data { get; }

        private QueryResult()
        {

        }

        private QueryResult(string failureReason)
        {
            FailureReason = failureReason;
        }

        private QueryResult(object resultData)
        {
            Data = resultData;
        }


        public static QueryResult QueryFail(string reason)
        {
            return new QueryResult(reason);
        }

        public static QueryResult QuerySuccess(object resultData)
        {
            return new QueryResult(resultData);
        }

        public static implicit operator bool(QueryResult cr)
        {
            return cr.IsSuccess;
        }
    }
}
