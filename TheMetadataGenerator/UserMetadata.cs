namespace TheMetadataGenerator
{
    class UserMetadata : Metadata
    {
        public UserMetadata() : base("User")
        {
            AddField("FirstName", "string required");
            AddField("LastName", "string");
            AddField("Email", "email");
            AddField("CreditCard", "creditcard");
        }
    }
}
