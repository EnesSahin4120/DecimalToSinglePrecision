using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SinglePrecisionConverter : MonoBehaviour
{
    private string sign;
    private string exponent;
    private string mantissa;

    [SerializeField] private InputField decimalField;
    [SerializeField] private Text _sign;
    [SerializeField] private Text _binaryForm;
    [SerializeField] private Text _normalisedBinary;
    [SerializeField] private Text _mantissa;
    [SerializeField] private Text _unbiasedExponent;
    [SerializeField] private Text _biasedExponent;
    [SerializeField] private Text _singlePrecision;

    [SerializeField] private List<Text> allOutputs = new List<Text>();

    public void Show_Info()
    {
        SetEmpty_AllOutputs();
        SinglePrecision(decimalField.text);
    }

    private void SetEmpty_AllOutputs()
    {
        for(int i = 0; i < allOutputs.Count; i++)
        {
            allOutputs[i].text = "";
        }
    }

    private string SinglePrecision(string targetString)
    {      
        sign = Sign(targetString);
        _sign.text = "Sign : " + sign;

        string binaryForm = Decimal_to_Binary(targetString);
        _binaryForm.text = "Binary : " + binaryForm;

        if (binaryForm == "0.0"||binaryForm=="-0.0")
        {
            string resultString= sign + "0000000000000000000000000000000";
            _singlePrecision.text = "Single Precision : " + resultString;

            return resultString;
        }
     
        mantissa = Mantissa(binaryForm);
        _normalisedBinary.text = "Normalise Binary : " + mantissa;

        mantissa = Remove_Leading1_from_Mantissa(mantissa);
        _mantissa.text = "Mantissa : " + mantissa;

        int unbiased_Exponent = Unbiased_Exponent(binaryForm);
        _unbiasedExponent.text = "Unbiased Exponent : " + unbiased_Exponent;

        exponent = BiasedExponent(unbiased_Exponent);
        if (String_Prev_toComma(binaryForm, '.') == "0" || String_Prev_toComma(binaryForm, '.') == "-0")
        {
            exponent = exponent.Insert(0, "0");
        }
        _biasedExponent.text = "Exponent : " + exponent;

        string SinglePrecision = "";
        SinglePrecision += sign += exponent += mantissa;

        int charCount = 0;
        foreach (char c in SinglePrecision)
        {
            charCount++;
        }
        if (charCount < 32)
        {
            for (int i = 0; i < 32 - charCount; i++)
            {
                SinglePrecision += "0";
            }
        }
        else
        {
            for (int i = 0; i < charCount - 32 ; i++)
            {
                SinglePrecision = SinglePrecision.Remove(SinglePrecision.Length - 1, 1);
            }
        }
        _singlePrecision.text = "Single Precision : " + SinglePrecision;

        return SinglePrecision;
    }

    private int IndexOfTarget_from_String(string target, string mainString)
    {
        int targetIndex;
        bool isContent = mainString.Contains(target);

        if (isContent)
            targetIndex = mainString.IndexOf(target);
        else
            targetIndex = -10;

        return targetIndex;
    }

    private string Remove_Leading1_from_Mantissa(string targetString)
    {
        targetString = targetString.Remove(0, 2);

        return targetString;
    }

    private string BiasedExponent(int _unbiasedTarget)
    {
        int decimalRadixInt = _unbiasedTarget + 127;
        string decimalRadixString = decimalRadixInt.ToString();

        string biased = Int_To_Binary(decimalRadixString);

        return biased;
    }

    private string Mantissa(string targetString)
    {
        bool isMines = targetString.Contains("-");
        if (isMines)
        {
            int minesContent_Index = IndexOfTarget_from_String("-", targetString);
            if (minesContent_Index == 0)
                targetString = targetString.Remove(0, 1);
        }

        //Find index what contains numerical one
        int target_Numerical_Index = IndexOfTarget_from_String("1", targetString);

        //Find string personal count
        int charCount = 0;
        foreach (char c in targetString)
        {
            charCount++;
        }

        //Find mantissa
        string prev_Comma_String = "1.";
        string next_Comma_String = "";
        for (int i = 0; i < charCount - 1 - target_Numerical_Index; i++)
        {
            int addIndex;
            addIndex = target_Numerical_Index + i + 1;

            next_Comma_String += targetString[addIndex];
        }

        int pointIndex = IndexOfTarget_from_String(".", next_Comma_String);
        if (pointIndex >= 0)
            next_Comma_String = next_Comma_String.Remove(pointIndex, 1);

        string resultString = "";
        resultString += prev_Comma_String += next_Comma_String;

        return resultString;
    }

    private int Unbiased_Exponent(string targetString)
    {
        int target_Numerical_Index = IndexOfTarget_from_String("1", targetString);
        int targetPointIndex = IndexOfTarget_from_String(".", targetString);

        int _exponent;
        if (target_Numerical_Index > targetPointIndex)
        {
            _exponent = targetPointIndex - target_Numerical_Index;
        }
        else
        {
            _exponent = targetPointIndex - target_Numerical_Index - 1;
        }

        return _exponent;
    }

    private string Int_To_Binary(string targetString)
    {
        string mathSign;
        string signNumerical = Sign(targetString);

        if (signNumerical == "1")
        {
           targetString = targetString.Remove(0, 1);
           mathSign = "-";
        }
        else
        { 
           mathSign = "";
        }
    
        if (targetString == "0"||targetString=="-0" || targetString == "-")
        {
            return "0";
        }

        int targetInt;
        int.TryParse(targetString, out targetInt);

        string resultString = "";
        while (targetInt > 0)
        {
            int remainder;
            remainder = targetInt % 2;

            resultString += remainder.ToString();

            targetInt = Mathf.FloorToInt(targetInt / 2);    
        }

        return mathSign + ReverseString(resultString);
    }

    private string ReverseString(string target)
    {
        char[] chars = target.ToCharArray();
        for (int i = 0, j = target.Length - 1; i < j; i++, j--)
        {
            char c = chars[i];
            chars[i] = chars[j];
            chars[j] = c;
        }
        return new string(chars);
    }

    private string Fractional_to_Binary(string targetString)
    {
        string new_Next_comma_String;
        new_Next_comma_String = String_Next_toComma(targetString, '.');

        string zero_for_prevComma = "0";

        string newNumString;
        newNumString = string.Format("{0},{1}", zero_for_prevComma , new_Next_comma_String);
        float current;
        float.TryParse(newNumString, out current);

        int counter = 0;
        string resultString = "";
        while (counter<23)
        {
            current *= 2;
            if (current == 1)
            {
                resultString += "1";
                break;
            }

            string answerString;
            answerString = current.ToString();

            string answerPrevCommaString;
            answerPrevCommaString = String_Prev_toComma(answerString, ',');

            string answerNextCommaString;
            answerNextCommaString = String_Next_toComma(answerString, ',');

            resultString += answerPrevCommaString;

            string newCurrentString;
            newCurrentString = string.Format("{0},{1}", zero_for_prevComma , answerNextCommaString);
          
            float.TryParse(newCurrentString, out current);

            counter++;
        }

        return resultString;
    }

    private string Decimal_to_Binary(string targetString)
    {
        bool haveComma;
        haveComma = targetString.Contains(".");

        string target_Prev_to_comma_String;
        string target_Next_to_comma_String; 
        if (haveComma)
        {
            target_Next_to_comma_String = String_Next_toComma(targetString, '.');
            target_Prev_to_comma_String = String_Prev_toComma(targetString, '.');

            //--------------Specific values of zero to be entered by the user--------------

            if (target_Next_to_comma_String == "0"|| string.IsNullOrEmpty(target_Next_to_comma_String))
                target_Next_to_comma_String = "0";
            else
                target_Next_to_comma_String = Fractional_to_Binary(targetString);

            if (string.IsNullOrEmpty(target_Prev_to_comma_String)||target_Prev_to_comma_String=="0"||target_Prev_to_comma_String=="+" || target_Prev_to_comma_String == "+0")
                target_Prev_to_comma_String = "0";
            else if(target_Prev_to_comma_String=="-0"|| target_Prev_to_comma_String == "-")
                target_Prev_to_comma_String = "-0";
            else
                target_Prev_to_comma_String = Int_To_Binary(String_Prev_toComma(targetString, '.'));
        }
      
        else
        {
            if(targetString=="-"|| targetString == "-0")
            {
                target_Prev_to_comma_String = "-0";
                target_Next_to_comma_String = "0";
            }
            else if(targetString == "+" || targetString == "+0" || targetString == "0")
            {
                target_Prev_to_comma_String = "0";
                target_Next_to_comma_String = "0";
            }
            else
            {
                target_Prev_to_comma_String = Int_To_Binary(targetString);
                target_Next_to_comma_String = "0";
            }
        }

        string resultString;
        resultString = string.Format("{0}.{1}", target_Prev_to_comma_String, target_Next_to_comma_String);
        return resultString;
    }

    private string String_Prev_toComma(string targetString,char separatingChar)
    {
        string[] separateStrings;
        separateStrings = targetString.Split(separatingChar);
        return separateStrings[0];
    }

    private string String_Next_toComma(string targetString,char separatingChar)
    {
        string[] separateStrings;
        separateStrings = targetString.Split(separatingChar);
        return separateStrings[1];
    }

    private string Sign(string targetString)
    {
        bool isFloat;
        isFloat = targetString.Contains(".");

        if (isFloat)
        {
            string target_Prev_toComma = String_Prev_toComma(targetString, '.');

            if (target_Prev_toComma == "-"|| target_Prev_toComma == "-0")
                return "1";
        }
        else
        {
            if (targetString == "-" || targetString == "-0")
                return "1";
        }

        float targetFloat;
        float.TryParse(targetString, out targetFloat);

        int sign;
        if (targetFloat < 0)
            sign = 1;
        else
            sign = 0;

        return sign.ToString();
    }
}
