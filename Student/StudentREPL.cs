using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentLib
{
    public class StudentREPL
    {
        private StudentAPI m_Api;

        public void Run()
        {
            //Instantiate the API and greet the user
            m_Api = new StudentAPI();
            GreetUser();

            //Process command until the user exits.
            //Handle invalid command names with a standard message.
            //Route the command to the correct command processor.
            //Handle anything unexpected with an exception message.
         
            Console.ForegroundColor = ConsoleColor.Yellow;
            while (true)
            {
                
                string line = Console.ReadLine();
                CommandParseResult parseResult = m_Api.ParseCommandName(line);

                try
                {
                    switch (parseResult.CommandName)
                    {
                        case CommandName.Exit:
                            return;
                        case CommandName.Clear:
                            Console.Clear();
                            break;
                        case CommandName.Invalid:
                            PrintInvalidCommandMessage(parseResult);
                            break;
                        default:
                            CommandProcessResult result = m_Api.ProcessCommand(parseResult);
                            Console.WriteLine(String.Join("\r\n", result.MessageLines));
                            break;
                    }
                }
                catch (Exception excp)
                {
                    PrintExceptionMessage(excp);
                }

                //No matter what, prompt for the next input.
                Console.Write("--> ");
            }
        }

        private void GreetUser()
        {
            Console.Clear();
            Console.WriteLine("Welcome to the Student REPL");
            Console.WriteLine("Enter 'help' to show a list of options");
            Console.WriteLine("Use 'open' command to start your session");
            Console.Write("--> ");
        }

        private void PrintInvalidCommandMessage(CommandParseResult parseResult)
        {
            Console.WriteLine("Invalid command '" + parseResult.CommandVerb + "'. Enter 'help' to show a list of options");
        }

        private void PrintExceptionMessage(Exception excp)
        {
            Console.WriteLine("An error occured: " + excp.Message);
        }
    }
}