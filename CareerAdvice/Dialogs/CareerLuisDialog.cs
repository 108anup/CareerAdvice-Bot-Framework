using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CareerAdvice.Dialogs
{
    [LuisModel("4cffb2d7-dde5-4ae3-b931-4738e052ea46", "fa4319fe7a164a828174f45550b872b3")]
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

        static public string[] getedu = { "Could you tell me about your Educational Background?",
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
            "Hey How do you do?",
            "At Your Service..."
        };
        static public string[] help = {"Ask me Career Advice, Tell me about Yourself :)",
            "I give Career Help, Ask me Anything"
        };
        static public string[] endings = {"Bye!",
            "Hello There!",
            "Hey How do you do?"
        };
        static public string[] fail = { "Sorry I could not Understand What you are Saying." };


        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(getRandomString(fail));
            context.Wait(MessageReceived);
        }

        [LuisIntent("greeting")]
        public async Task Greet(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(getRandomString(greetings));
            context.Wait(MessageReceived);
        }

        [LuisIntent("end")]
        public async Task EndConv(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(getRandomString(endings));
            context.Wait(MessageReceived);
        }

        [LuisIntent("info")]
        public async Task Process(IDialogContext context, LuisResult result)
        {
            string entity_kind = "";
            string entity = "";

            for (int i = 0; i < result.Entities.Count; i++) {
                entity_kind = result.Entities[i].Type;
                entity = result.Entities[i].Entity;
                context.ConversationData.SetValue<string>(entity_kind, entity);
            }
                        
            string s = getMessage(context, result);

            if (s == "")
            {
                await onComplete(context, result);
            }
            else
            {
                await context.PostAsync(getRandomString(ques[s]));
                context.Wait(MessageReceived);
            }            
        }

        [LuisIntent("needanswer")]
        public async Task onComplete(IDialogContext context, LuisResult result)
        {
            //TODO::
            //-> Tockenize entities 
            //-> Form Feature Vector
            //-> Get Response from ML API

            await context.PostAsync("");
            context.Wait(MessageReceived);
        }

        [LuisIntent("feedback")]
        public void ProcessFeedback(IDialogContext context, LuisResult result)
        {
            //TODO:: Process Feedback
        }

        public string getMessage(IDialogContext context, LuisResult result) {
            //Returns the Required Entities or "" if all the Entities have been acquired
            string a = "";
            string entity = "";
            for (int i = 0; i < info.Length; i++) {
                if (!context.ConversationData.TryGetValue(info[i], out a))
                {
                    entity = info[i];
                    break;
                }
            }
            return entity;
        }

        public string getRandomString(string[] a) {
            Random rnd = new Random();
            int idx = rnd.Next(0,a.Length);
            return a[idx];
        }
    }
}