namespace Agent;

public static class Prompts
{
    public static string GenerateQueryWriterPrompt(DateTime currentDate, string researchTopic)
    {
        return $$"""
                 Your goal is to generate a targeted web search query.

                 <CONTEXT>
                 Current date: {{currentDate}}
                 Please ensure your queries account for the most current information available as of this date.
                 </CONTEXT>

                 <TOPIC>
                 {{researchTopic}}
                 </TOPIC>

                 <FORMAT>
                 Format your response as a JSON object with ALL three of these exact keys:
                    - "query": The actual search query string
                    - "rationale": Brief explanation of why this query is relevant
                 </FORMAT>

                 <EXAMPLE>
                 Example output:
                 {
                     "query": "machine learning transformer architecture explained",
                     "rationale": "Understanding the fundamental structure of transformer models"
                 }
                 </EXAMPLE>

                 Provide your response in JSON format:
                 """;
    }

    // ask: warum funktioniert das so gut mit den xml elementen
    public const string SummarizerPrompt =
        """
        <GOAL>
        Generate a high-quality summary of the provided context.
        </GOAL>

        <REQUIREMENTS>
        When creating a NEW summary:
        1. Highlight the most relevant information related to the user topic from the search results
        2. Ensure a coherent flow of information

        When EXTENDING an existing summary:                                                                                                                 
        1. Read the existing summary and new search results carefully.                                                    
        2. Compare the new information with the existing summary.                                                         
        3. For each piece of new information:                                                                             
            a. If it's related to existing points, integrate it into the relevant paragraph.                               
            b. If it's entirely new but relevant, add a new paragraph with a smooth transition.                            
            c. If it's not relevant to the user topic, skip it.                                                            
        4. Ensure all additions are relevant to the user's topic.                                                         
        5. Verify that your final output differs from the input summary.                                                                                                                                                            
        < /REQUIREMENTS >

        < FORMATTING >
        - Start directly with the updated summary, without preamble or titles. Do not use XML tags in the output.  
        < /FORMATTING >

        <Task>
        Think carefully about the provided Context first. Then generate a summary of the context to address the User Input.
        </Task>
        """;

    public static string GenerateReflectionPrompt(string researchTopic)
    {
        // todo: needs adaption so it guides the planning agent to some sort of fully detailed development
        
        return $$"""
               You are an expert research assistant analyzing a summary about {{researchTopic}}.

               <GOAL>
               1. Identify knowledge gaps or areas that need deeper exploration
               2. Generate a follow-up question that would help expand your understanding
               3. Focus on technical details, implementation specifics, or emerging trends that weren't fully covered
               </GOAL>

               <REQUIREMENTS>
               Ensure the follow-up question is self-contained and includes necessary context for web search.
               </REQUIREMENTS>

               <FORMAT>
               Format your response as a JSON object with these exact keys:
               - knowledge_gap: Describe what information is missing or needs clarification
               - query: Write a specific question to address this gap
               </FORMAT>

               <Task>
               Reflect carefully on the Summary to identify knowledge gaps and produce a follow-up query. Then, produce your output following this JSON format:
               {
                   "knowledge_gap": "The summary lacks information about performance metrics and benchmarks",
                   "query": "What are typical performance benchmarks and metrics used to evaluate [specific technology]?"
               }
               </Task>
               """;
    }
}   