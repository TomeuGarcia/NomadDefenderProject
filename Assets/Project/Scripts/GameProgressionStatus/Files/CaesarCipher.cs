


public class CaesarCipher
{
    private readonly int _charCount;

    public CaesarCipher(int charCount)
    {
        _charCount = charCount;
    }

    public string Cipher(string content)
    {
        char[] codedContent = new char[content.Length];
        for (int i = 0; i < content.Length; ++i)
        {
            codedContent[i] = (char)(content[i] + _charCount);
        }
        
        return new string(codedContent);
    }
    
    public string Decipher(string content)
    {
        char[] codedContent = new char[content.Length];
        for (int i = 0; i < content.Length; ++i)
        {
            codedContent[i] = (char)(content[i] - _charCount);
        }
        
        return new string(codedContent);
    }
}