
using UnityEngine;

public class ActivationFunctions
{
    Functions function;
    public ActivationFunctions(Functions function)
    {
        this.function = function;
    }
    public enum Functions
    {
        SIGMOID,
        HYPERTAN,
        STEP,
        RELU,
        COLLZAT,
        HARDLIMIT
    }
    public float calculate(float weights)
    {
        switch (this.function)
        {
            case Functions.SIGMOID:
                return (float)(1 / (1 + Mathf.Pow((float)System.Math.E, -weights)));
            case Functions.HYPERTAN:
                return (float)((1 - Mathf.Pow((float)System.Math.E, -weights)) / (1 + Mathf.Pow((float)System.Math.E, -weights)));
            case Functions.COLLZAT:
                return (100 * weights) % 2 == 0 ? (weights / 2) : (3 * weights) + 1;
            case Functions.HARDLIMIT:
                return weights > .5f ? 1.0f : weights > 0.25f ? 0.5f : 0.0f;
            case Functions.RELU:
                return weights > 0 ? weights : 0.0f;
            case Functions.STEP:
                return weights > 0 ? 1.0f : 0.0f;
        }
        return weights > 0 ? 1.0f : 0.0f;
    }
}