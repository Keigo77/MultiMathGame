using UnityEngine;

public class CreateQuestion
{
    public int num1;
    public int num2;
    public int result;
    
    public CreateQuestion()
    {
        num1 = Random.Range(1, 50);
        num2 = Random.Range(1, 50);
        result = num1 + num2;
    }
}
