using StudentLib.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentLib
{
    public enum CommandName
    {
        Invalid,
        Exit,
        Open,
        List,
        Select,
        Help,
        Clear,
        Show,
        Modify,
        Delete,
        Filter

    }

    public class StudentAPI
    {
        private Dictionary<CommandName, ICommandProcessor> m_CommandProcessors;
        private List<Student> m_FilteredStudents;
        private Student m_CurrentStudent;
        private StudentDataManager m_DataManager;

        public List<Student> FilteredStudents => m_FilteredStudents;
        public Student CurrentStudent => m_CurrentStudent;

        public StudentAPI()
        {
            m_DataManager = new();
            m_CommandProcessors = new();
            m_CommandProcessors.Add(CommandName.Help, new HelpCommandProcessor(m_CommandProcessors));
            m_CommandProcessors.Add(CommandName.Open, new OpenCommandProcessor(m_DataManager));
            m_CommandProcessors.Add(CommandName.List, new ListCommandProcessor(m_DataManager));
            m_CommandProcessors.Add(CommandName.Select, new SelectCommandProcessor());
            m_CommandProcessors.Add(CommandName.Show, new ShowCommandProcessor());
            m_CommandProcessors.Add(CommandName.Modify, new ModifyCommandProcessor());
            m_CommandProcessors.Add(CommandName.Delete, new DeleteCommandProcessor());
            m_CommandProcessors.Add(CommandName.Filter, new FilterCommandProcessor());



        }

        public CommandParseResult ParseCommandName(string inputLine)
        {
            string[] parts = inputLine.Split(' ', 2);     
            string commandVerb = parts[0].ToLower();
            string[] args = null;
            if (parts.Length > 1)
                args = parts[1].Split(' ');

            switch (commandVerb)
            {
                case "exit": return new CommandParseResult(CommandName.Exit, commandVerb, args);
                case "open": return new CommandParseResult(CommandName.Open, commandVerb, args);
                case "list": return new CommandParseResult(CommandName.List, commandVerb, args);
                case "help": return new CommandParseResult(CommandName.Help, commandVerb, args);
                case "clear": return new CommandParseResult(CommandName.Clear, commandVerb, args);
                case "select": return new CommandParseResult(CommandName.Select, commandVerb, args);
                case "show": return new CommandParseResult(CommandName.Show, commandVerb, args);
                case "modify": return new CommandParseResult(CommandName.Modify, commandVerb, args);
                case "delete": return new CommandParseResult(CommandName.Delete, commandVerb, args);
                case "filter": return new CommandParseResult(CommandName.Filter, commandVerb, args);
                default: return new CommandParseResult(CommandName.Invalid, commandVerb, args);
            }
        }

        public CommandProcessResult ProcessCommand(CommandParseResult parseResult)
        {
            return m_CommandProcessors[parseResult.CommandName].Execute(parseResult, ref m_FilteredStudents, ref m_CurrentStudent);
        }

        #region HelpCommandProcessor
        public class HelpCommandProcessor : ICommandProcessor
        {
            private Dictionary<CommandName, ICommandProcessor> m_CommandProcessors;
            public string CommandVerb => "help";
            public string HelpText => "Print this help text";

            public HelpCommandProcessor(Dictionary<CommandName, ICommandProcessor> commandProcessors)
            {
                m_CommandProcessors = commandProcessors;
            }

            public CommandProcessResult Execute(CommandParseResult parseResult, ref List<Student> filteredStudents, ref Student currentStudent)
            {
                List<string> messageLines = new();
                messageLines.Add("clear".PadRight(12) + "Clear the terminal screen");
                messageLines.Add("exit".PadRight(12) + "Terminate the Student REPL");
                foreach (ICommandProcessor processor in m_CommandProcessors.Values)
                    messageLines.Add(processor.CommandVerb.PadRight(12) + processor.HelpText);
                return new() { MessageLines = messageLines.ToArray() };
            }
        }
        #endregion

        #region OpenCommandProcessor
        public class OpenCommandProcessor : ICommandProcessor
        {
            private StudentDataManager m_DataManager;

            public string CommandVerb => "open";
            public string HelpText => "Open a list of students";

            public OpenCommandProcessor(StudentDataManager dataManager)
            {
                m_DataManager = dataManager;
            }

            public CommandProcessResult Execute(CommandParseResult parseResult, ref List<Student> filteredStudents, ref Student currentStudent)
            {
                List<string> messageLines = new();

                //TODO: Handle the number of students and seed arguments

                int numStudents = 20;
                int seed = 4;
                filteredStudents = m_DataManager.Open(numStudents, seed);
                messageLines.Add("List of " + numStudents + " students opened with seed " + seed);
                return new() { MessageLines = messageLines.ToArray() };
            }
        }
        #endregion

        #region ListCommandProcessor
        public class ListCommandProcessor : ICommandProcessor
        {
            private StudentDataManager m_DataManager;

            public string CommandVerb => "list";
            public string HelpText => "Show ids and names for the current filtered student list";

            public ListCommandProcessor(StudentDataManager dataManager)
            {
                m_DataManager = dataManager;
            }

            public CommandProcessResult Execute(CommandParseResult parseResult, ref List<Student> filteredStudents, ref Student currentStudent)
            {
                List<string> messageLines = new();

                if (filteredStudents == null)
                    messageLines.Add("\nNo student list selected. Use 'open' or 'filter' to create filtered list");
                else if (filteredStudents.Count == 0)
                    messageLines.Add("\nFiltered student list is empty");
                else
                {
                    messageLines.Add("---------------------------------------------------------------");
                    messageLines.Add(("Id").PadRight(10) + "Name".PadRight(30) +"# of Students: "+filteredStudents.Count);
                    messageLines.Add("---------------------------------------------------------------");

                    foreach (Student student in filteredStudents)
                    {
                      
                        messageLines.Add(student.Id.ToString().PadRight(10) + student.FirstName + " " + student.LastName);
                    }
                    
                }

                return new() { MessageLines = messageLines.ToArray() };
            }
        }
        #endregion

        #region SelectCommandProcessor
        public class SelectCommandProcessor : ICommandProcessor
        {
            public string CommandVerb => "select";
            public string HelpText => "Select a student from the filtered student list by Id";
       
            public CommandProcessResult Execute(CommandParseResult parseResult, ref List<Student> filteredStudents, ref Student currentStudent)
            {
                List<string> messageLines = new();
               
                if (parseResult.CommandArgs == null || parseResult.CommandArgs.Length == 0)
                    messageLines.Add("\nPlease provide the id of the student you want to select");
                else if (!Int32.TryParse(parseResult.CommandArgs[0], out int id))
                    messageLines.Add("\nStudent id must be a 6-digit number");
                else
                {
                    currentStudent = filteredStudents.FirstOrDefault(s => s.Id == id);
                    if (currentStudent == null)
                        messageLines.Add("\nStudent with id " + id + " does not exist or is not in filtered student list");
                    else
                        messageLines.Add("\nCurrent student is now " + currentStudent.Id + " " + currentStudent.FirstName + " " + currentStudent.LastName);
                }
                return new() { MessageLines = messageLines.ToArray() };
            }
        }
        #endregion

        #region ShowCommandProcessor
        public class ShowCommandProcessor : ICommandProcessor
        {
            public string CommandVerb => "show";
            public string HelpText => "After selecting a student, enter 'show' to show more details.";


            public CommandProcessResult Execute(CommandParseResult parseResult, ref List<Student> filteredStudents, ref Student currentStudent)
            {
                List<string> messageLines = new();

                if (parseResult.CommandArgs == null || parseResult.CommandArgs.Length == 0)
                    if (currentStudent == null)
                        messageLines.Add("\nNo student has been selected.");
                    else
                        messageLines.Add("\n"+currentStudent.ToString());

                return new() { MessageLines = messageLines.ToArray() };
            }
        }
        #endregion

        #region ModifyCommandProcessor
        public class ModifyCommandProcessor : ICommandProcessor
        {
            public string CommandVerb => "select";
            public string HelpText => "Select a student from the filtered student list by Id";

            public ModifyCommandProcessor()
            {

            }

            public CommandProcessResult Execute(CommandParseResult parseResult, ref List<Student> filteredStudents, ref Student currentStudent)
            {
                List<string> messageLines = new();

                if (parseResult.CommandArgs == null || parseResult.CommandArgs.Length == 0)
                    messageLines.Add("\nPlease provide the id of the student you want to select");
                else if (!Int32.TryParse(parseResult.CommandArgs[0], out int id))
                    messageLines.Add("\nStudent id must be a 6-digit number");
                else
                {
                    currentStudent = filteredStudents.FirstOrDefault(s => s.Id == id);
                    if (currentStudent == null)
                        messageLines.Add("\nStudent with id " + id + " does not exist or is not in filtered student list");
                    else
                        messageLines.Add("\nCurrent student is now " + currentStudent.Id + " " + currentStudent.FirstName + " " + currentStudent.LastName);
                }

                return new() { MessageLines = messageLines.ToArray() };
            }
        }
        #endregion

        #region DeleteCommandProcessor
        public class DeleteCommandProcessor : ICommandProcessor
        {
            public string CommandVerb => "delete";
            public string HelpText => "Select a student from the filtered student list by Id to be DELETED";

            public CommandProcessResult Execute(CommandParseResult parseResult, ref List<Student> filteredStudents, ref Student currentStudent)
            {
                List<string> messageLines = new();

                    if (currentStudent == null)
                        messageLines.Add("\nStudent with id " + currentStudent.Id + " does not exist or is not in filtered student list");
                    else
                        filteredStudents.Remove(currentStudent);
                        messageLines.Add("\nStudent " + currentStudent.FirstName + " " + currentStudent.LastName + " has been deleted.");
           

                return new() { MessageLines = messageLines.ToArray() };
            }
        }
        #endregion

        #region FilterCommandProcessor
        public class FilterCommandProcessor : ICommandProcessor
        {
            public string CommandVerb => "filter";
            public string HelpText => "Filter the list of students by their name, birthdate, GPA, credits completed or major.";

            public CommandProcessResult Execute(CommandParseResult parseResult, ref List<Student> filteredStudents, ref Student currentStudent)
            {
                List<string> messageLines = new();
                if (parseResult.CommandArgs == null || parseResult.CommandArgs.Length == 0)
                {
                    messageLines.Add("\nPlease provide attribute you wish to filter by: ");
                    messageLines.Add("  filter firstName  --> Filter the list of students by the letter of their first name.");
                    messageLines.Add("  filter lastName  --> Filter the list of students by the letter of their last name.");
                    messageLines.Add("  filter birthDate  --> Filter the list of students by their birthdate.");
                    messageLines.Add("  filter gpa  --> Filter the list of students, given a GPA range.");
                    messageLines.Add("  filter completedCredits  --> Filter the list of students by their Credits Completed given a range.");
                    messageLines.Add("  filter major  --> Filter the list of students by their major.\n");
                }
                else
                {
                    if (parseResult.CommandArgs[parseResult.CommandArgs.Length-1].Contains("firstName"))
                    {
                        messageLines.Add("Filter the list by the student's first name starting with that letter.");
                        messageLines.Add("Command:  filter firstName {?} ");
                        messageLines.Add("Example:   filter firstName A ");
                    }
                    else if (parseResult.CommandArgs[parseResult.CommandArgs.Length - 1].Contains("lastName"))
                    {
                        messageLines.Add("Filter the list of students by the letter of their last name.");
                        messageLines.Add("Command:  filter lastName {?} ");
                        messageLines.Add("Example:   filter lastName A ");
                    }
                    else if (parseResult.CommandArgs[parseResult.CommandArgs.Length - 1].Contains("birthDate"))
                    {
                        messageLines.Add("Filter the list of students by their birth year, month or day.");
                        messageLines.Add("Command:  filter birthDate {t} {?}");
                        messageLines.Add("Example:   filter birthDate yyyy 1993");
                        messageLines.Add("Example:   filter birthDate mm 03");
                        messageLines.Add("Example:   filter birthDate dd 01");
                    }
                    else if (parseResult.CommandArgs[parseResult.CommandArgs.Length - 1].Contains("gpa"))
                    {
                        messageLines.Add("Filter the list of students with GPA's that are within a range.");
                        messageLines.Add("Command:  filter gpa {h} {l}");
                        messageLines.Add("Example:  filter gpa 2.4 3.5");
                    }
                    else if (parseResult.CommandArgs[parseResult.CommandArgs.Length - 1].Contains("completedCredits"))
                    {
                        messageLines.Add("\nFilter the list of students with a minimum number of completed credits.");
                        messageLines.Add("Command:  filter completedCredits {?}");
                        messageLines.Add("Example:   filter completedCredits 58");
                    }
                    else if (parseResult.CommandArgs[parseResult.CommandArgs.Length - 1].Contains("major"))
                    {
                        List<string> majors = new List<string>();
                        foreach(Student s in filteredStudents)
                        {
                            if (!majors.Contains(s.Major))
                            {
                                majors.Add(s.Major);
                            }                         
                        }
                        messageLines.Add("Filter the list of students by on of the following majors.");
                        foreach(string str in majors)
                        {
                            messageLines.Add("filter major "+str);
                        }
                    }

                    else if (parseResult.CommandArgs[parseResult.CommandArgs.Length - 2].Contains("firstName"))
                    {
                        List<Student> filterByName = new List<Student>();
                        string letterFilter = parseResult.CommandArgs[parseResult.CommandArgs.Length - 1].ToUpper();

                        foreach (Student s in filteredStudents)
                        {
                            if (s.FirstName.StartsWith(letterFilter))
                            {
                                filterByName.Add(s);
                            }
                        }
                        if (filterByName.Count == 0)
                        {
                            messageLines.Add("No student's first names start with the letter " + letterFilter + ".");
                        }
                        else
                        {
                            messageLines.Add("---------------------------------------------------------------");
                            messageLines.Add(("Id").PadRight(10) + "Name".PadRight(30) + "# of Students: " + filterByName.Count);
                            messageLines.Add("---------------------------------------------------------------");
                            foreach (Student s in filterByName)
                            {
                                messageLines.Add(s.Id.ToString().PadRight(10) + s.FirstName + " " + s.LastName);
                            }
                        }
                    }
                    else if(parseResult.CommandArgs[parseResult.CommandArgs.Length - 2].Contains("lastName"))
                    {
                        List<Student> filterByName = new List<Student>();
                        string letterFilter = parseResult.CommandArgs[parseResult.CommandArgs.Length - 1].ToUpper();

                        foreach (Student s in filteredStudents)
                        {
                            if (s.LastName.StartsWith(letterFilter))
                            {
                                filterByName.Add(s);
                            }
                        }
                        if (filterByName.Count == 0)
                        {
                            messageLines.Add("No student's last names start with the letter " + letterFilter + ".");
                        }
                        else
                        {
                            messageLines.Add("---------------------------------------------------------------");
                            messageLines.Add(("Id").PadRight(10) + "Name".PadRight(30) + "# of Students: " + filterByName.Count);
                            messageLines.Add("---------------------------------------------------------------");
                            foreach (Student s in filterByName)
                            {
                                messageLines.Add(s.Id.ToString().PadRight(10) + s.FirstName + " " + s.LastName);
                            }
                        }
                    }
                    else if (parseResult.CommandArgs[parseResult.CommandArgs.Length - 3].Contains("birthDate"))
                    {
                        List<Student> filterByName = new List<Student>();

                        if (parseResult.CommandArgs[parseResult.CommandArgs.Length - 2].Contains("yyyy"))
                        {
                            int yearFilter = int.Parse(parseResult.CommandArgs[parseResult.CommandArgs.Length - 1]);
                            messageLines.Add("Filtering by year " + yearFilter + "...");
                            foreach (Student s in filteredStudents)
                            {
                                if (s.BirthDate.Year == yearFilter)
                                {
                                    filterByName.Add(s);
                                }
                            }
                        }
                        else if (parseResult.CommandArgs[parseResult.CommandArgs.Length - 2].Contains("mm"))
                        {
                            int monthFilter = int.Parse(parseResult.CommandArgs[parseResult.CommandArgs.Length - 1]);
                            messageLines.Add("Filtering by month " + monthFilter + "...");
                            foreach (Student s in filteredStudents)
                            {
                                if (s.BirthDate.Month == monthFilter)
                                {
                                    filterByName.Add(s);
                                }
                            }
                        }
                        else if (parseResult.CommandArgs[parseResult.CommandArgs.Length - 2].Contains("dd"))
                        {
                            int dayFilter = int.Parse(parseResult.CommandArgs[parseResult.CommandArgs.Length - 1]);
                            messageLines.Add("Filtering by day " + dayFilter + "...");
                            foreach (Student s in filteredStudents)
                            {
                                if (s.BirthDate.Day == dayFilter)
                                {
                                    filterByName.Add(s);
                                }
                            }
                        }

                        if (filterByName.Count == 0)
                        {
                            messageLines.Add("No student's have a birthdate matching your criteria.");
                        }
                        else
                        {
                            messageLines.Add("-----------------------------------------------------------------------------------");
                            messageLines.Add(("Id").PadRight(10) + "Birthdate".PadRight(15) + "Name".PadRight(30) + "# of Students: " + filterByName.Count);
                            messageLines.Add("-----------------------------------------------------------------------------------");
                            foreach (Student s in filterByName)
                            {
                                messageLines.Add(s.Id.ToString().PadRight(10) + s.BirthDate.ToShortDateString().PadRight(15) + s.FirstName + " " + s.LastName);
                            }
                        }
                    
                }
                    else if (parseResult.CommandArgs[parseResult.CommandArgs.Length - 3].Contains("gpa"))
                    {
                        double min = double.Parse(parseResult.CommandArgs[parseResult.CommandArgs.Length - 2]);
                        double max = double.Parse(parseResult.CommandArgs[parseResult.CommandArgs.Length - 1]);
                        List<Student> filteredByGPA = new List<Student>();
                        foreach(Student s in filteredStudents)
                        {
                            if(s.GPA>=min && s.GPA <= max)


                            {
                                filteredByGPA.Add(s);
                            }
                        }
                        if (filteredByGPA.Count == 0)
                        {
                            messageLines.Add("\nNo students have a GPA within the range " + min+" - "+max+".");
                        }
                        else
                        {
                            foreach(Student s in filteredByGPA)
                            {
                                messageLines.Add("-----------------------------------------------------------------------------------");
                                messageLines.Add(("Id").PadRight(10) + "GPA".PadRight(10) + "Name".PadRight(30) + "# of Students: " + filteredByGPA.Count);
                                messageLines.Add("-----------------------------------------------------------------------------------");
                                messageLines.Add(s.Id.ToString().PadRight(10) + s.GPA.ToString().PadRight(10)+ s.FirstName+ " " + s.LastName );
                            }
                        }
                    }
                    else if (parseResult.CommandArgs[parseResult.CommandArgs.Length - 2].Contains("creditsCompleted"))
                    {

                    }
                    else if (parseResult.CommandArgs[parseResult.CommandArgs.Length - 2].Contains("major"))
                    {

                    }
                }                   
             
                return new() { MessageLines = messageLines.ToArray() };
            }
        }
        #endregion





    }
}