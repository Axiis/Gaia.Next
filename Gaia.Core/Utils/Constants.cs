namespace Gaia.Core.Utils
{
    public static class Constants
    {
        #region Credential Types

        public static readonly string CredentialType_Password = "Password";

        #endregion

        #region Credential Status

        public static readonly int CredentialStatus_Active = 1;
        public static readonly int CredentialStatus_Archived = 0;

        #endregion

        #region Contact Channels

        public static readonly string ContactChannel_Phone = "Phone";
        public static readonly string ContactChannel_Email = "Email";

        #endregion

        #region Contact Status

        public static readonly int ContactStatus_Active = 1;
        public static readonly int ContactStatus_Archived = 0;

        #endregion

        #region User Status

        public static readonly int UserStatus_Unverified = 0;

        public static readonly int UserStatus_Active = 1;

        public static readonly int UserStatus_Blocked = 2;

        public static readonly int UserStatus_Archived = 3;

        #endregion

        #region Multi-Factor Event Labels

        public static readonly string MultiFactorEventLabels_CooperativeAdminAccountVerification = "Gaia.MultiFactor.EventLabel.CooperativeAdminAccountVerification";
        public static readonly string MultiFactorEventLabels_FarmerAccountVerification = "Gaia.MultiFactor.EventLabel.FarmerAccountVerification";

        #endregion
    }
}
