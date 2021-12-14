namespace master._7zip.Legacy
{
    public interface IPasswordProvider
    {
        string CryptoGetTextPassword();
    }

    public class Password : master._7zip.Legacy.IPasswordProvider
    {
        string _pw;

        public Password(string pw)
        {
            _pw = pw;
        }

        string master._7zip.Legacy.IPasswordProvider.CryptoGetTextPassword()
        {
            return _pw;
        }
    }
}
