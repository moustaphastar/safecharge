using System;

namespace UI.SafeChargeModels
{
    [Serializable]
    public class Authenticate3dResult
    {
        public string Result { get; set; }
        public string ErrCode { get; set; }
        public string ErrorDescription { get; set; }
        public string UserPaymentOptionId { get; set; }
        public string Cavv { get; set; }
        public string Eci { get; set; }
        public string Xid { get; set; }
        public string DsTransID { get; set; }
        public string CcCardNumber { get; set; }
        public string Bin { get; set; }
        public string Last4Digits { get; set; }
        public string CcExpMonth { get; set; }
        public string CcExpYear { get; set; }
        public string TransactionId { get; set; }
        public string ThreeDReason { get; set; }
        public string ThreeDReasonId { get; set; }
        public string ChallengeCancelReasonId { get; set; }
        public string ChallengeCancelReason { get; set; }
        public string IsLiabilityOnIssuer { get; set; }
        public string ChallengePreferenceReason { get; set; }
        public string Cancelled { get; set; }
    }
}
