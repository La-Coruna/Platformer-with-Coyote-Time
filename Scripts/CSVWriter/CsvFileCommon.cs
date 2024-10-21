public abstract class CsvFileCommon
{
	protected char[] SpecialChars = new char[] { ',', '"', '\r', '\n' };
	
	private const int DelimiterIndex = 0;
	private const int QuoteIndex = 1;
	
	public char Delimiter
	{
		get { return SpecialChars[DelimiterIndex]; }
		set { SpecialChars[DelimiterIndex] = value; }
	}
	
	public char Quote
	{
		get { return SpecialChars[QuoteIndex]; }
		set { SpecialChars[QuoteIndex] = value; }
	}
}
