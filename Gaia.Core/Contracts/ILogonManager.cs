using Axis.Luna.Operation;
using Axis.Pollux.Logon.Contracts.Params;

namespace Gaia.Core.Contracts
{
    public interface ILogonManager
    {
        Operation<LogonSession> UserSignin(string userId, string password);

        /// <summary>
        /// Signs out the user from the current session - the current session can be gotten from the ISessionContext from the
        /// Axis.Pollux.Logon package
        /// </summary>
        /// <returns></returns>
        Operation SessionSignOut();
    }
}
