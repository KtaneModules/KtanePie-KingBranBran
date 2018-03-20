using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PieScript : MonoBehaviour {

    string pi = "141592653589793238462643383279502884197169399375105820974944592307816406286208998628034825342117067982148086513282306647093844609550582231725359408128481117450284102701938521105559644622948954930381964428810975665933446128475648233786783165271201909145648566923460348610454326648213393607260249141273724587006606315588174881520920962829254091715364367892590360011330530548820466521384146951941511609433057270365759591953092186117381932611793105118548074462379962749567351885752724891227938183011949129833673362440656643086021394946395224737190702179860943702770539217176293176752384674818467669405132000568127145263560827785771342757789609173637178721468440901224953430146549585371050792279689258923542019956112129021960864034418159813629774771309960518707211349999998372978049951059731732816096318595024459455346908302642522308253344685035261931188171010003137838752886587533208381420617177669147303598253490428755468731159562863882353787593751957781857780532171226806613001927876611195909216420198";
    public KMBombModule Pie;
    public KMAudio Audio;
    public KMSelectable[] buttons = new KMSelectable[5];
    public TextMesh[] buttonTexts = new TextMesh[5];
    public string[] soundList = { "C", "D", "E", "F", "G" };
    string[] codes;
    int codePlace;
    int[] answer;
    int codesNum;
    int sumOfDigits;
    int stage = 1;
    bool bombSolved = false;
    

    void Start () {
        codes = PickNumber();
        codesNum = int.Parse(codes[0] + codes[1] + codes[2] + codes[3] + codes[4]);
        answer = CalculateAnswer();
        

        Pie.OnActivate += Activate();

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].OnInteract += ButtonPressed(i);
        }
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
            if (!bombSolved)
                return false;

            Audio.PlaySoundAtTransform(soundList[stage - 1], Pie.transform);

            if (answer[number] == stage)
            {
                stage++;
                buttonTexts[number].color = Color.gray;
            }
            else
            {
                StartCoroutine(ResetColors(true));
                Pie.HandleStrike();
            }

            if (stage == 6)
            {
                StartCoroutine(ResetColors(false));
                StartCoroutine(PlaySolveSound());
                Pie.HandlePass();
                bombSolved = true;
            }
            return false;
        };

    }

    IEnumerator ResetColors(bool strike)
    {
        if (strike)
        {
            foreach (TextMesh buttonText in buttonTexts) {
                buttonText.color = Color.red;
            }
            yield return new WaitForSeconds(.5f);
        }
        else
        {
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
            SetDisplay(new[] { "", "", "π", "", "" });
        }
        foreach (TextMesh buttonText in buttonTexts)
        {
            buttonText.color = Color.green;
        }
    }

    IEnumerator PlaySolveSound()
    {
        Audio.PlaySoundAtTransform("G", Pie.transform);
        yield return new WaitForSeconds(.2f);
        Audio.PlaySoundAtTransform("F", Pie.transform);
        yield return new WaitForSeconds(.2f);
        Audio.PlaySoundAtTransform("D", Pie.transform);
        yield return new WaitForSeconds(.2f);
        Audio.PlaySoundAtTransform("E", Pie.transform);
        yield return new WaitForSeconds(.2f);
        Audio.PlaySoundAtTransform("C", Pie.transform);
    }

    
    int[] CalculateAnswer()
    {
        int[] solution = { 0, 0, 0, 0, 0 };
        int calculation = codesNum;
        sumOfDigits = codesNum.ToString().Sum(x => int.Parse(x.ToString()));
        calculation += codePlace;
        calculation %= 100;
        print(calculation);

        solution = GetOrder(calculation);
        return solution;
    }

    int[] GetOrder(int number)
    {
        int[] order = { 0, 0, 0, 0, 0 };

        // 1st number => If the number is prime.
        order[0] = (new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97 }.Any(x => number == x)) ? 1 : 5;

        // 2nd number => if the number is even.
        if (number % 2 == 0) 
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

        print("" + order[0] + order[1] + order[2] + order[3] + order[4]);

        return order;
    }

    string[] PickNumber()
    {
        int i = Random.Range(1, 495);
        codePlace = i; // Keep track of place
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
}
