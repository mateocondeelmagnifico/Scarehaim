using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class TranslateStrings : MonoBehaviour
{
    public Event onLenguajeChanged;
    private StringTable table;

    private void Awake()
    {
        table = LocalizationSettings.StringDatabase.GetTable("Lenguajes Table");
    }

    public List<string> TranslateTexts(List<string> strings)
    {
        List <string> translatedStrings = new List<string>();

        //Busca un string que sea igual 
        //Para que funcione el titulo del string tiene que ser igual que el de la entry

        for(int i = 0; i < strings.Count; i++)
        {
            for(int j = 0; j < table.Count; j++)
            {
                if (strings[i] == table.SharedData.Entries[j].Key)
                {
                    translatedStrings.Add(table.SharedData.Entries[j].ToString());
                    break;
                }
            }
        }

        return translatedStrings;
    }
}
