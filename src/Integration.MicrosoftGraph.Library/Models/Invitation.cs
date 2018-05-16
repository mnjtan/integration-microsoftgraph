using Newtonsoft.Json;

namespace Integration.MicrosoftGraph.Service.Models
{
    public class Invitation
    {
        [JsonProperty(PropertyName= "@oData.Type")]
        public string odataType { get; set; }
        public string invitedUserDisplayName { get; set; }
        public string invitedUserEmailAddress { get; set; }

        // Indicates whether an email should be sent to the user being invited or not. The default is false.
        public bool sendInvitationMessage { get; set; }

        // The URL user should be redirected to once the invitation is redeemed. Required.
        public string inviteRedirectUrl { get; set; }

        // The URL user can use to redeem his invitation. Read-Only
        public string inviteRedeemUrl { get; set; }

        // The status of the invitation. Possible values: PendingAcceptance, Completed, InProgress, and Error
        public string status { get; set; }
    }
}