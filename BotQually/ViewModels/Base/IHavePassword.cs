using System.Security;

namespace BotQually
{
    public interface IHavePassword
    {
        SecureString SecurePassword { get; }
    }
}
