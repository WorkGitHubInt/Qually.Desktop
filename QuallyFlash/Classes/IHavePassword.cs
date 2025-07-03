using System.Security;

namespace QuallyFlash
{
    public interface IHavePassword
    {
        SecureString SecurePassword { get; }
    }
}
