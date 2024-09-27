// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Infrastructure.Utils;

namespace Infrastructure.Entities
{
    public class user
    {
        [Key] public string email { get; set; }
        public string name { get; set; }
        public string school_id { get; set; }
        
        //Zermos related
        public string custom_huiswerk { get; set; }
        
        //Zermelo Related
        public string zermelo_access_token { get; set; }
        public DateTime? zermelo_access_token_expires_at { get; set; }
        public string zermelo_school_abbr { get; set; }
        //public List<custom_appointment> custom_appointments { get; set; } = new();
        
        //Somtoday Related
        public string somtoday_access_token { get; set; }
        public string somtoday_refresh_token { get; set; }
        public string somtoday_student_id { get; set; }
        public string cached_somtoday_grades { get; set; }
        public string cached_somtoday_homework { get; set; }
        public string cached_somtoday_leermiddelen { get; set; }
        public string cached_somtoday_plaatsingen { get; set; }
        
        //Infowijs Related
        public string infowijs_access_token { get; set; }
        public string custom_leermiddelen { get; set; }
        public string cached_infowijs_news { get; set; }
        
        //Settings
        public string settings { get; set; }
    
        [NotMapped, Setting("light", "dark", "red", "blue", "pink")] 
        public string theme
        {
            get => GetSetting<string>("theme");
            set => SetSetting("theme", value);
        }
    
        [NotMapped, Setting("/Zermelo/Rooster", "/Somtoday/Cijfers", "/Somtoday/Huiswerk", "/Infowijs/Schoolnieuws", "/Infowijs/Schoolkalender", "/School/Informatiebord")] 
        public string default_page
        {
            get => GetSetting<string>("default_page");
            set => SetSetting("default_page", value);
        }
    
        [NotMapped, Setting("left", "right")] 
        public string hand_side
        {
            get => GetSetting<string>("hand_side");
            set => SetSetting("hand_side", value);
        }
        
        [NotMapped, Setting("(\\d\\.?)+|DEV") , Requestable] 
        public string version_used
        {
            get => GetSetting<string>("version_used");
            set => SetSetting("version_used", value);
        }

        [NotMapped, Setting("font-scale-1", "font-scale-2", "font-scale-3", "font-scale-4", "font-scale-5")]
        public string font_size
        {
            get => GetSetting<string>("font_size");
            set => SetSetting("font_size", value);
        }
        
        [NotMapped, Setting("euclid", "open-sans", "monospace", "roboto", "inter", "dyslexic")]
        public string prefers_font
        {
            get => GetSetting<string>("prefers_font");
            set => SetSetting("prefers_font", value);
        }
        
        
        //ZERMELO SETTINGS
        [NotMapped, Setting("\\d{2}:\\d{2}-\\d{2}:\\d{2}")] 
        public string zermelo_timestamps 
        {
            get => GetSetting<string>("zermelo_timestamps");
            set => SetSetting("zermelo_timestamps", value);
        }
        
        //1 -> 60*24
        [NotMapped, Setting("1440|(14[0-3]\\d)|(1[0-3]\\d{2})|(\\d{1,3})")]
        public string tropen_rooster_time
        {
            get => GetSetting<string>("tropen_rooster_time");
            set => SetSetting("tropen_rooster_time", value);
        }
        
        //ROOSTER SETTINGS
        [NotMapped, Setting("zermelo", "somtoday")] 
        public string prefered_rooster_engine
        {
            get => GetSetting<string>("prefered_rooster_engine");
            set => SetSetting("prefered_rooster_engine", value);
        }
        
        //CODE VERIFIER REGEX
        [NotMapped, Setting("^[A-Za-z0-9-._~]{43,128}$")]
        public string somtoday_code_verifier
        {
            get => GetSetting<string>("somtoday_code_verifier");
            set => SetSetting("somtoday_code_verifier", value);
        }
        
        // Helper method to get a setting value do with <int> or <string> etc.
        private T GetSetting<T>(string settingName)
        {
            if (settings == null) return default;

            var settingsArray = settings.Split(';');
            foreach (var setting in settingsArray)
            {
                var keyValue = setting.Split('=');
                if (keyValue.Length == 2 && keyValue[0].Trim() == settingName)
                {
                    return (T) Convert.ChangeType(keyValue[1].Trim(), typeof(T));
                }
            }

            return default;
        }


        // Helper method to set a setting value
        private void SetSetting(string settingName, string settingValue)
        {
            if (settings == null) settings = "";
            
            if (settingName.Contains("=") || settingName.Contains(";")) throw new ArgumentException("Setting name cannot contain '=' or ';'.");
            if (settingValue.Contains("=") || settingValue.Contains(";")) throw new ArgumentException("Setting value cannot contain '=' or ';'.");

            var property = GetType().GetProperty(settingName);
            if (property != null)
            {
                if (property.GetCustomAttributes(typeof(SettingAttribute), false)
                        .FirstOrDefault() is SettingAttribute settingAttr && !settingAttr.IsValid(settingValue))
                {
                    if (settingAttr.Options.Length == 1)
                        throw new ArgumentException($"Invalid value '{settingValue}' for setting '{settingName}'. Must match pattern '{settingAttr.Options[0]}'");
                    throw new ArgumentException($"Invalid value '{settingValue}' for setting '{settingName}'. Possible values: {string.Join(", ", settingAttr.Options)}");
                }
            }

            var settingsArray = settings.Split(';');
            var newSettings = new List<string>();

            bool updated = false;
            foreach (var setting in settingsArray)
            {
                var keyValue = setting.Split('=');
                if (keyValue.Length == 2 && keyValue[0].Trim() == settingName)
                {
                    newSettings.Add($"{settingName}={settingValue}");
                    updated = true;
                }
                else
                {
                    newSettings.Add(setting);
                }
            }

            if (!updated)
            {
                newSettings.Add($"{settingName}={settingValue}");
            }

            settings = string.Join(";", newSettings);
        }

        
        //set the ! operator for user to check if the user.email is null
        public static bool operator !(user user)
        {
            return user.email == null;
        }
    }
}