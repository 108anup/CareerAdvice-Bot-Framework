using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CareerAdvice.Dialogs
{
    public static partial class UserData
    {
        public const string Education = "education";
        public const string Interests = "interests";
        public const string Subjects = "subjects";
    }

    [Serializable]
    public sealed class CareerLuisDialog : LuisDialog<object>
    {
    }
}