﻿using CSETWebCore.Interfaces.Helpers;
using CSETWebCore.Model.Edm;
using CSETWebCore.Model.Question;
using CSETWebCore.Model.Set;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Pkcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSETWebCore.Helpers
{
    /// <summary>
    /// Manages translation lookup based on language pack
    /// information stored in App_Data/LanguagePacks
    /// </summary>
    public class TranslationOverlay
    {
        private Dictionary<string, RequirementTranslations> dReq = new Dictionary<string, RequirementTranslations>();
        private Dictionary<string, CategoryTranslation> dCat = new Dictionary<string, CategoryTranslation>();

        private Dictionary<string, GenericTranslation> dKVP = new Dictionary<string, GenericTranslation>();


        /// <summary>
        /// Generically gets a value for the specified key and collection.
        /// Collection indicates the name of the JSON file.
        /// </summary>
        public Model.Question.KeyValuePair GetValue(string collection, string key, string lang)
        {
            GenericTranslation langPack = null;

            if (lang == "en")
            {
                return null;
            }

            lang = lang.ToLower();
            collection = collection.ToLower();

            var kvpKey = $"{lang}|{collection}";

            if (!dKVP.ContainsKey(kvpKey))
            {
                var rh = new ResourceHelper();
                var json = rh.GetCopiedResource(System.IO.Path.Combine("app_data", "LanguagePacks", lang, $"{collection}.json"));

                if (json == null)
                {
                    return null;
                }

                langPack = Newtonsoft.Json.JsonConvert.DeserializeObject<GenericTranslation>(json);

                dKVP.Add(kvpKey, langPack);
            }
            else
            {
                langPack = dKVP[kvpKey];
            }

            
            return langPack.Pairs.FirstOrDefault(x => x.Key.ToLower() == key.ToLower());
        }


        /// <summary>
        /// Lazy loads the CATEGORIES language pack and tries to get 
        /// a value for the specified key.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public Model.Question.KeyValuePair GetCat(string category, string lang)
        {
            CategoryTranslation langPack = null;

            if (lang == "en")
            {
                return null;
            }

            if (!dCat.ContainsKey(lang))
            {
                var rh = new ResourceHelper();
                var json = rh.GetCopiedResource(System.IO.Path.Combine("app_data", "LanguagePacks", lang, "CATEGORIES.json"));

                langPack = Newtonsoft.Json.JsonConvert.DeserializeObject<CategoryTranslation>(json);

                dCat.Add(lang, langPack);
            }
            else
            {
                langPack = dCat[lang];
            }

            return langPack.Categories.FirstOrDefault(x => x.Key.ToLower() == category.ToLower());
        }


        /// <summary>
        /// Gets the overlay requirement object for the language.
        /// </summary>
        /// <returns></returns>
        public RequirementTranslation GetReq(int requirementId, string lang)
        {
            RequirementTranslations langPack = null;

            if (lang == "en")
            {
                return null;
            }

            if (!dReq.ContainsKey(lang))
            {
                var rh = new ResourceHelper();
                var json = rh.GetCopiedResource(System.IO.Path.Combine("app_data", "LanguagePacks", lang, "NEW_REQUIREMENT.json"));

                langPack = Newtonsoft.Json.JsonConvert.DeserializeObject<RequirementTranslations>(json);

                dReq.Add(lang, langPack);
            }
            else
            {
                langPack = dReq[lang];
            }

            return langPack.Requirements.FirstOrDefault(x => x.RequirementId == requirementId);
        }
    }
}