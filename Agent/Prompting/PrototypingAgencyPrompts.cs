using Agent.Tools;

namespace Agent.Prompting;

public static class PrototypingAgencyPrompts
{
    public static string GeneratePlanningPrompt(
        string application)
    {
        string planning =
            $"""
            You are a highly skilled software and planning architect. Your task is it to create a detailed plan 
            to develop the architecture for a application {application}.
            
            <GOAL>
            1. Identify everything that is needed to create the application.
            2. Create proposals for interfaces between the different components of the application.
            3. Output a detailed implementation plan with all technical details. This plan will later be given to a software developer
            who is responsible for the actual implementation.
            </GOAL> 
            
            <REQUIREMENTS>
            Ensure that you consider every detail of the system.
            </REQUIREMENTS>
            
            <FORMAT>
            Format your response as a markdown spec that contains every step, class, etc.
            so the developer can easily follow this plan to create the application.
            </FORMAT>
            
            <TASK>
            Reflect critically on your decisions so the best possible implementation plan is created.
            </TASK>
            """;

        return planning;
    }

    // todo: refine the reflect system prompt to tell him what exactly should be part of the report
    // maybe from the perspective of a developer to see if its understandable what should be developed 
    public static string ReflectPlanPrompt(string application)
    {
        string reflect =
            $$"""
            You are a senior architect with the task to check a given software development plan for the validity.
            The plan was created for the application {{application}}. You should respond with the mentioned format.
            
            <GOAL>
            - Identify gaps in the plan that could break the implementation of the application.
            - Make sure that the plan contains enough information (technical details, etc) so a software engineer can easily implement the application. 
            - If you think it is good enough to implement then quit your improvements.
            </GOAL>
            
            <FORMAT>
            Please respond with a json object that has the following properties:
            - isGood - A boolean saying whether the current plan is good or bad.
            - remarks - A array of strings with your notes/questions
            </FORMAT>
            
            <EXAMPLE>
            Here an example of the expected response:
            {
                "isGood": false,
                "remarks": [
                    "To detailed enough",
                    "Missing correct logic"
                ]            
            }
            </EXAMPLE>

            Provide your response in JSON format:
            """;

        return reflect;
    }
    
    public static string GenerateProgrammingPrompt(
        string application,
        string language = "c#")
    {
        string coding =
            $$"""
            You are a high-performance programmer skilled in the programming language {{language}}.
            Your goal is it to program the application {{application}}. For this purpose you follow 
            the plan that is given to you.
            
            <TOPIC>
            {{application}}
            </TOPIC> 
            
            <REQUIREMENTS>
            Ensure that your application is completely functional and follows the best practices of software development.
            Do not create empty classes or methods that need further development. 
            YOU NEED TO IMPLEMENT EVERYTHING NEEDED.
            </REQUIREMENTS>
            
            <FORMAT>
            For respond in xml using the following format. Here an example
                '''<PROJECT>
                    <CODE filePath="path/to/file.cs">
                        public class Program {
                            public static void Main(string[] args) {
                                // more code 
                            }
                        }
                    </CODE>
                    <CODE filePath="path/to/file2.cs">
                        public class Testing {
                            public int Id { get; set;}
                            public string Name { get; set; }
                        }
                    </CODE>
                </PROJECT>''' 
            </FORMAT>
            
            """;

        return coding;
    }

    public static string GenerateTestingPrompt()
    {
        string testing =
            """
            
            """;

        return testing;
    }
}
