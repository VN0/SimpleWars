using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class I18NText : MonoBehaviour
{
    static System.EventHandler<SystemLanguageArg> _SetLanguageAll;
    static SystemLanguage _currentLanguage = SystemLanguage.Unknown;
    public static SystemLanguage currentLanguage { get { return _currentLanguage; } }

    [System.Serializable]
    public class TextPair
    {
        public SystemLanguage language = SystemLanguage.English;
        public string text;
    }
    public class SystemLanguageArg : System.EventArgs
    {
        public SystemLanguage language;
        public SystemLanguage defaultLang;
        public SystemLanguageArg (SystemLanguage language, SystemLanguage defaultLang = SystemLanguage.Unknown)
        {
            this.language = language;
            this.defaultLang = defaultLang;
        }
    }

    public IDictionary<SystemLanguage, string> languageDict = new Dictionary<SystemLanguage, string>();
    public bool autoSetLanguage = true;
    public List<TextPair> texts;
    Text txt;

    void Awake ()
    {
        _SetLanguageAll += delegate (object sender, SystemLanguageArg arg)
        {
            SetLanguage(arg.language, arg.defaultLang);
        };
        txt = GetComponent<Text>();
        foreach (TextPair pair in texts)
        {
            languageDict[pair.language] = pair.text.Replace(@"\n", "\n").Replace(@"\t", "\t");
        }
        if (_currentLanguage != SystemLanguage.Unknown)
        {
            SetLanguage(_currentLanguage);
        }
        else if (autoSetLanguage)
        {
            SetLanguage(Application.systemLanguage);
        }
    }

    /// <summary>
    /// Set language for all I18NText.
    /// </summary>
    /// <param name="lang">Language to set.</param>
    /// <param name="defaultLang">Language to use if the language doe not exist.</param>
    public static void SetLanguageAll (SystemLanguage lang, SystemLanguage defaultLang = SystemLanguage.Unknown)
    {
        _SetLanguageAll(null, new SystemLanguageArg(lang, defaultLang));
        _currentLanguage = lang;
    }

    /// <summary>
    /// Set language.
    /// </summary>
    /// <param name="lang">Language to set.</param>
    /// <param name="defaultLang">Language to use if the language doe not exist.</param>
    public void SetLanguage (SystemLanguage lang, SystemLanguage defaultLang = SystemLanguage.Unknown)
    {
        try
        {
            txt.text = languageDict[lang];
        }
        catch (KeyNotFoundException)
        {
            if (defaultLang != SystemLanguage.Unknown)
            {
                txt.text = languageDict[defaultLang];
            }
        }
    }
}
