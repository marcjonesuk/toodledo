using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Models.AccountViewModels
{
    public class UserProfileViewModel
    {
        public UserProfileViewModel()
        {
            //This is needed
        }

        public UserProfileViewModel(User user)
        {
            DisplayName = user.DisplayName;
            ProfileImageUrl = user.ProfileImageUrl;
            Id = user.Id;
        }

        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string ProfileImageUrl { get; set; }

        public User GetUser()
        {
            return new User()
            {
                Id = Id,
                DisplayName = DisplayName,
                ProfileImageUrl = ProfileImageUrl
            };
        }
    }
}
