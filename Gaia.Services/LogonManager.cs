using System;
using Axis.Luna.Operation;
using Axis.Pollux.Identity.Contracts;
using Axis.Pollux.Logon.Contracts;
using Axis.Pollux.Logon.Contracts.Params;
using Gaia.Core.Contracts;
using Gaia.Core.Exceptions;
using Gaia.Core.Utils;
using static Axis.Luna.Extensions.ExceptionExtension;

namespace Gaia.Services
{

    public class LogonManager: ILogonManager
    {
        private readonly IAccountLogonService _logon;
        private readonly IAccountLogonInvalidator _logonInvalidator;
        private readonly ISessionContext _sessionContext;
        private readonly IUserContext _userContext;


        public LogonManager(
            IAccountLogonService logonService,
            IAccountLogonInvalidator logonInvalidator,
            IUserContext userContext,
            ISessionContext sessionContext)
        {
            ThrowNullArguments(
                () => logonService,
                () => sessionContext,
                () => userContext,
                () => logonInvalidator);

            _logon = logonService;
            _sessionContext = sessionContext;
            _userContext = userContext;
            _logonInvalidator = logonInvalidator;
        }


        public Operation<LogonSession> UserSignin(string userName, string password)
        => Operation.Try(async () =>
        {
            var userNameLogonCredential = new LogonCredential
            {
                Name = Constants.CredentialType_UserName,
                Value = userName
            };

            var passwordLogonCredential = new LogonCredential
            {
                Name = Constants.CredentialType_Password,
                Value = password
            };

            await userNameLogonCredential
                .Validate()
                .Then(() => passwordLogonCredential.Validate());

            return await _logon.Login(
                userNameLogonCredential,
                passwordLogonCredential);
        });

        public Operation SessionSignOut()
        => Operation.Try(async () =>
        {
            var sessionId = (await _sessionContext
                .CurrentSessionId())
                .ThrowIf(default(Guid), new GaiaException(ErrorCodes.DomainLogicError));

            var userId = (await _userContext
                .CurrentUserId())
                .ThrowIf(default(Guid), new GaiaException(ErrorCodes.DomainLogicError));

            await _logonInvalidator.InvalidateLogon(userId, sessionId);

        });
    }
}
