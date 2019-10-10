using System.Collections.Generic;

namespace Utilities.Enum
{
    //reference to https://gist.github.com/JT5D/a2fdfefa80124a06f5a9 as of 09/10/2019
    /// <summary>
    /// Contains supported language constants for language translator.
    /// </summary>
    public enum Language
    {
        Afrikaans,
        Akan,
        Albanian,
        Amharic,
        Arabic,
        Armenian,
        Azerbaijani,
        Basque,
        Belarusian,
        Bemba,
        Bengali,
        Bihari,
        /// <summary>
        /// What kind of language is this? doesn't seem to work tho.
        /// </summary>
        Bork_bork_bork,
        Bosnian,
        Breton,
        Bulgarian,
        Cambodian,
        Catalan,
        Cherokee,
        Chichewa,
        Chinese_Simplified,
        Chinese_Traditional,
        Corsican,
        Croatian,
        Czech,
        Danish,
        Dutch,
        Elmer_Fudd,
        English,
        Esperanto,
        Estonian,
        Ewe,
        Faroese,
        Filipino,
        Finnish,
        French,
        Frisian,
        Ga,
        Galician,
        Georgian,
        German,
        Greek,
        Guarani,
        Gujarati,
        Hacker,
        Haitian_Creole,
        Hausa,
        Hawaiian,
        Hebrew,
        Hindi,
        Hungarian,
        Icelandic,
        Igbo,
        Indonesian,
        Interlingua,
        Irish,
        Italian,
        Japanese,
        Javanese,
        Kannada,
        Kazakh,
        Kinyarwanda,
        Kirundi,
        Klingon,
        Kongo,
        Korean,
        Krio_Sierra_Leone,
        Kurdish,
        Kurdish_Soranî,
        Kyrgyz,
        Laothian,
        Latin,
        Latvian,
        Lingala,
        Lithuanian,
        Lozi,
        Luganda,
        Luo,
        Macedonian,
        Malagasy,
        Malay,
        Malayalam,
        Maltese,
        Maori,
        Marathi,
        Mauritian_Creole,
        Moldavian,
        Mongolian,
        Montenegrin,
        Nepali,
        Nigerian_Pidgin,
        Northern_Sotho,
        Norwegian,
        Norwegian_Nynorsk,
        Occitan,
        Oriya,
        Oromo,
        Pashto,
        Persian,
        Pirate,
        Polish,
        Portuguese_Brazil,
        Portuguese_Portugal,
        Punjabi,
        Quechua,
        Romanian,
        Romansh,
        Runyakitara,
        Russian,
        Scots_Gaelic,
        Serbian,
        Serbo_Croatian,
        Sesotho,
        Setswana,
        Seychellois_Creole,
        Shona,
        Sindhi,
        Sinhalese,
        Slovak,
        Slovenian,
        Somali,
        Spanish,
        Spanish_Latin_American,
        Sundanese,
        Swahili,
        Swedish,
        Tajik,
        Tamil,
        Tatar,
        Telugu,
        Thai,
        Tigrinya,
        Tonga,
        Tshiluba,
        Tumbuka,
        Turkish,
        Turkmen,
        Twi,
        Uighur,
        Ukrainian,
        Urdu,
        Uzbek,
        Vietnamese,
        Welsh,
        Wolof,
        Xhosa,
        Yiddish,
        Yoruba,
        Zulu,
    }
    /// <summary>
    /// Contains supported language constants for language translator.
    /// </summary>
    internal static class InternalPredefinedConfiguration
    {
        internal static readonly Dictionary<Language, string> _languageInternalValue = new Dictionary<Language, string>() {
            {Language.Afrikaans,"af"},
            {Language.Akan,"ak"},
            {Language.Albanian,"sq"},
            {Language.Amharic,"am"},
            {Language.Arabic,"ar"},
            {Language.Armenian,"hy"},
            {Language.Azerbaijani,"az"},
            {Language.Basque,"eu"},
            {Language.Belarusian,"be"},
            {Language.Bemba,"bem"},
            {Language.Bengali,"bn"},
            {Language.Bihari,"bh"},
            {Language.Bork_bork_bork,"xx-bork"},
            {Language.Bosnian,"bs"},
            {Language.Breton,"br"},
            {Language.Bulgarian,"bg"},
            {Language.Cambodian,"km"},
            {Language.Catalan,"ca"},
            {Language.Cherokee,"chr"},
            {Language.Chichewa,"ny"},
            {Language.Chinese_Simplified,"zh-CN"},
            {Language.Chinese_Traditional,"zh-TW"},
            {Language.Corsican,"co"},
            {Language.Croatian,"hr"},
            {Language.Czech,"cs"},
            {Language.Danish,"da"},
            {Language.Dutch,"nl"},
            {Language.Elmer_Fudd,"xx-elmer"},
            {Language.English,"en"},
            {Language.Esperanto,"eo"},
            {Language.Estonian,"et"},
            {Language.Ewe,"ee"},
            {Language.Faroese,"fo"},
            {Language.Filipino,"tl"},
            {Language.Finnish,"fi"},
            {Language.French,"fr"},
            {Language.Frisian,"fy"},
            {Language.Ga,"gaa"},
            {Language.Galician,"gl"},
            {Language.Georgian,"ka"},
            {Language.German,"de"},
            {Language.Greek,"el"},
            {Language.Guarani,"gn"},
            {Language.Gujarati,"gu"},
            {Language.Hacker,"xx-hacker"},
            {Language.Haitian_Creole,"ht"},
            {Language.Hausa,"ha"},
            {Language.Hawaiian,"haw"},
            {Language.Hebrew,"iw"},
            {Language.Hindi,"hi"},
            {Language.Hungarian,"hu"},
            {Language.Icelandic,"is"},
            {Language.Igbo,"ig"},
            {Language.Indonesian,"id"},
            {Language.Interlingua,"ia"},
            {Language.Irish,"ga"},
            {Language.Italian,"it"},
            {Language.Japanese,"ja"},
            {Language.Javanese,"jw"},
            {Language.Kannada,"kn"},
            {Language.Kazakh,"kk"},
            {Language.Kinyarwanda,"rw"},
            {Language.Kirundi,"rn"},
            {Language.Klingon,"xx-klingon"},
            {Language.Kongo,"kg"},
            {Language.Korean,"ko"},
            {Language.Krio_Sierra_Leone,"kri"},
            {Language.Kurdish,"ku"},
            {Language.Kurdish_Soranî,"ckb"},
            {Language.Kyrgyz,"ky"},
            {Language.Laothian,"lo"},
            {Language.Latin,"la"},
            {Language.Latvian,"lv"},
            {Language.Lingala,"ln"},
            {Language.Lithuanian,"lt"},
            {Language.Lozi,"loz"},
            {Language.Luganda,"lg"},
            {Language.Luo,"ach"},
            {Language.Macedonian,"mk"},
            {Language.Malagasy,"mg"},
            {Language.Malay,"ms"},
            {Language.Malayalam,"ml"},
            {Language.Maltese,"mt"},
            {Language.Maori,"mi"},
            {Language.Marathi,"mr"},
            {Language.Mauritian_Creole,"mfe"},
            {Language.Moldavian,"mo"},
            {Language.Mongolian,"mn"},
            {Language.Montenegrin,"sr-ME"},
            {Language.Nepali,"ne"},
            {Language.Nigerian_Pidgin,"pcm"},
            {Language.Northern_Sotho,"nso"},
            {Language.Norwegian,"no"},
            {Language.Norwegian_Nynorsk,"nn"},
            {Language.Occitan,"oc"},
            {Language.Oriya,"or"},
            {Language.Oromo,"om"},
            {Language.Pashto,"ps"},
            {Language.Persian,"fa"},
            {Language.Pirate,"xx-pirate"},
            {Language.Polish,"pl"},
            {Language.Portuguese_Brazil,"pt-BR"},
            {Language.Portuguese_Portugal,"pt-PT"},
            {Language.Punjabi,"pa"},
            {Language.Quechua,"qu"},
            {Language.Romanian,"ro"},
            {Language.Romansh,"rm"},
            {Language.Runyakitara,"nyn"},
            {Language.Russian,"ru"},
            {Language.Scots_Gaelic,"gd"},
            {Language.Serbian,"sr"},
            {Language.Serbo_Croatian,"sh"},
            {Language.Sesotho,"st"},
            {Language.Setswana,"tn"},
            {Language.Seychellois_Creole,"crs"},
            {Language.Shona,"sn"},
            {Language.Sindhi,"sd"},
            {Language.Sinhalese,"si"},
            {Language.Slovak,"sk"},
            {Language.Slovenian,"sl"},
            {Language.Somali,"so"},
            {Language.Spanish,"es"},
            {Language.Spanish_Latin_American,"es-419"},
            {Language.Sundanese,"su"},
            {Language.Swahili,"sw"},
            {Language.Swedish,"sv"},
            {Language.Tajik,"tg"},
            {Language.Tamil,"ta"},
            {Language.Tatar,"tt"},
            {Language.Telugu,"te"},
            {Language.Thai,"th"},
            {Language.Tigrinya,"ti"},
            {Language.Tonga,"to"},
            {Language.Tshiluba,"lua"},
            {Language.Tumbuka,"tum"},
            {Language.Turkish,"tr"},
            {Language.Turkmen,"tk"},
            {Language.Twi,"tw"},
            {Language.Uighur,"ug"},
            {Language.Ukrainian,"uk"},
            {Language.Urdu,"ur"},
            {Language.Uzbek,"uz"},
            {Language.Vietnamese,"vi"},
            {Language.Welsh,"cy"},
            {Language.Wolof,"wo"},
            {Language.Xhosa,"xh"},
            {Language.Yiddish,"yi"},
            {Language.Yoruba,"yo"},
            {Language.Zulu,"zu" }
        };
        //public const string Afrikaans = "af";
        //public const string Albanian = "sq";
        //public const string Arabic = "ar";
        //public const string Azerbaijani = "az";
        //public const string Basque = "eu";
        //public const string Bengali = "bn";
        //public const string Belarusian = "be";
        //public const string Bulgarian = "bg";
        //public const string Catalan = "ca";
        //public const string Chinese_Simplified = "zh-CN";
        //public const string Chinese_Traditional = "zh-TW";
        //public const string Croatian = "hr";
        //public const string Czech = "cs";
        //public const string Danish = "da";
        //public const string Dutch = "nl";
        //public const string English = "en";
        //public const string Esperanto = "eo";
        //public const string Estonian = "et";
        //public const string Filipino = "tl";
        //public const string Finnish = "fi";
        //public const string French = "fr";
        //public const string Galician = "gl";
        //public const string Georgian = "ka";
        //public const string German = "de";
        //public const string Greek = "el";
        //public const string Gujarati = "gu";
        //public const string Haitian_Creole = "ht";
        //public const string Hebrew = "iw";
        //public const string Hindi = "hi";
        //public const string Hungarian = "hu";
        //public const string Icelandic = "is";
        //public const string Indonesian = "id";
        //public const string Irish = "ga";
        //public const string Italian = "it";
        //public const string Japanese = "ja";
        //public const string Kannada = "kn";
        //public const string Korean = "ko";
        //public const string Latin = "la";
        //public const string Latvian = "lv";
        //public const string Lithuanian = "lt";
        //public const string Macedonian = "mk";
        //public const string Malay = "ms";
        //public const string Maltese = "mt";
        //public const string Norwegian = "no";
        //public const string Persian = "fa";
        //public const string Polish = "pl";
        //public const string Portuguese = "pt";
        //public const string Romanian = "ro";
        //public const string Russian = "ru";
        //public const string Serbian = "sr";
        //public const string Slovak = "sk";
        //public const string Slovenian = "sl";
        //public const string Spanish = "es";
        //public const string Swahili = "sw";
        //public const string Swedish = "sv";
        //public const string Tamil = "ta";
        //public const string Telugu = "te";
        //public const string Thai = "th";
        //public const string Turkish = "tr";
        //public const string Ukrainian = "uk";
        //public const string Urdu = "ur";
        //public const string Vietnamese = "vi";
        //public const string Welsh = "cy";
        //public const string Yiddish = "yi";
    }
}