﻿namespace TrashMob.Shared.Models
{
    public class SocialMediaAccount : KeyedModel
    {
        public int SocialMediaAccountTypeId { get; set; }
        
        public string AccountIdentifier { get; set; }

        public virtual SocialMediaAccountType SocialMediaAccountType { get; set; }
    }
}