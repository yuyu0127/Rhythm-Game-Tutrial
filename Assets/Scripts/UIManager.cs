using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text textValue;
    private string valueFormat;

    private void Start()
    {
        valueFormat = textValue.text;
    }

    private void Update()
    {
        textValue.text = string.Format(valueFormat,
            // {0}: スコア
            Mathf.CeilToInt(EvaluationManager.Score),
            // {1}: コンボ数
            EvaluationManager.Combo,
            // {2}: 最大コンボ数
            EvaluationManager.MaxCombo,
            // {3}: PERFECT
            EvaluationManager.JudgementCounts[JudgementType.Perfect],
            // {4}: GOOD
            EvaluationManager.JudgementCounts[JudgementType.Good],
            // {5}: BAD
            EvaluationManager.JudgementCounts[JudgementType.Bad],
            // {6}: MISS
            EvaluationManager.JudgementCounts[JudgementType.Miss]
        );
    }
}