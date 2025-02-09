using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslationManager : MonoBehaviour
{
    #region Instance
    public static TranslationManager instance { get; private set; }

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(this.gameObject);
    }
    #endregion
    public List<string> SpanishTxts, EnglishTxts;

}
