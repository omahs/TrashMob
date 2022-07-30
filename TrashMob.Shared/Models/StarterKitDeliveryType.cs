﻿#nullable disable

namespace TrashMob.Shared.Models
{
    using System.Collections.Generic;

    public partial class StarterKitDeliveryType
    {
        public StarterKitDeliveryType()
        {
            CommunityStarterKits = new HashSet<CommunityStarterKit>();
        }

        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public int? DisplayOrder { get; set; }
        
        public bool? IsActive { get; set; }

        public virtual ICollection<CommunityStarterKit> CommunityStarterKits { get; set; }
    }
}
