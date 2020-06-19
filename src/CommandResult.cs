using System;
using System.Collections.Generic;
using System.Text;

namespace MedPark.Common
{
    public class CommandResult
    {
        public string FailureReason { get; }
        public bool IsSuccess => String.IsNullOrEmpty(FailureReason);

        private CommandResult()
        {

        }

        private CommandResult(string failureReason)
        {
            FailureReason = failureReason;
        }



        public static CommandResult Success { get; } = new CommandResult();

        public static CommandResult Fail(string reason)
        {
            return new CommandResult(reason);
        }

        public static implicit operator bool(CommandResult cr)
        {
            return cr.IsSuccess;
        }
    }
}
