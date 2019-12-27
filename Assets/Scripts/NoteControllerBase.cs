using UnityEngine;

public abstract class NoteControllerBase : MonoBehaviour
{
	public NoteProperty noteProperty;
	public bool isProcessed = false; // ロングノーツ用処理中フラグ

	// キー押下時の処理
	public virtual void OnKeyDown(JudgementType judgementType) { }
	// キーを離したときの処理
	public virtual void OnKeyUp(JudgementType judgementType) { }
}