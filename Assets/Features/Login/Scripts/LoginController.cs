using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text CodewordLabel;

    void Start()
    {
        string codeword = CodewordGenerator.GetSimpleCode();
        CodewordLabel.text = codeword;
    }
}
