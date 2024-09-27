using Booski.Enums;
using Booski.I18n;

namespace Booski.Helpers;

public interface II18nHelpers
{
    string GetLangForLanguage(Language language);
    Language GetLanguageForLang(string lang);
    string GetPhrase(Phrase phrase, params string[]? replacements);
    string GetPhrase(Phrase phrase, Language? language = null, params string[]? replacements);
    string GetUnitSuffix(int amount, Unit unit);
    void SetDefaultLanguage(Language language);
    void SetDefaultLanguage(string lang);
}

internal sealed class I18nHelpers : II18nHelpers
{
    private Language DefaultLanguage = Language.En;
    private readonly Language FallbackLanguage = Language.En;
    private readonly string FallbackText = "(?)";

    public string GetLangForLanguage(Language language)
    {
        return language.ToString().ToLower(); // bit hacky but it works
    }

    public Language GetLanguageForLang(string lang)
    {
        return lang[..2] switch
        {
            "de" => Language.De,
            "en" => Language.En,
            "es" => Language.Es,
            "fi" => Language.Fi,
            "fr" => Language.Fr,
            "ja" => Language.Ja,
            "nl" => Language.Nl,
            "pt" => Language.Pt,
            "ru" => Language.Ru,
            _ => Language.En
        };
    }

    public string GetPhrase(
        Phrase phrase,
        params string[]? replacements
    )
    {
        return GetPhrase(
            phrase: phrase,
            language: null,
            replacements: replacements
        );
    }

    public string GetPhrase(
        Phrase phrase, 
        Language? language = null,
        params string[]? replacements
    ) {
        if(phrase == Phrase.Empty)
            return "";

        if(language == null)
            language = DefaultLanguage;

        var phrases = GetPhrases((Language)language);
        string? output = "";
        bool found;

        if(phrases.ContainsKey(phrase)) {
            phrases.TryGetValue(phrase, out output);
            found = true;
        } else {
            var defaultPhrases = GetPhrases((Language)language);
            if(defaultPhrases.ContainsKey(phrase)) {
                defaultPhrases.TryGetValue(phrase, out output);
            }
            found = true;
        }

        if(!found) {
            throw new System.Exception($"Phrase ({language}: {phrase}) not found.");
        } else {
            if(replacements != null) {
                int replacementIndex = 0;

                if(output != null)
                    foreach(var replacement in replacements) {
                        output = output.Replace($"[{replacementIndex}]", replacements[replacementIndex]);
                        replacementIndex++;
                    }
            }

            if(!String.IsNullOrEmpty(output))
                return output;
            else if(String.IsNullOrEmpty(output) && language == FallbackLanguage)
                return FallbackText;
            else
                return GetPhrase(phrase, FallbackLanguage, replacements);
        }
    }

    public string GetUnitSuffix(int amount, Unit unit)
    {
        Phrase? phrase;

        if(amount == 1)
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            phrase = unit switch
            {
                Unit.Hour => Phrase.Unit_Hour_Single,
                Unit.Second => Phrase.Unit_Second_Single
            };
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
        else
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            phrase = unit switch
            {
                Unit.Hour => Phrase.Unit_Hour_Multiple,
                Unit.Second => Phrase.Unit_Second_Multiple
            };
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

        if (phrase != null)
            return GetPhrase((Phrase)phrase);
        else
            return FallbackText;
    }

    public void SetDefaultLanguage(Language language) {
        DefaultLanguage = language;
    }

    public void SetDefaultLanguage(string lang) {
        DefaultLanguage = GetLanguageForLang(lang);
    }

    private Dictionary<Phrase, string> GetPhrases(Language language) {        
        return language switch
        {
            Language.De => De.Strings,
            Language.En => En.Strings,
            Language.Es => Es.Strings,
            Language.Fi => Fi.Strings,
            Language.Fr => Fr.Strings,
            Language.Ja => Ja.Strings,
            Language.Nl => Nl.Strings,
            Language.Pt => Pt.Strings,
            Language.Ru => Ru.Strings,
            _ => GetPhrases(DefaultLanguage)
        };
    }
}