using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace CareerAdvice.Dialogs
{
    [LuisModel("d45f9973-e944-4056-959e-c6f6a1978095", "8c49b30c27044a7ab894d3ee9be02f78")]
    [Serializable]
    public class CareerLuisDialog : LuisDialog<object>
    {
        public string[] greetings = {"Hi!",
                    "Hello There!",
                    "Hey How do you do?",
                    "At Your Service..."
                };

        public string[] help = {"Ask me Career Advice, Tell me about Yourself :)",
                    "I give Career Help, Ask me Anything"
                };

        public string[] endings = {"Bye!",
                    "Hello There!",
                    "Hey How do you do?"
                };

        public string[] fail = { "Sorry I could not Understand What you are Saying." };

        public string[] info = { "subject",
            "education",
            "interests"//,
            //"skills"
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

        public Dictionary<string, string[]> ques = new Dictionary<string, string[]>(){
                    { "education" , getedu },
                    { "interests" , getinterests},
                    { "subjects" , getsubjects },
                    { "skills" , getskills }
                };


        [LuisIntent("None")]
        [LuisIntent("")]
        public async Task NoneHandler(IDialogContext context, LuisResult result)
        {
            int idx = getRandomString(fail);
            await context.PostAsync(fail[idx]);
            context.Wait(MessageReceived);
        }

        [LuisIntent("greeting")]
        public async Task Greet(IDialogContext context, LuisResult result)
        {
            int idx = getRandomString(greetings);
            await context.PostAsync(greetings[idx]);
            context.Wait(MessageReceived);
        }

        [LuisIntent("end")]
        public async Task EndConv(IDialogContext context, LuisResult result)
        {
            int idx = getRandomString(endings);
            await context.PostAsync(endings[idx]);
            context.Wait(MessageReceived);
        }

        [LuisIntent("help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            int idx = getRandomString(help);
            await context.PostAsync(help[idx]);
            context.Wait(MessageReceived);
        }

        [LuisIntent("info")]
        public async Task Process(IDialogContext context, LuisResult result)
        {
            string entity_kind = "";
            string entity = "";

            for (int i = 0; i < result.Entities.Count; i++)
            {
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
                int idx = getRandomString(ques[s]);
                await context.PostAsync(ques[s][idx]);
                context.Wait(MessageReceived);
            }
        }

        public string getMessage(IDialogContext context, LuisResult result)
        {
            //Returns the Required Entities or "" if all the Entities have been acquired
            string a = "";
            string entity = "";
            for (int i = 0; i < info.Length; i++)
            {
                if (!context.ConversationData.TryGetValue(info[i], out a))
                {
                    entity = info[i];
                    break;
                }
            }
            return entity;
        }

        [LuisIntent("needanswer")]
        public async Task onComplete(IDialogContext context, LuisResult result)
        {
            //TODO::
            //-> Tockenize entities 
            //-> Form Feature Vector
            //-> Get Response from ML API

            await context.PostAsync("Looking");
            context.Wait(MessageReceived);
        }

        [LuisIntent("feedback")]
        public async Task ProcessFeedback(IDialogContext context, LuisResult result)
        {
            int res = await getSentiment(result.Query);
            if (res == 1) {
                //TODO::
                //Update SQL Table
            }
            await context.PostAsync("Thank you for your valuable Feedback");
            context.Wait(MessageReceived);

        }

        public int getRandomString(string[] a)
        {
            Random rnd = new Random();
            int idx = rnd.Next(0, a.Length);
            return idx;
        }

        static async Task<int> getSentiment(string body)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "320baec5-aa3b-473f-b491-727d22a11bdc");

            var uri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(body);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }

            dynamic res = JsonConvert.DeserializeObject(response.Content.ToString());
            int score = res.Sentiment.documents.score;

            if (score > 0.5)
                return 1;
            else return 0;
        }
    }
}