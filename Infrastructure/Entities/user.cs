// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        
        //Token Related
        public string zermelo_access_token { get; set; }
        public DateTime? zermelo_access_token_expires_at { get; set; }
        public string zermelo_school_abbr { get; set; }
        
        public string somtoday_access_token { get; set; }
        public string somtoday_refresh_token { get; set; }
        public string somtoday_student_id { get; set; }
        public string infowijs_access_token { get; set; }

        //cache related
        public string cached_somtoday_grades { get; set; }
        public string cached_somtoday_homework { get; set; }
        public string cached_somtoday_leermiddelen { get; set; }
        public string custom_leermiddelen { get; set; }
        public string cached_infowijs_news { get; set; }
        
        //Settings
        public string settings { get; set; }
    
        [NotMapped, Setting] 
        public string theme
        {
            get => GetSetting<string>("theme");
            set => SetSetting("theme", value);
        }
    
        [NotMapped, Setting] 
        public string default_page
        {
            get => GetSetting<string>("default_page");
            set => SetSetting("default_page", value);
        }
    
        [NotMapped, Setting] 
        public string hand_side
        {
            get => GetSetting<string>("hand_side");
            set => SetSetting("hand_side", value);
        }
        
        [NotMapped, Setting, Requestable] 
        public string version_used
        {
            get => GetSetting<string>("version_used");
            set => SetSetting("version_used", value);
        }

        [NotMapped, Setting] 
        public string font_size
        {
            get => GetSetting<string>("font_size");
            set => SetSetting("font_size", value);
        }
        
        // Helper method to get a setting value do with <int> or <string> etc.
        private T GetSetting<T>(string settingName)
        {
            if (settings == null)
            {
                return default;
            }

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
        private void SetSetting(string settingName, object settingValue)
        {
            if (settings == null)
            {
                settings = "";
            }

            var settingsArray = settings.Split(';');
            var newSettings = new List<string>();

            // Update or add the setting value
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