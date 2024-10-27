using UnityEngine;

public class ExpressARUtility
{
    public class ExpressionResult
    {
        public string Name { get; set; }
        public float Value { get; set; }

        public ExpressionResult(string name, float value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
    }

    public static ExpressionResult getMax(FacialExpression expression)
    {
        float maxValue = Mathf.Max(expression.angry, expression.disgust, expression.fear, expression.happy, expression.sad, expression.surprise, expression.neutral);
        string maxName = "";

        if (maxValue == expression.angry) maxName = "angry";
        else if (maxValue == expression.disgust) maxName = "disgust";
        else if (maxValue == expression.fear) maxName = "fear";
        else if (maxValue == expression.happy) maxName = "happy";
        else if (maxValue == expression.sad) maxName = "sad";
        else if (maxValue == expression.surprise) maxName = "surprise";
        else if (maxValue == expression.neutral) maxName = "neutral";

        return new ExpressionResult(maxName, maxValue);
    }
}