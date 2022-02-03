using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentLib
{
    public struct CommandParseResult
    {
        public CommandName CommandName;
        public string CommandVerb;
        public string[] CommandArgs;

        public CommandParseResult(CommandName commandName, string commandVerb, string[] commandArgs)
        {
            CommandName = commandName;
            CommandVerb = commandVerb;
            CommandArgs = commandArgs;
        }
    }
}