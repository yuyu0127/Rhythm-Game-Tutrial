public class TempoChange
{
	public float beat; // テンポ変化が起こるbeat
	public float tempo; // テンポ変化後のBPM

	// コンストラクタ
	public TempoChange(float beat, float tempo)
	{
		this.beat = beat;
		this.tempo = tempo;
	}
}