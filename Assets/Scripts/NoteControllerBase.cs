using UnityEngine;

public abstract class NoteControllerBase : MonoBehaviour
{
	public NoteProperty noteProperty;
	// キー押下時の処理
	public virtual void OnKeyDown(JudgementType judgementType) { }
}