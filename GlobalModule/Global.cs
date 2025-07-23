using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace GlobalModule
{
    // ---=== Константи ===---
    public class C_Const
    {
        //        public const string StartURL = "https://principlesyou.com/"; // Посилання на початкову сторінку   
        //        public const string SignInURL = "https://principlesyou.com/session/new?email_address=slavaua%40mail.ru&commit=Continue"; // Посилання на сторінку введення логіну
        public const string TestURL = "https://principlesyou.com/assessment"; // Посилання на сторінку з тестом.
        public const string ResultURL = "https://principlesyou.com/results"; // Посилання на сторінку з результатом

        public const char CSV_File_Separator = '\t';                                   // Розділювач в CSV файлі даних

        public const string Info_Block_Separator = "\n------------------------------\n";  // Розділювач блоків на інформаційній панелі 
        public const string Info_Іndent = "  ";

        public const string UserID_Text = "Електронна адреса";  // Текст поля ідентифікатора користувача
    }

    // ---=== Статичні методи ===---
    public static class StaticMetod
    {
        // Отримання Description з типу eNum
        public static string GetDescription(Enum aEnumElement)
        {
            Type vType = aEnumElement.GetType();
            MemberInfo[] vMemberInfo = vType.GetMember(aEnumElement.ToString());
            if (vMemberInfo != null && vMemberInfo.Length > 0)
            {
                object[] vAttrs = vMemberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (vAttrs != null && vAttrs.Length > 0)
                    return ((DescriptionAttribute)vAttrs[0]).Description;
            }
            return aEnumElement.ToString();
        }

    }

    #region ---=== Структури даних ===---

    //  ---=== ДАНІ Користувача  ===---
    public struct UserData
    {
        public string UserID;                                   // Ідентифікатор користувача
        public Dictionary<string, AnswerQuestion> UserAnswers;  // Відповіді на запитання
        public String UserArchetype;                               // Результат тесту (назва основного АРХЕТИПУ)

        //КОНСТРУКТОР
        public UserData(string aUserID, Dictionary<string, AnswerQuestion> aUserAnswers, String aUserArchetype)
        {
            UserID = aUserID;                           // Ідентифікатор користувача
            UserAnswers = aUserAnswers;                 // Відповіді на запитання
            if (!String.IsNullOrEmpty(aUserArchetype))
                UserArchetype = aUserArchetype;         // Результат тесту  (назва основного АРХЕТИПУ)
            else
                UserArchetype = "";
        }
    }

    // МОЖЛИВІ ВІДПОВІДІ КОРИСТУВАЧА
    public enum AnswerQuestion
    {
        [Description("Disagree strongly")]              // Категорично не згоден
        Disagree_strongly,
        [Description("Disagree")]                       // Не згоден
        Disagree,
        [Description("Disagree slightly")]              // Трохи не згоден
        Disagree_slightly,
        [Description("Neither agree nor disagree")]     // Ні погоджуюсь, ні не погоджуюсь
        Neither,
        [Description("Agree slightly")]                 // Трохи згоден
        Agree_slightly,
        [Description("Agree")]                          // Згоден
        Agree,
        [Description("Agree strongly")]                 // Повністю згоден
        Agree_strongly,
        [Description("Err")]                            // Помилковий параметр
        Err,
    }

    // ---=== WB ===---
    // Визначаєм СТАН БРОУЗЕРА
    public enum WBState // Стан Веб Броузера
    {
        [Description("Невизначено.")]
        stPage_Unknown,
        [Description("Форма Тестування без введених даних.(з ВІДКЛЮЧЕНОЮ кнопкою continue.)")]
        stPage_Test_Clear,
        [Description("Форма Тестування з введеними даними (з АКТИВНОЮ кнопкою continue.)")]
        stPage_Test_Сontinue,
        [Description("Форма Тестування завершено - кнопка (Переглянути результат/ View Results)")]
        stPage_Test_End,
        [Description("Сторінка з результатом для збереження")]
        stPage_Result,
    }

    // Вирази для пошуку елементів на WEB сторінках
    public enum XPathSelect
    {
        [Description(".//div[@id='page']")]                                         // Форма тесту - сторінка з id
        FT_Page_ID,
        [Description(".//h2[contains(@id, 'TopTextQuestion')]")]                    // Форма тесту - ЗАПИТАННЯ
        FT_Questions,
        [Description(".//button[contains(@rel, 'continue') and  @disabled='']")]    // Форма тесту - кнопка продовжити ВІДКЛЮЧЕНА
        FT_ButtonContinue_Disable,
        [Description(".//button[contains(@rel, 'continue')]")]                      // Форма тесту - кнопка продовжити АКТИВНА
        FT_ButtonContinue_Enable,
        [Description(".//button[@id='skipToResults']")]                             // Форма тесту (ЗАВЕРШЕННЯ) - кнопка (Переглянути результат/ View Results)
        FT_Button_ViewResults,
        [Description(".//*[@id='Archetype']")]                                      // Сторінка з результатом для збереження
        Page_Result,
        [Description(".//*[@id='Archetype']/div/div[5]/div[1]/div[2]/h2/span")]     // Блок з текстом результату 
        Block_Archetypes_Result,


    }

    // Результат (Архетипи)
    public class Archetypes
    {
        public Dictionary<string, string> _Types;
        public Archetypes()
        {
            _Types = new Dictionary<string, string>
            {
                // ---=== Leaders === ---
                { "The Commander", "https://principlesyou.com/archetypes/commander" },
                { "The Shaper", "https://principlesyou.com/archetypes/shaper" },
                { "The Quiet Leader", "https://principlesyou.com/archetypes/quietleader" },

                //-- -=== Advocates === ---
                { "The Inspirer", "https://principlesyou.com/archetypes/inspirer" },
                { "The Campaigner", "https://principlesyou.com/archetypes/campaigner" },
                { "The Coach", "https://principlesyou.com/archetypes/coach" },

                //---=== Enthusiasts === ---
                { "The Promoter", "https://principlesyou.com/archetypes/promoter" },
                { "The Impresario", "https://principlesyou.com/archetypes/impresario" },
                { "The Entertainer", "https://principlesyou.com/archetypes/entertainer" },

                //---=== Givers === ---
                { "The Peacekeeper", "https://principlesyou.com/archetypes/peacekeeper" },
                { "The Problem Solver", "https://principlesyou.com/archetypes/problemsolver" },
                { "The Helper", "https://principlesyou.com/archetypes/helper" },

                //---=== Architects === ---
                { "The Strategist", "https://principlesyou.com/archetypes/strategist" },
                { "The Planner", "https://principlesyou.com/archetypes/planner" },
                { "The Orchestrator", "https://principlesyou.com/archetypes/orchestrator" },

                //---=== Producers === ---
                { "The Implementer", "https://principlesyou.com/archetypes/implementer" },
                { "The Investigator", "https://principlesyou.com/archetypes/investigator" },
                { "The Technician", "https://principlesyou.com/archetypes/technician" },

                //---=== Creators === ---
                { "The Adventurer", "https://principlesyou.com/archetypes/adventurer" },
                { "The Artisan", "https://principlesyou.com/archetypes/artisan" },
                { "The Inventor", "https://principlesyou.com/archetypes/inventor" },

                //---=== Seekers === ---
                { "The Explorer", "https://principlesyou.com/archetypes/explorer" },
                { "The Thinker", "https://principlesyou.com/archetypes/thinker" },
                { "The Growth Seeker", "https://principlesyou.com/archetypes/growthseeker" },

                //---=== Fighters === ---
                { "The Protector", "https://principlesyou.com/archetypes/protector" },
                { "The Enforcer", "https://principlesyou.com/archetypes/enforcer" },
                { "The Critic", "https://principlesyou.com/archetypes/critic" },

                //---=== === === ---
                { "The Individualist", "https://principlesyou.com/archetypes/individualist" }
            };

        }

    }
    #endregion ---=== Структури даних ===---

}
