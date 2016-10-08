using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CareerAdvice.Dialogs
{
    [Serializable]
    public sealed class CareerLuisDialog : LuisDialog<object>
    {
        public string[] info = { "education" , "interests" , "subjects" , "skills" };

        public Dictionary<string, string[]> ques = new Dictionary<string, string[]>(){
            { "education" , getedu },
            { "interests" , getinterests},
            { "subjects" , getsubjects },
            { "skills" , getskills }
        };

        static public string[] getedu = { "Could you tell me about your Education Background?",
            "So what all have you done in terms of Academics till now?"
        };
        static public string[] getinterests = { "What are your interests?",
            "What are some of the things that you like to do"
        };
        static public string[] getsubjects = { "What Subjects did you take in Academics?",
            "What all Subjects do you know?"
        };
        static public string[] getskills = { "What all are your skills?"
        };


        static public string[] greetings = {"Hi!",
            "Hello There!",
            "Hey How do you do?"
        };
        static public string[] help = {"Ask me Career Advice, Tell me about Yourself :)",
            "I give Career Help, Ask me Anything"
        };
        static public string[] endings = {"Bye!",
            "Hello There!",
            "Hey How do you do?"
        };


        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand your statement";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("education")]
        [LuisIntent("interests")]
        [LuisIntent("subjects")]
        [LuisIntent("skills")]
        public async Task Process(IDialogContext context, LuisResult result)
        {
            string[] ent = result.Entities[0].Entity;
            string intents = result.Intents[0].Intent;
            context.ConversationData.SetValue<string[]>(intents, ent);
            
            string s = getMessage(context, result);

            if (s == "")
            {
                onComplete(context, result);
            }
            else
            {
                await context.PostAsync(getRandomString(ques[s]));
                context.Wait(MessageReceived);
            }            
        }

        public string getMessage(IDialogContext context, LuisResult result) {
            //Returns the Required Intents or "" if all the Intents have been acquired
            string a = "";
            string intent = "";
            for (int i = 0; i < info.Length; i++) {
                if (!context.ConversationData.TryGetValue(info[i], out a))
                {
                    intent = info[i];
                    break;
                }
            }
            return intent;
        }

        [LuisIntent("needanswer")]
        public async Task onComplete(IDialogContext context, LuisResult result) {
            //TODO:: Get Response from ML API
            await context.PostAsync("");
            context.Wait(MessageReceived);
        }

        public string getRandomString(string[] a) {
            Random rnd = new Random();
            int idx = rnd.Next(0,a.Length);
            return a[idx];
        }
    }
}