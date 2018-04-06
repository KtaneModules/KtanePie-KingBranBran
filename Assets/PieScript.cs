using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PieScript : MonoBehaviour {

    string pi = "31415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679821480865132823066470938446095505822317253594081284811174502841027019385211055596446229489549303819644288109756659334461284756482337867831652712019091456485669234603486104543266482133936072602491412737245870066063155881748815209209628292540917153643678925903600113305305488204665213841469519415116094330572703657595919530921861173819326117931051185480744623799627495673518857527248912279381830119491";
    public KMBombModule Pie;
    public KMAudio Audio;
    public KMSelectable[] buttons = new KMSelectable[5];
    public TextMesh[] buttonTexts = new TextMesh[5];
    public string[] soundList = { "C", "D", "E", "F", "G" };
    string[] codes;
    int[] intCodes;
    int codePlace;
    int[] answer;
    int codesNum;
    int sumOfDigits;
    int stage = 1;
    bool bombSolved = false;
    private static int _moduleIdCounter = 1;
    private int _moduleId;
    private int[] buttonOrder;
    int[] pressedButtons = { 0, 0, 0, 0, 0 };


    void Start () {
        _moduleId = _moduleIdCounter++;
        codes = PickNumber();
        intCodes = new int[] { int.Parse(codes[0]), int.Parse(codes[0]), int.Parse(codes[0]), int.Parse(codes[0]), int.Parse(codes[0]) };
        codesNum = int.Parse(codes[0] + codes[1] + codes[2] + codes[3] + codes[4]);
        answer = CalculateAnswer();
        buttonOrder = CalculateOrder(answer);
        DebugLog("Display is {0} {1} {2} {3} {4}", codes[0], codes[1], codes[2], codes[3], codes[4]);
        DebugLog("Correct button order: {0} {1} {2} {3} {4}", buttonOrder[0], buttonOrder[1], buttonOrder[2], buttonOrder[3], buttonOrder[4]);

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].OnInteract += ButtonPressed(i);
        }

        Pie.OnActivate += Activate();

        
	}

    int[] CalculateOrder( int[] list )
    {
        int[] order = { 0, 0, 0, 0, 0 };

        for (int i = 0; i < order.Length; i++)
        {
            order[i] = FindInList(list, i + 1) + 1;
        }
        DebugLog("Position in pi: {0}", order);
        return order;
    }

    int FindInList(int[] list, int num)
    {
        int number = 0;
        bool found = false;

        for (int i = 0; !found; i++)
        {
            if (list[i] == num)
            {
                number = i;
                found = true;
            }
        }
        return number;
    }

     KMBombModule.KMModuleActivateEvent Activate()
    {
        return delegate
        {
            SetDisplay(codes);
        };
    }

    private KMSelectable.OnInteractHandler ButtonPressed(int number)
    {
        return delegate
        {
            if (bombSolved)
                return false;

            Audio.PlaySoundAtTransform(soundList[stage - 1], Pie.transform);

            if (answer[number] == stage)
            {
                stage++;
                buttonTexts[number].color = Color.gray;
                // pressedButtons[stage - 1] = number;
            }
            else
            {
                // StartCoroutine(ResetColors(true));
                Pie.HandleStrike();
                DebugLog("Strike! Pressed {0}, expected {1}", number, buttonOrder[stage]);
            }

            if (stage == 6)
            {
                
                StartCoroutine(ResetColors(false));
                StartCoroutine(PlaySolveSound()); // Will also handle pass.
                bombSolved = true;
            }
            return false;
        };

    }

    IEnumerator ResetColors(bool strike)
    {
        // Note: Strike is not in use.
        if (strike)
        {
            for (int i = 0; i < buttonTexts.Length; i++) {
                if (pressedButtons.All(x => x != i));
                    buttonTexts[i].color = Color.red;
            }
            yield return new WaitForSeconds(.5f);
            for (int i = 0; i < buttonTexts.Length; i++)
            {
                if (pressedButtons.All(x => x != i));
                buttonTexts[i].color = Color.green;
            }
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            foreach (TextMesh buttonText in buttonTexts)
            {
                buttonText.color = Color.green;            
            }
            yield return new WaitForSeconds(.2f);
            foreach (TextMesh buttonText in buttonTexts)
            {
                buttonText.color = Color.gray;
            }
            yield return new WaitForSeconds(.2f);
            foreach (TextMesh buttonText in buttonTexts)
            {
                buttonText.color = Color.green;
            }
            yield return new WaitForSeconds(.2f);
            foreach (TextMesh buttonText in buttonTexts)
            {
                buttonText.color = Color.gray;
            }
            yield return new WaitForSeconds(.2f);
            foreach (TextMesh buttonText in buttonTexts)
            {
                buttonText.color = Color.green;
            }
            SetDisplay(new[] { "", "", "π", "", "" });
        }
       
    }

    IEnumerator PlaySolveSound()
    {
        yield return new WaitForSeconds(.5f);
        Audio.PlaySoundAtTransform("G", Pie.transform);
        yield return new WaitForSeconds(.2f);
        Audio.PlaySoundAtTransform("F", Pie.transform);
        yield return new WaitForSeconds(.2f);
        Audio.PlaySoundAtTransform("D", Pie.transform);
        yield return new WaitForSeconds(.2f);
        Audio.PlaySoundAtTransform("E", Pie.transform);
        yield return new WaitForSeconds(.2f);
        Audio.PlaySoundAtTransform("C", Pie.transform);
        Pie.HandlePass();
        DebugLog("Module passed!");
    }

    
    int[] CalculateAnswer()
    {
        int[] solution = { 0, 0, 0, 0, 0 };
        int calculation = codesNum;
        sumOfDigits = codesNum.ToString().Sum(x => int.Parse(x.ToString()));
        calculation += codePlace;
        calculation %= 100;
        solution = GetOrder(calculation);
        return solution;
    }

    int[] GetOrder(int number)
    {
        int[] order = { 0, 0, 0, 0, 0 };

        // 1st number => If the number is prime.
        order[0] = (new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97 }.Any(x => number == x)) ? 1 : 5;

        // 2nd number => if the sum of digits and the number are both even or both odd.
        if ((sumOfDigits % 2 == 0 && number % 2 == 0) || (sumOfDigits % 2 == 1 && number % 2 == 1))
        {
            order[1] = order[0] == 1 ? 2 : 1;
        }
        else
        {
            order[1] = order[0] == 5 ? 4 : 5;
        }

        // 3rd number => If the number is divisible by 3
        if (number % 3 == 0)
        {
            for (int i = 1; order[2] == 0; i++)
            {
                if (order[0] != i && order[1] != i) //Keeps checking high priorities until it finds an empty spot.
                {
                    order[2] = i;
                }
            }
        }
        else
        {
            for (int i = 5; order[2] == 0; i--)
            {
                if (order[0] != i && order[1] != i) //Keeps checking low priorities until it finds an empty spot.
                {
                    order[2] = i;
                }
            }
        }

        // 4th number => If the number is divisible by the least signaficant digit of the sum of the 5 integers on the module. (Wow I am evil)
        int digitsModuloTen = sumOfDigits % 10;
        bool numThree = false;
        if (digitsModuloTen != 0)
        {
            if (number % digitsModuloTen == 0)
            {
                numThree = true;
            }
        }

        if (numThree) 
        {
            for (int i = 1; order[3] == 0; i++)
            {
                if (order[0] != i && order[1] != i && order[2] != i) //Keeps checking high priorities until it finds an empty spot.
                {
                    order[3] = i;
                }
            }
        }
        else
        {
            for (int i = 5; order[3] == 0; i--)
            {
                if (order[0] != i && order[1] != i && order[2] != i) //Keeps checking low priorities until it finds an empty spot.
                {
                    order[3] = i;
                }
            }
        }

        // 5th number (and easiest ;D)
        for (int i = 1; order[4] == 0; i++)
        {
            if (order[0] != i && order[1] != i && order[2] != i && order[3] != i) //Keeps checking high priorities until it finds an empty spot.
            {
                order[4] = i;
            }
        }

        return order;
    }

    string[] PickNumber()
    {
        int i = Random.Range(0, 496);
        codePlace = i + 1; // Keep track of place
        
        char[] piCharArray = pi.ToCharArray();
        string[] stringCharArray = piCharArray.Select(x => x.ToString()).ToArray();
        string[] code = { stringCharArray[i], stringCharArray[i + 1], stringCharArray[i + 2], stringCharArray[i + 3], stringCharArray[i + 4] };
        return code;
    }

    void SetDisplay(string[] text)
    {
        if (text.Length != 5)
            return;
        for (int i = 0; i < 5; i++)
        {
            buttonTexts[i].text = text[i];
        }
    }

    private void DebugLog(string log, params object[] args)
    {
        var logData = string.Format(log, args);
        Debug.LogFormat("[Pie #{0}] {1}", _moduleId, logData);
    }

    private string TwitchHelpMessage = @"Use !{0} 1 4 5 3 2 to press the buttons in a certain order.";

    private static string[] supportedTwitchCommands = new[] { "press", "click", "submit" };

    IEnumerator ProcessTwitchCommand(string command)
    {
        var parts = command.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        var skip = 0;
        if (parts.Length > 0 && supportedTwitchCommands.Contains(parts[0]))
            skip = 1;

        if (parts.Length > skip && parts.Skip(skip).All(part => part.Length == 1 && "12345".Contains(part)))
        {
            yield return null;

            for (int i = skip; i < parts.Length; i++)
            {
                int num = Int32.Parse(parts[i]);
                buttons[num - 1].OnInteract();
                yield return new WaitForSeconds(.2f);
            }
        }
    }
}

